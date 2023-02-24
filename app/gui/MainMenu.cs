using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
    public override void _Ready()
    {

    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
