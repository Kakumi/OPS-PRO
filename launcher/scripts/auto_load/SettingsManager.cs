using Godot;
using Serilog;
using System;
using System.Linq;

public class SettingsManager : Node
{
    private ConfigFile _configFile;
    private string _path;
    private static SettingsManager _instance;

    public static SettingsManager Instance => _instance;

    public override void _Ready()
    {
        _instance = this;
        _path = "user://config.cfg";
        _configFile = new ConfigFile();
    }

    public void Init(IConfig config)
    {
        if (ConfigExist())
        {
            LoadConfig(config);
        }
        else
        {
            Log.Information($"Creating default config file...");
            config.CreateDefaultConfig();

            SaveConfig(config);
            config.ApplyChanges();
        }
    }

    public bool ConfigExist()
    {
        return new File().FileExists(_path);
    }

    public void LoadConfig(IConfig config)
    {
        _configFile.Load(_path);
        var defaultConfig = config.CreateDefaultConfig();

        //Properties
        foreach (var property in config.GetType().GetProperties())
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                var value = GetConfigValue(settings.Section, property.Name, property.PropertyType);
                if (value == null || value.GetType() != property.PropertyType)
                {
                    property.SetValue(config, property.GetValue(defaultConfig));
                }
                else
                {
                    property.SetValue(config, value);
                }
            }
        }

        Log.Information($"Config loaded");

        config.ApplyChanges();
    }

    public void SaveConfig(IConfig config)
    {
        Log.Information($"Saving config file...");

        //Properties
        foreach (var property in config.GetType().GetProperties())
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                if (property.PropertyType == typeof(Godot.Collections.Dictionary<object, object>))
                {
                    var dictionary = property.GetValue(config) as Godot.Collections.Dictionary<object, object>;
                    foreach (var pair in dictionary.ToList())
                    {
                        _configFile.SetValue(settings.Section.ToString(), pair.Key.ToString(), pair.Value);
                    }
                }
                else
                {
                    _configFile.SetValue(settings.Section.ToString(), property.Name, property.GetValue(config));
                }
            }
        }

        _configFile.Save(_path);
    }

    private object GetConfigValue(string section, string key, Type propertyType)
    {
        if (_configFile.HasSectionKey(section, key))
        {
            return Convert.ChangeType(_configFile.GetValue(section, key), propertyType);
        }

        return null;
    }
}
