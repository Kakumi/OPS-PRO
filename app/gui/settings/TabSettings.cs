using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

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

	protected void SearchFiles(List<string> paths, string pattern, OptionButton select, ref Dictionary<int, string> files, string currentValuePath)
    {
		files = new Dictionary<int, string>();

		int id = 0;
		foreach (var path in paths)
		{
			Log.Debug($"Searching files inside {path}");
			if (DirAccess.DirExistsAbsolute(path))
			{
				var dir = DirAccess.Open(path);
				foreach (var file in dir.GetFiles(pattern))
				{
					var name = Path.GetFileName(file);
					while (Path.HasExtension(name))
                    {
						name = Path.GetFileNameWithoutExtension(name);
                    }

					select.AddItem(name, id);
					files.Add(id, file);

					if (file == currentValuePath)
					{
						select.Selected = id;
					}

					id++;
				}
			}
			else
			{
				Log.Warning($"Folder not found: {path}");
			}
		}
	}
}
