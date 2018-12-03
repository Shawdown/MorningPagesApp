using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace MorningPagesApp
{
    public class DropboxManager
    {
        private static readonly string API_KEY = "9f7kpp1386tvyuf";

        // This loopback host is for demo purpose. If this port is not
        // available on your machine you need to update this URL with an unused port.
        private static readonly string LoopbackHost = "http://127.0.0.1:52475/";

        // URL to receive OAuth 2 redirect from Dropbox server.
        // You also need to register this redirect URL on https://www.dropbox.com/developers/apps.
        private static readonly Uri RedirectUri = new Uri(LoopbackHost + "authorize");

        // URL to receive access token from JS.
        private static readonly Uri JSRedirectUri = new Uri(LoopbackHost + "token");

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private bool initialized;
        private DropboxClient client;
        private HttpClient httpClient;

        public async Task<bool> Init()
        {
            DropboxCertHelper.InitializeCertPinning();

            var accessToken = await GetAccessToken();
            if (string.IsNullOrEmpty(accessToken))
            {
                return false;
            }

            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            try
            {
                var config = new DropboxClientConfig("MorningPager app")
                {
                    HttpClient = httpClient
                };

                client = new DropboxClient(accessToken, config);

                initialized = true;
            }
            catch (HttpException e)
            {
                Program.log.Info("Exception reported from RPC layer");
                Program.log.Info($"    Status code: {e.StatusCode}");
                Program.log.Info($"    Message    : {e.Message}");
                if (e.RequestUri != null)
                {
                    Program.log.Info($"    Request uri: {e.RequestUri}");
                }
            }

            return initialized;
        }

        /// <summary>
        /// Uploads given content to a file in Dropbox.
        /// </summary>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder to upload the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileContent">The file content.</param>
        /// <returns></returns>
        public async Task Upload(string fileName, string fileContent)
        {
            if (!initialized)
            {
                return;
            }

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(fileContent)))
            {
                var response = await client.Files.UploadAsync("/MorningPager/" + fileName, WriteMode.Overwrite.Instance, body: stream);
            }
        }

        /// <summary>
        /// Downloads a file.
        /// </summary>
        /// <remarks>This demonstrates calling a download style api in the Files namespace.</remarks>
        /// <param name="client">The Dropbox client.</param>
        /// <param name="folder">The folder path in which the file should be found.</param>
        /// <param name="file">The file to download within <paramref name="folder"/>.</param>
        /// <returns></returns>
        public async Task<string> Download (string fileName)
        {
            Console.WriteLine("Download file...");

            using (var response = await client.Files.DownloadAsync("/MorningPager/" + fileName))
            {
                Console.WriteLine("Downloaded {0} Rev {1}", response.Response.Name, response.Response.Rev);
                Console.WriteLine("------------------------------");
                Console.WriteLine(await response.GetContentAsStringAsync());
                Console.WriteLine("------------------------------");

                return await response.GetContentAsStringAsync();
            }
        }

        /// <summary>
        /// Gets the dropbox access token.
        /// <para>
        /// This fetches the access token from the applications settings, if it is not found there
        /// (or if the user chooses to reset the settings) then the UI in <see cref="LoginForm"/> is
        /// displayed to authorize the user.
        /// </para>
        /// </summary>
        /// <returns>A valid access token or null.</returns>
        private async Task<string> GetAccessToken()
        {
            var accessToken = ConfigManager.ReadSetting(ConfigKeys.DROPBOX_ACCESS_TOKEN);

            if (string.IsNullOrEmpty(accessToken))
            {
                try
                {
                    var state = Guid.NewGuid().ToString("N");
                    var authorizeUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, API_KEY, RedirectUri, state: state);
                    var http = new HttpListener();
                    http.Prefixes.Add(LoopbackHost);

                    http.Start();

                    System.Diagnostics.Process.Start(authorizeUri.ToString());

                    // Handle OAuth redirect and send URL fragment to local server using JS.
                    await HandleOAuth2Redirect(http);

                    // Handle redirect from JS and process OAuth response.
                    var result = await HandleJSRedirect(http);

                    if (result.State != state)
                    {
                        // The state in the response doesn't match the state in the request.
                        return null;
                    }

                    // Bring console window to the front.
                    //SetForegroundWindow(GetConsoleWindow());

                    accessToken = result.AccessToken;
                    var uid = result.Uid;
                    Program.log.Info($"Uid: {uid}");

                    ConfigManager.UpdateSetting(ConfigKeys.DROPBOX_ACCESS_TOKEN, accessToken);
                    ConfigManager.UpdateSetting(ConfigKeys.DROPBOX_UID, uid);
                }
                catch (Exception e)
                {
                    Program.log.Info($"Error: {e.Message}");
                    return null;
                }
            }

            return accessToken;
        }

        /// <summary>
        /// Handles the redirect from Dropbox server. Because we are using token flow, the local
        /// http server cannot directly receive the URL fragment. We need to return a HTML page with
        /// inline JS which can send URL fragment to local server as URL parameter.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task HandleOAuth2Redirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to RedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            // Respond with a page which runs JS and sends URL fragment as query string
            // to TokenRedirectUri.
            using (var file = File.OpenRead("index.html"))
            {
                file.CopyTo(context.Response.OutputStream);
            }

            context.Response.OutputStream.Close();
        }

        /// <summary>
        /// Handle the redirect from JS and process raw redirect URI with fragment to
        /// complete the authorization flow.
        /// </summary>
        /// <param name="http">The http listener.</param>
        /// <returns>The <see cref="OAuth2Response"/></returns>
        private async Task<OAuth2Response> HandleJSRedirect(HttpListener http)
        {
            var context = await http.GetContextAsync();

            // We only care about request to TokenRedirectUri endpoint.
            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

            var result = DropboxOAuth2Helper.ParseTokenFragment(redirectUri);

            return result;
        }
    }
}