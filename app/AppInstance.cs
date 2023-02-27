using Godot;
using Serilog;
using System;
using System.Linq;

public class AppInstance : Control
{
    [Export]
    public PackedScene MainMenu { get; set; }

    [Signal]
    public delegate void ThemeChanged(Theme theme);

    public TextureRect Background { get; set; }
    public Control Content { get; set; }

    private static AppInstance _instance;
    public static AppInstance Instance => _instance;

    public override void _Ready()
    {
        _instance = this;

        Background = GetNode<TextureRect>("AspectRatioContainer/Background");
        Content = GetNode<Control>("Content");

        //Init size
        OS.WindowResizable = true;
        OS.WindowBorderless = false;
        OS.WindowMaximized = true;
        OS.WindowSize = new Vector2(1920, 1080);

        ShowMainMenu();
    }

    public void UpdateTheme(Theme theme)
    {
        Theme = theme;
        EmitSignal(nameof(ThemeChanged), theme);
    }

    public void GoTo(PackedScene packedScene)
    {
        if (packedScene != null)
        {
            var instance = packedScene.Instance();
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
        Content.GetChildren().QueueFreeAll();
    }

    public void ShowMainMenu()
    {
        if (MainMenu != null)
        {
            var instance = MainMenu.Instance();
            Content.CallDeferred("add_child", instance);
        }
    }
}
