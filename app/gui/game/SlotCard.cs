using Godot;
using System;

public partial class SlotCard : TextureRect
{
	public Card Card { get; private set; }

	public override void _Ready()
	{
		Card = GetNode<Card>("MarginContainer/Card");
	}
}
