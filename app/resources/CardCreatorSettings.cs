using Godot;
using Godot.Collections;

public partial class CardCreatorSettings : Resource
{
    [Export]
    public Array<CardCreatorColorResource> Colors { get; set; }

    [Export]
    public Array<CardCreatorAttribute> Attributes { get; set; }

    [Export]
    public Dictionary<string, string> Rarities { get; set; }

    [Export]
    public Array<Texture2D> CostTextures { get; set; }
}
