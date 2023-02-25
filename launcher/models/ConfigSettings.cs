using System;

[AttributeUsage(AttributeTargets.Property)]
public class ConfigSettings : Attribute
{
    public string Section { get; set; }

    public ConfigSettings(string section)
    {
        Section = section;
    }
}