using System;

[AttributeUsage(AttributeTargets.Property)]
public partial class ConfigSettings : Attribute
{
    public string Section { get; set; }
    public object Default { get; set; }

    public ConfigSettings(string section, object @default)
    {
        Section = section;
        Default = @default;
    }
}