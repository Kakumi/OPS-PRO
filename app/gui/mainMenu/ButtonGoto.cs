using Godot;
using System;

public class ButtonGoto : Button
{
    [Export]
    public PackedScene Scene { get; set; }

    public override void _Ready()
    {
        
    }

    public void OnButtonPressed()
    {
        AppInstance.Instance.GoTo(Scene);
    }
}
