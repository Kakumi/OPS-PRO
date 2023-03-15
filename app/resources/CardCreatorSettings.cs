using Godot;
using Godot.Collections;

public partial class CardCreatorSettings : Resource
{
    [Export]
    public Array<CardCreatorColorResource> Colors { get; set; }
}
