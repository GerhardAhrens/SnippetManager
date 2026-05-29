namespace SnippetManager.Core
{
    using System;
    using System.IO;
    using System.Media;

    public static class CustomSounds
    {
        // Beispiel 1: Ton aus den WPF-Ressourcen (Buildvorgang: Resource)
        public static CustomSoundItem ErrorBeep { get; } = new CustomSoundItem("pack://application:,,,/Sounds/error.wav");

        // Beispiel 2: Ton aus einer externen Datei im App-Ordner
        public static CustomSoundItem SuccessBeep { get; } = new CustomSoundItem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds/success.wav"));
    }

    public class CustomSoundItem : IDisposable
    {
        private readonly SoundPlayer _player;
        private bool _disposed;

        // Konstruktor für App-Ressourcen (pack://...)
        public CustomSoundItem(string uriPath)
        {
            var uri = new Uri(uriPath, UriKind.RelativeOrAbsolute);
            var streamInfo = System.Windows.Application.GetResourceStream(uri);
            if (streamInfo != null)
            {
                this._player = new SoundPlayer(streamInfo.Stream);
                this._player.Load(); // Lädt den Sound vorab in den Speicher
            }
        }

        // Konstruktor für direkte Datei­pfade
        public CustomSoundItem(string filePath, bool isFilePath = true)
        {
            if (File.Exists(filePath) == true)
            {
                this._player = new SoundPlayer(filePath);
                this._player.Load();
            }
        }

        public void Play()
        {
            this._player?.Play();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                this._player?.Dispose();
            }

            _disposed = true;
        }
    }
}
