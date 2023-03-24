using Godot;
using Godot.Collections;
using System;

public partial class CardCreatorCostResource : Resource
{
    [Export]
    public int MinCost { get; set; }

    [Export]
    public int MaxCost { get; set; }

    [Export]
    public Array<Texture2D> Textures { get; set; }
}
