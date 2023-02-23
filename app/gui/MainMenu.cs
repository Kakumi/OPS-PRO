using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : PanelContainer
{
    [Export]
    public PackedScene SoundManager { get; set; }

    public override void _Ready()
    {
        //Init size
        OS.WindowResizable = true;
        OS.WindowBorderless = false;
        OS.WindowMaximized = true;
        OS.WindowSize = new Vector2(1920, 1080);
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        if (SoundManager != null)
        {
            var instance = SoundManager.Instance();
            GetTree().Root.CallDeferred("add_child", instance);
        }
    }
}
