using Godot;
using Serilog;
using System;
using System.Linq;

public partial class AppInstance : Control
{
	[Export]
	public PackedScene MainMenu { get; set; }

	[Signal]
	public delegate void ThemeChangedEventHandler(Theme theme);

	public TextureRect Background { get; set; }
	public Control Content { get; set; }

	private static AppInstance _instance;
	public static AppInstance Instance => _instance;

	public override void _Ready()
	{
		_instance = this;

		Background = GetNode<TextureRect>("AspectRatioContainer/Background");
		Content = GetNode<Control>("Content");

		ShowMainMenu();
	}

	public void UpdateTheme(Theme theme)
	{
		Theme = theme;
		EmitSignal(SignalName.ThemeChanged, theme);
	}

	public void GoTo(PackedScene packedScene)
	{
		if (packedScene != null)
		{
			var instance = packedScene.Instantiate();
			if (instance != null)
			{
				ClearOthersControlNode();
				Content.AddChild(instance);
			}
			else
			{
				Log.Error($"Instance from packed scene {packedScene} is null.");
			}
		}
		else
		{
			Log.Error($"AppInstance, can't goto packed scene because null.");
		}
	}

	public void ClearOthersControlNode()
	{
		Content.GetChildren().ToList().ForEach(x => x.QueueFree());
	}

	public void ShowMainMenu()
	{
		if (MainMenu != null)
		{
			var instance = MainMenu.Instantiate();
			Content.CallDeferred("add_child", instance);
		}
	}
}
