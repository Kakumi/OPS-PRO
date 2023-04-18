using Godot;
using System;
using System.Linq;

public partial class HandContainer : PanelContainer
{
	[Export]
	public PackedScene CardScene { get; set; }

	public Container Hand { get; private set; }

	[Signal]
	public delegate void InvokeCardEventHandler(Card card);

	public override void _Ready()
	{
		Hand = GetNode<Container>("MarginContainer/Hand");
	}

	public Card AddCard(CardResource cardResource)
	{
		var instance = CardScene.Instantiate<Card>();
		Hand.AddChild(instance);
		instance.SetCardResource(cardResource);

        instance.LeftClickCard += (x) => Card_LeftClickCard(instance, x);

		return instance;
	}

	private void Card_LeftClickCard(Card card, CardResource cardResource)
    {
        if (cardResource.CardTypeList == CardTypeList.CHARACTER)
        {
			EmitSignal(SignalName.InvokeCard, card);
        }
    }
}
