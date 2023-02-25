using Godot;
using System;

public class TrLabel : Label
{
    public override void _Ready()
    {
        Text = Tr(Text);
    }
}
