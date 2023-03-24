using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class CardCreatorCardTypeResource : Resource
{
    [Export]
    public string Suffix { get; set; }

    [Export]
    public Color TextColor { get; set; } = new Color(255, 255, 255, 1);

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
