namespace MorningPagesApp
{
    public interface IConfigListener
    {
        void OnConfigChanged(string key, string newValue);
    }
}