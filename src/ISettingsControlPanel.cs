namespace MorningPagesApp
{
    public interface ISettingsControlPanel
    {
        bool IsValid();
        void Save();
        void LoadSettings();
    }
}