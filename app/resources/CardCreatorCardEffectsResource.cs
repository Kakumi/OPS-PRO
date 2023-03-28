using Godot;
using Godot.Collections;

public partial class CardCreatorCardEffectsResource : Resource
{
    [Export]
    public bool ToggableBackgroundColor { get; set; }

    [Export]
    public Array<CardCreatorEffectResource> Values { get; set; }
}
