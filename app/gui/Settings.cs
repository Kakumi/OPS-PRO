using Godot;
using System;

public class Settings : Control
{
    public Config OriginalConfig { get; private set; }
    public Config UpdatedConfig { get; private set; }

    public override void _EnterTree()
    {
        OriginalConfig = new Config();
        SettingsManager.Instance.Init(OriginalConfig);

        UpdatedConfig = (Config)OriginalConfig.Clone();
    }

    public void OnQuitPressed()
    {
        UpdatedConfig = (Config)OriginalConfig.Clone();
        OriginalConfig.ApplyChanges();
        Hide();
    }

    public void OnSavePressed()
    {
        SettingsManager.Instance.SaveConfig(UpdatedConfig);
        OriginalConfig = (Config)UpdatedConfig.Clone();
        OriginalConfig.ApplyChanges();

        Hide();
    }
}
