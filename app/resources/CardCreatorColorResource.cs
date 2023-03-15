using Godot;
using Godot.Collections;
using System;

public partial class CardCreatorColorResource : Resource
{
	[Export]
	public string Name { get; set; }

	[Export]
	public Array<CardCreatorCardTypeResource> Types { get; set; }
}
