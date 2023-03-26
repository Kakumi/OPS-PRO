using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public partial class Config : Resource, IConfig
{
    [ConfigSettings("Interface", "en")]
    public string Language { get; set; }

    [ConfigSettings("Interface", "res://app/resources/images/backgrounds/luffy_logo.jpg")]
    public string Background { get; set; }

    [ConfigSettings("Interface", "res://app/resources/themes/default_theme.tres")]
    public string Theme { get; set; }

    [ConfigSettings("Audio", 0.1f)]
    public float BackgroundMusicVolume { get; set; }

    [ConfigSettings("Audio", true)]
    public bool BackgroundMusicEnabled { get; set; }

    [ConfigSettings("Audio", "res://app/resources/sounds/background/One Piece OST Overtaken.ogg")]
    public string BackgroundMusic { get; set; }

    public void ApplyChanges()
    {
        Log.Information($"Applying changes from the config...");
        Log.Debug($"Setting locale to {Language}");
        Log.Debug($"Setting theme to {Theme}");
        Log.Debug($"Setting background to {Background}");
        Log.Debug($"Setting music volume to {BackgroundMusicVolume}");
        Log.Debug($"Setting sound to {BackgroundMusic} (enabled: {BackgroundMusicEnabled})");

        var defaultConfig = (Config) new Config().CreateDefaultConfig();
        if (TranslationServer.GetLoadedLocales().Any(x => x.ToLower() == Language.ToLower()))
        {
            TranslationServer.SetLocale(Language);
        } else
        {
            Log.Warning($"Language not found, set to default");
            TranslationServer.SetLocale(defaultConfig.Language);
        }

        if (FileAccess.FileExists(Theme) || ResourceLoader.Exists(Theme))
        {
            AppInstance.Instance.UpdateTheme(GD.Load<Theme>(Theme));
        }
        else
        {
            Log.Warning($"Theme not found, set to default");
            AppInstance.Instance.UpdateTheme(GD.Load<Theme>(defaultConfig.Theme));
        }

        if (FileAccess.FileExists(Background) || ResourceLoader.Exists(Background))
        {
            AppInstance.Instance.Background.Texture = GD.Load<Texture2D>(Background);
        }
        else
        {
            Log.Warning($"Background not found, set to default");
            AppInstance.Instance.Background.Texture = GD.Load<Texture2D>(defaultConfig.Background);
        }

        if (FileAccess.FileExists(BackgroundMusic) ||  ResourceLoader.Exists(BackgroundMusic))
        {
            SoundManager.Instance.UpdateSound(BackgroundMusic, BackgroundMusicEnabled);
        }
        else
        {
            Log.Warning($"Sound not found, set to default");
            SoundManager.Instance.UpdateSound(defaultConfig.BackgroundMusic, BackgroundMusicEnabled);
        }

        AudioServer.SetBusVolumeDb(0, Mathf.LinearToDb(BackgroundMusicVolume));
    }

    public IConfig CreateDefaultConfig()
    {
        foreach (var property in GetType().GetProperties())
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                property.SetValue(this, settings.Default);
            }
        }

        return this;
    }

    public IConfig Clone()
    {
        Config cloneConfig = new Config();
        var properties = cloneConfig.GetType().GetProperties();
        foreach (var property in properties)
        {
            var settings = property.GetCustomAttributes(true).OfType<ConfigSettings>().FirstOrDefault();
            if (settings != null)
            {
                var value = property.GetValue(this);
                property.SetValue(cloneConfig, value);
            }
        }

        return cloneConfig;
    }
}