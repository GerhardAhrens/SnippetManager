namespace SnippetManager.Data
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Id VARCHAR(36), Gruppe VARCHAR(50), Titel VARCHAR(50),Beschreibung VARCHAR(500), SnippetContent TEXT,CreatedOn DateTime,CreatedBy VARCHAR(50),ModifiedOn DateTime,ModifiedBy VARCHAR(50)
    /// </remarks>
    public class SnippetItem
    {
        public Guid Id { get; set; }
        public string Gruppe { get; set; }
        public string Titel { get; set; }
        public string Beschreibung { get; set; }
        public string SnippetContent { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}
