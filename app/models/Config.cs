using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class Config : Resource, IConfig
{
    [ConfigSettings("Interface")]
    public string Language { get; set; }

    [ConfigSettings("Interface")]
    public string Background { get; set; }

    [ConfigSettings("Interface")]
    public string Theme { get; set; }

    [ConfigSettings("Audio")]
    public float BackgroundMusicVolume { get; set; }

    [ConfigSettings("Audio")]
    public bool BackgroundMusicEnabled { get; set; }

    [ConfigSettings("Audio")]
    public string BackgroundMusic { get; set; }

    public void ApplyChanges()
    {
        Log.Information($"Applying changes from the config...");
        Log.Debug($"Setting locale to {Language}");
        Log.Debug($"Setting theme to {Theme}");
        Log.Debug($"Setting background to {Background}");
        Log.Debug($"Setting music volume to {BackgroundMusicVolume}");
        Log.Debug($"Setting sound to {BackgroundMusic} (enabled: {BackgroundMusicEnabled})");

        TranslationServer.SetLocale(Language);
        AppInstance.Instance.UpdateTheme(GD.Load<Theme>(Theme));
        AppInstance.Instance.Background.Texture = GD.Load<Texture>(Background);
        AudioServer.SetBusVolumeDb(0, GD.Linear2Db(BackgroundMusicVolume));
        SoundManager.Instance.UpdateSound(BackgroundMusic, BackgroundMusicEnabled);
    }

    public IConfig CreateDefaultConfig()
    {
        Language = TranslationServer.GetLocale().Substring(0, 2);
        Background = "res://app/resources/images/backgrounds/luffy_logo.jpg";
        Theme = "res://app/resources/themes/default_theme.tres";
        BackgroundMusicVolume = 0.1f; //GD.Db2Linear(AudioServer.GetBusVolumeDb(1));
        BackgroundMusicEnabled = true;
        BackgroundMusic = "res://app/resources/sounds/background/One Piece OST Overtaken.ogg";

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