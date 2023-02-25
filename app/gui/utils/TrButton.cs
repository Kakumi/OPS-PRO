using Godot;
using System;

public class TrButton : Button
{
    public override void _Ready()
    {
        Text = Tr(Text);
    }
}
