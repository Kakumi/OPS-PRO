using Godot;
using Serilog;
using System;
using System.Text;

public class LauncherScene : PanelContainer
{
    public Label Title { get; protected set; }
    public RichTextLabel Message { get; protected set; }
    public ProgressBar ProgressBar { get; protected set; }
    public ProgressBar ProgressBar2 { get; protected set; }

    private string _path;

    public override void _Ready()
    {
        Title = GetNode<Label>("MarginContainer/HBoxContainer/Title");
        Message = GetNode<RichTextLabel>("MarginContainer/HBoxContainer/Message");
        ProgressBar = GetNode<ProgressBar>("MarginContainer/HBoxContainer/ProgressBar");
        ProgressBar2 = GetNode<ProgressBar>("MarginContainer/HBoxContainer/ProgressBar2");

        _path = ProjectSettings.GlobalizePath("user://launcher/main.pck");

        Init();
    }

    public bool HasFile()
    {
        return System.IO.File.Exists(_path);
    }

    public void Init()
    {
        if (HasFile())
        {
            var success = ProjectSettings.LoadResourcePack(_path);

            if (success)
            {
                Log.Information($"Launching app from PCK file...");
                var importedScene = ResourceLoader.Load<PackedScene>("res://app/gui/MainMenu.tscn");
                var instance = importedScene.Instance();
                GetTree().Root.CallDeferred("add_child", instance);
                QueueFree();
            }
            else
            {
                ChangeMessage("Failed to load the application, patch cannot be loaded.", true);
            }
        } else
        {
            Log.Warning($"PCK file not found.");

            OS.WindowResizable = false;
            OS.WindowBorderless = true;
            OS.WindowMaximized = false;
            OS.WindowSize = new Vector2(960, 540);

            var screenSize = OS.GetScreenSize(0);
            var windowSize = OS.WindowSize;
            var multiplier = new Vector2(0.5f, 0.5f);

            OS.WindowPosition = screenSize * multiplier - windowSize * multiplier;

            ChangeMessage("PCK File doesn't exists.", true);
        }
    }

    public void ChangeMessage(string message, bool error = false)
    {
        var sBuilder = new StringBuilder();
        sBuilder.Append("[center]");
        if (error)
        {
            sBuilder.Append("[color=red]Error:[/color] ");
        }

        sBuilder.Append(message);
        sBuilder.Append("[/center]");

        Message.BbcodeText = sBuilder.ToString();
    }
}
