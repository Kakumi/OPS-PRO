using Godot;
using System;

public class SecurityLoad : Node
{
    public override void _Ready()
    {
        GD.Print(string.Join(" - ", OS.GetCmdlineArgs()));
    }
}
