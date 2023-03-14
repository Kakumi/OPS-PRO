using Godot;
using Godot.Collections;

public partial class DeckResource : Resource
{
    [Export]
    public Array<CardResource> Cards { get; set; }
    [Export]
    public string Name { get; set; }
}
