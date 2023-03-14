using Godot;
using Serilog;
using System;

public abstract partial class TabSettings : Godot.TabBar
{
	[Export]
	public string TabName { get; set; }

	public Settings Settings { get; protected set; }

	public override void _Ready()
	{
		Name = Tr(TabName);

		Settings = this.SearchParent<Settings>();
		Settings.VisibilityChanged += SettingsVisibilityChanged;
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
