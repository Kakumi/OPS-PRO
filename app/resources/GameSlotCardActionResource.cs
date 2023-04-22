using Godot;
using Godot.Collections;
using System;

public partial class GameSlotCardActionResource : Resource
{
    [Export]
    public CardSelectorSource Source { get; set; }

    [Export]
    public Array<CardSelectorAction> Actions { get; set; }
}
