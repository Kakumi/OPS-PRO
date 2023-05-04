using Godot;
using Serilog;
using System;

public partial class Settings : Control
{
    public Config UpdatedConfig { get; private set; }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _EnterTree()
    {
        try
        {
            UpdatedConfig = SettingsManager.Instance.Config.Clone();
        } catch (Exception ex)
        {
            Log.Error(ex, $"Failed to load Config because {ex.Message}");
        }
    }

    public void OnQuitPressed()
    {
        UpdatedConfig = SettingsManager.Instance.Config.Clone();
        SettingsManager.Instance.Config.ApplyChanges();
        Hide();
    }

    public void OnSavePressed()
    {
        SettingsManager.Instance.SaveConfig(UpdatedConfig);
        SettingsManager.Instance.Config.ApplyChanges();

        Hide();
    }
}
