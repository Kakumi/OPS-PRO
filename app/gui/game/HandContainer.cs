using Godot;
using OPSProServer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class HandContainer : PanelContainer
{
	[Export]
	public GameSlotCardActionResource CardActionResource { get; set; }

	[Export]
	public PackedScene CardScene { get; set; }

	public PlayerArea PlayerArea { get; private set; }

	[Export]
	public NodePath PlayerAreaPath { get; set; }

	public Container Hand { get; private set; }

	public override void _Ready()
	{
		PlayerArea = GetNode<PlayerArea>(PlayerAreaPath);

		Hand = GetNode<Container>("MarginContainer/Hand");
	}

	public void SetCards(List<PlayingCard> playingCards)
	{
		//Create or update cards
		foreach(var playingCard in playingCards)
        {
			var handSlotCard = Hand.GetChildren().OfType<SlotCard>().FirstOrDefault(x => x.Card.PlayingCard.Id == playingCard.Id);
			if (handSlotCard == null)
            {
                var cardResource = playingCard.GetCardResource();
                var instance = CardScene.Instantiate<SlotCard>();
                Hand.AddChild(instance);
                instance.Card.UpdateCard(playingCard);
                instance.MouseEntered += () => PlayerArea.Gameboard.GameView.CardInfoPanel.ShowcardResource(instance.Card);
                instance.MouseExited += () => PlayerArea.Gameboard.GameView.CardInfoPanel.ShowcardResource(instance.Card);
                instance.SizeFlagsHorizontal = SizeFlags.ExpandFill;
                instance.CardActionResource = CardActionResource;
                instance.CardActionUpdated(PlayerArea.CurrentPhase);
                instance.CardAction += Instance_CardAction;
            } else
			{
				handSlotCard.Card.UpdateCard(playingCard);
			}
        }

		//Remove all others cards
		Hand.GetChildren().OfType<SlotCard>().ToList().ForEach(x =>
		{
			if (!playingCards.Any(y => y.Id == x.Card.PlayingCard.Id)) {
				x.QueueFree();
			}
		});
    }

    private async void Instance_CardAction(SlotCard slotCard, GameSlotCardActionResource resource, int id)
    {
		await PlayerArea.Playmat.CallCardAction(slotCard, resource, id);
	}

	public List<SlotCard> GetCards()
    {
		return Hand.GetChildren().OfType<SlotCard>().ToList();
    }
}
