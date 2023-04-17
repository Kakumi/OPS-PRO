using Godot;
using System;

public partial class HandContainer : PanelContainer
{
	[Export]
	public PackedScene CardScene { get; set; }

	public Container Hand { get; private set; }

	public override void _Ready()
	{
		Hand = GetNode<Container>("MarginContainer/Hand");
	}

	public Card AddCard(CardResource cardResource)
	{
		var instance = CardScene.Instantiate<Card>();
		Hand.AddChild(instance);
		instance.SetCardResource(cardResource);

		return instance;
	}
}
