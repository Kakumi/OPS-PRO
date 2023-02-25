using Godot;
using Serilog;
using System;

public abstract class TabSettings : Godot.Tabs
{
    [Export]
    public string TabName { get; set; }

    public Settings Settings { get; protected set; }

    public override void _Ready()
    {
        Name = Tr(TabName);

        Settings = this.SearchParent<Settings>();
        Settings.ConnectIfMissing("visibility_changed", this, nameof(SettingsVisibilityChanged));
    }

    protected virtual void SettingsVisibilityChanged()
    {
        try
        {
            if (Settings.Visible)
            {
                Init();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to init settings tab because {ex.Message}");
        }
    }

    public abstract void Init();
}
