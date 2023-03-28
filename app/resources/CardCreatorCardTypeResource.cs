using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class CardCreatorCardTypeResource : Resource
{
    private string _suffix;
    [Export]
    public string Suffix
    {
        get => Tr(_suffix);
        set => _suffix = value;
    }

    [Export]
    public bool HasColor { get; set; } = true;

    [Export]
    public Color TextColor { get; set; } = new Color(255, 255, 255, 1);

    [Export]
    public bool HasSecondaryColor { get; set; } = true;

    [Export]
    public Color SecondaryTextColor { get; set; } = new Color(0, 0, 0, 1);

    [Export]
    public CardCreatorCardTypeSettingsResource Settings { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    public string GetFullName()
    {
        return string.IsNullOrWhiteSpace(Suffix) ? Settings.Name : $"{Settings.Name} ({Suffix})";
    }
}
