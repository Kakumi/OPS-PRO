using Godot;
using System;

public partial class CardCreatorCardTypeSettingsResource : Resource
{
    private string _name;
    [Export]
    public string Name
    {
        get => Tr(_name); 
        set => _name = value;
    }

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

    [Export]
    public CardCreatorCostResource Costs { get; set; }

    [Export]
    public CardCreatorCardEffectsResource Effects { get; set; }
}
