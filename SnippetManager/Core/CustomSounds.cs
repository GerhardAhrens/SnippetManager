namespace SnippetManager.Core
{
    using System;
    using System.IO;
    using System.Media;
    using System.Reflection;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    public class CustomSounds
    {
        private const string RootNamespace = "SnippetManager.Resources";
        private readonly string name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSounds"/> class.
        /// </summary>
        public CustomSounds(string name)
        {
            this.name = name;
        }

        public static CustomSounds SoundCapture
        {
            get
            {
                return new CustomSounds("SoundCapture");
            }
        }

        public static CustomSounds SoundFailure
        {
            get
            {
                return new CustomSounds("SoundFailure");
            }
        }

        public static CustomSounds SoundSuccess
        {
            get
            {
                return new CustomSounds("SoundSuccess");
            }
        }

        public static CustomSounds AccessAllowedTone
        {
            get
            {
                return new CustomSounds("AccessAllowedTone");
            }
        }

        public static CustomSounds ArabianMysteryNotification
        {
            get
            {
                return new CustomSounds("ArabianMysteryNotification");
            }
        }

        public static CustomSounds ArcadeMagicNotification
        {
            get
            {
                return new CustomSounds("ArcadeMagicNotification");
            }
        }

        public static CustomSounds BellNotification
        {
            get
            {
                return new CustomSounds("BellNotification");
            }
        }

        public static CustomSounds CashMachineKeyPress
        {
            get
            {
                return new CustomSounds("CashMachineKeyPress");
            }
        }

        public static CustomSounds ClearAnnounceTones
        {
            get
            {
                return new CustomSounds("ClearAnnounceTones");
            }
        }

        public static CustomSounds ConfirmationTone
        {
            get
            {
                return new CustomSounds("ConfirmationTone");
            }
        }

        public static CustomSounds GuitarNotificationAlert
        {
            get
            {
                return new CustomSounds("GuitarNotificationAlert");
            }
        }

        public static CustomSounds HappyBellsNotification
        {
            get
            {
                return new CustomSounds("HappyBellsNotification");
            }
        }

        public static CustomSounds MagicMarimba
        {
            get
            {
                return new CustomSounds("MagicMarimba");
            }
        }

        public static CustomSounds MelodicalFluteMusicNotification
        {
            get
            {
                return new CustomSounds("MelodicalFluteMusicNotification");
            }
        }

        public static CustomSounds MusicalAlertNotification
        {
            get
            {
                return new CustomSounds("MusicalAlertNotification");
            }
        }

        public static CustomSounds MusicalReveal
        {
            get
            {
                return new CustomSounds("MusicalReveal");
            }
        }

        public static CustomSounds OrchestralEmergencyAlarm
        {
            get
            {
                return new CustomSounds("OrchestralEmergencyAlarm");
            }
        }

        public static CustomSounds PositiveNotification
        {
            get
            {
                return new CustomSounds("PositiveNotification");
            }
        }

        public static CustomSounds SoftwareInterfaceBack
        {
            get
            {
                return new CustomSounds("SoftwareInterfaceBack");
            }
        }

        public static CustomSounds SuccessTone
        {
            get
            {
                return new CustomSounds("SuccessTone");
            }
        }

        public static CustomSounds UpliftingFluteNotification
        {
            get
            {
                return new CustomSounds("UpliftingFluteNotification");
            }
        }

        public bool HasSound
        {
            get
            {
                try
                {
                    bool soundFound = HasResource($"{this.name}.wav");
                    return soundFound;
                }
                catch
                {
                    return false;
                }
            }
        }

        public void Play()
        {
            try
            {
                if (this.name == "Beep")
                {
                    Console.Beep();
                }
                else
                {
                    Stream resourceStream = GetResourceStream($"Sound.{this.name}.wav");
                    SoundPlayer player = new SoundPlayer(resourceStream);
                    player.Play();
                }
            }
            catch
            {
                Console.Beep();
            }
        }

        private static bool HasResource(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName) == true)
            {
                return false;
            }

            try
            {
                string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                int count = names.ToList().Count(p => p.Contains(resourceName, StringComparison.InvariantCultureIgnoreCase) == true);

                return count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private static Stream GetResourceStream(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName) == true)
            {
                return null;
            }

            try
            {
                string streamName = $"{RootNamespace}.{resourceName}";
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
                if (stream != null)
                {
                    return stream;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                string errortext = $"Can't load Image from {resourceName}, Error: {ex.Message}";
                throw new ArgumentException(errortext);
            }
        }
    }
}
