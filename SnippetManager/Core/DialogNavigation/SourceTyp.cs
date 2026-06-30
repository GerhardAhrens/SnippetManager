namespace SnippetManager.Core
{
    using System.ComponentModel;

    public enum SourceTyp
    {
        [Description("Keine Auswahl")]
        None = 0,
        [Description("UserControl mit Kontruktor Parameter")]
        UserControlWithArgs = 1,
        [Description("UserControl ohne Kontruktor Parameter")]
        UserControlWithoutArgs = 2,
        [Description("UserControl Standard")]
        UserControlDefault = 21,
        [Description("WPF Window ohne Kontruktor Parameter")]
        Window = 3,
        [Description("WPF Dialog Window ohne Kontruktor Parameter")]
        DialogWindow = 4,
        [Description("Enum Klasse")]
        EnumClass = 5,
        [Description("Default Klasse")]
        DefaultClass = 6,
        [Description("Static Extension Klasse")]
        ExtensionClass = 7,
        [Description("Static ExtensionBlock Klasse")]
        ExtensionBlockClass = 8,
        [Description("Interface Klasse")]
        InterfaceClass = 9,
        [Description("Record Klasse")]
        RecordClass = 10,
        [Description("Struct Klasse")]
        StructClass = 11,
        [Description("Custom Data Type Klasse")]
        CustomDataTypeClass = 12,
        [Description("Dependency Property")]
        DependencyProperty = 13,
        [Description("Dependency Property with Callback")]
        DependencyPropertyCallback = 14,
    }
}
