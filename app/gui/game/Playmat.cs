using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Playmat : PanelContainer
{
	public List<CardResource> Deck { get; private set; }
	public List<CardResource> Trash { get; private set; }
	public List<CardResource> Lifes { get; private set; }
	public List<CardResource> DonDeck { get; private set; }
	public List<CardResource> CostArea { get; private set; }

	public Control LifesSlots { get; private set; }
	public SlotCard LeaderSlotCard { get; private set; }
	public SlotCard LastLifeSlotCard { get; private set; }

	[Signal]
	public delegate void MouseEnterCardEventHandler(Card card);

	[Signal]
	public delegate void MouseExitCardEventHandler(Card card);

	public override void _Ready()
	{
		LifesSlots = GetNode<Control>("Control/LifesSlots");

		LeaderSlotCard = GetNode<SlotCard>("Control/LeaderSlotCard");
		LastLifeSlotCard = LifesSlots.GetNode<SlotCard>("LastLifeSlotCard");

		LeaderSlotCard.Card.MouseEntered += () => OnCardMouseEntered(LeaderSlotCard.Card);
		LeaderSlotCard.Card.MouseExited += () => OnCardMouseExited(LeaderSlotCard.Card);

		var deck = DeckManager.Instance.LoadDecks().Where(x => x.IsValid()).First();
		Init(deck);
	}

	public void Init(DeckResource deckResource)
    {
		var leaderCardResource = deckResource.Cards.Keys.First(x => x.CardTypeList == CardTypeList.LEADER);
		LeaderSlotCard.Card.SetCardResource(leaderCardResource);

		//1 Because a life card already exist
		for(int i = 1; i < leaderCardResource.Cost; i++)
        {
			var newLifeNode = (SlotCard) LastLifeSlotCard.Duplicate();
			newLifeNode.Name = $"LifeSlotCard{leaderCardResource.Cost - i}";
			newLifeNode.Position = new Vector2(LastLifeSlotCard.Position.X, LastLifeSlotCard.Position.Y + (i * 30));
			LifesSlots.AddChild(newLifeNode);
		}
	}

	private void OnCardMouseEntered(Card card)
    {
		EmitSignal(SignalName.MouseEnterCard, card);
	}

	private void OnCardMouseExited(Card card)
	{
		EmitSignal(SignalName.MouseExitCard, card);
	}
}
