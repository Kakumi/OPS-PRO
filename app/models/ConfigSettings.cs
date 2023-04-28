using System;

[AttributeUsage(AttributeTargets.Property)]
public partial class ConfigSettings : Attribute
{
    public string Section { get; set; }
    public object Default { get; set; }

    public ConfigSettings(string section, object @default, bool unique = false)
    {
        Section = section;
        Default = @default;

        if (unique)
        {
            Default += DateTime.Now.ToString("yyyyMMddmmss");
        }
    }
}