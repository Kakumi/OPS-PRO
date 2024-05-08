using Godot;
using Godot.Collections;
using OPSProServer.Contracts.Models;
using System;

public partial class GameSlotCardActionResource : Resource
{
    [Export]
    public CardSource Source { get; set; }

    [Export]
    public Array<CardAction> Actions { get; set; }
}
