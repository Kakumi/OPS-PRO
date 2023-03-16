using Godot;
using System;

public partial class CardCreatorCardTypeResource : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public PackedScene Template { get; set; }

    [Export]
    public bool HasTitle { get; set; } = true;

    [Export]
    public bool HasType { get; set; } = true;

    [Export]
    public bool HasNumber { get; set; } = true;

    [Export]
    public bool HasRarity { get; set; } = true;

    [Export]
    public bool HasAttribute { get; set; } = true;

    [Export]
    public bool HasCost { get; set; } = true;

    [Export]
    public bool HasCounter { get; set; } = true;

    [Export]
    public bool HasPower { get; set; } = true;
}
