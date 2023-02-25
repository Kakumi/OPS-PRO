using Godot;
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
        if (Settings.Visible)
        {
            Init();
        }
    }

    public abstract void Init();
}
