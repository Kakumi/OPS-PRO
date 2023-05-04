using Godot;
using Serilog;
using System;
using System.Linq;

public partial class SettingsManager : Node
{
    private ConfigFile _configFile;
    private string _path;
    private static SettingsManager _instance;

    public Config Config { get; private set; }

    public static SettingsManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;
        _path = "user://config.cfg";
        _configFile = new ConfigFile();

        Init();
    }

    private void Init()
    {
        if (ConfigExist())
        {
            Config = ReadConfig();
        }
        else
        {
            Log.Information($"Creating default config file...");
            Config = new Config().CreateDefaultConfig();
            SaveConfig(Config);
        }
    }

    private bool ConfigExist()
    {
        return FileAccess.FileExists(_path);
    }

    private Config ReadConfig()
    {
        _configFile.Load(_path);
        var config = new Config().CreateDefaultConfig();

        //Properties
        foreach (var property in config.GetType().GetProperties())
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                var value = GetConfigValue(settings.Section, property.Name, property.PropertyType, settings.Default);
                if (value != null && value.GetType() == property.PropertyType)
                {
                    property.SetValue(config, value);
                }
            }
        }

        Log.Information($"Config loaded");
        return config;
    }

    private object GetConfigValue(string section, string key, Type propertyType, object @default)
    {
        if (_configFile.HasSectionKey(section, key))
        {
            return Convert.ChangeType(_configFile.GetValue(section, key, @default.ToVariant()).Obj, propertyType);
        }

        return null;
    }

    public void SaveConfig(Config config)
    {
        Log.Information($"Saving config file...");
        Config = config.Clone();

        //Properties
        foreach (var property in config.GetType().GetProperties())
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                _configFile.SetValue(settings.Section.ToString(), property.Name, property.GetValue(config).ToVariant());
            }
        }

        _configFile.Save(_path);
    }
}
