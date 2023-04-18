using Godot;
using System;
using System.Linq;

public partial class HandContainer : PanelContainer
{
	[Export]
	public PackedScene CardScene { get; set; }

	public Container Hand { get; private set; }

	public override void _Ready()
	{
		Hand = GetNode<Container>("MarginContainer/Hand");
	}

	public SlotCard AddCard(CardResource cardResource)
	{
		var instance = CardScene.Instantiate<SlotCard>();
		Hand.AddChild(instance);
		instance.Card.SetCardResource(cardResource);
		instance.SizeFlagsHorizontal = SizeFlags.ExpandFill;

		return instance;
	}
}
