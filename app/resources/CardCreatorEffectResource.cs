using Godot;
using System;

public partial class CardCreatorEffectResource : Resource
{
    [Export]
    public string EffectName { get; set; }

    [Export]
    public bool HasText { get; set; }

    [Export]
    public bool HasDon { get; set; }

    [Export]
    public bool HasDamage { get; set; }

    [Export]
    public PackedScene TemplateText { get; set; }
}
