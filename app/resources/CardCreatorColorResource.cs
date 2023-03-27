using Godot;
using Godot.Collections;
using System;

public partial class CardCreatorColorResource : Resource
{
	private string _name;
	[Export]
	public string Name
    {
		get => Tr(_name); 
		set => _name = value; 
    }

	[Export]
	public Array<CardCreatorCardTypeResource> Types { get; set; }
}
