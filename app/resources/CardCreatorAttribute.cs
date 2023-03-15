using Godot;
using System;

public partial class CardCreatorAttribute : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Texture { get; set; }
}
