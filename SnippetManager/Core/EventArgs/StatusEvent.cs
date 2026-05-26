namespace SnippetManager.Core
{
    using System;

    public sealed class StatusEvent
    {
        public StatusEvent(string databaseInfo, string databaseInfoTooltip)
        {
            this.Id = Guid.CreateVersion7();
            this.DatabaseInfo = databaseInfo;
            this.DatabaseInfoTooltip = databaseInfoTooltip;
        }

        public StatusEvent(string notification)
        {
            this.Id = Guid.CreateVersion7();
            this.Notification = notification;
        }

        public Guid Id { get; private set; }
        public string DatabaseInfo { get; private set; }
        public string DatabaseInfoTooltip { get; private set; }
        public string Notification { get; private set; }
    }
}
