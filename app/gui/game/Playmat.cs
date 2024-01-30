using Godot;
using Godot.Collections;
using OPSProServer.Contracts.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Playmat : PanelContainer
{
	[Export]
	public NodePath PlayerAreaPath { get; set; }

	public PlayerArea PlayerArea { get; private set; }

	private List<PlayingCard> _deck;
	private List<PlayingCard> _trash;
	private Stack<PlayingCard> _lifes;
	private int _cardsDonDeck;
	public int CardsDonDeck
    {
		get => _cardsDonDeck;
		internal set
        {
			_cardsDonDeck = value;
			UpdateDon();
		}
	}
	private int _cardsCostDeck;
	public int CardsCostDeck
	{
		get => _cardsCostDeck;
		internal set
		{
			_cardsCostDeck = value;
			UpdateDon();
		}
	}
	private int _cardsRestedCostDeck;
	public int CardsRestedCostDeck
	{
		get => _cardsRestedCostDeck;
		internal set
		{
			_cardsRestedCostDeck = value;
			UpdateDon();
		}
	}

	public SlotCard LeaderSlotCard { get; private set; }
	public SlotCard StageSlotCard { get; private set; }
	public SlotCard DeckSlotCard { get; private set; }
	public SlotCard TrashSlotCard { get; private set; }
	public SlotCard CostSlotCard { get; private set; }
	public SlotCard DonDeckSlotCard { get; private set; }
	public SlotCard LifeSlotCard { get; private set; }
	public Container CharactersSlotsContainer { get; private set; }
	public List<SlotCard> CharactersSlots { get; private set; }
	public SlotCard CharactersSlot1 { get; private set; }
	public SlotCard CharactersSlot2 { get; private set; }
	public SlotCard CharactersSlot3 { get; private set; }
	public SlotCard CharactersSlot4 { get; private set; }
	public SlotCard CharactersSlot5 { get; private set; }

	public Label CostLabel { get; private set; }
	public Label LifeLabel { get; private set; }

	[Signal]
	public delegate void MouseEnterCardEventHandler(Card card);

	[Signal]
	public delegate void MouseExitCardEventHandler(Card card);

	[Signal]
	public delegate void CardDrawnEventHandler(CardResource cardResource);

	[Signal]
	public delegate void GameFinishedEventHandler(bool victory);

	[Signal]
	public delegate void CharactersChangedEventHandler(Array<CardResource> cards);

	public override void _Ready()
	{
		PlayerArea = GetNode<PlayerArea>(PlayerAreaPath);

		LeaderSlotCard = GetNode<SlotCard>("Control/LeaderSlotCard");
		StageSlotCard = GetNode<SlotCard>("Control/StageSlotCard");
		DeckSlotCard = GetNode<SlotCard>("Control/DeckSlotCard");
		TrashSlotCard = GetNode<SlotCard>("Control/TrashSlotCard");
		CostSlotCard = GetNode<SlotCard>("Control/CostSlotCard");
		DonDeckSlotCard = GetNode<SlotCard>("Control/DonDeckSlotCard");
		LifeSlotCard = GetNode<SlotCard>("Control/LifeSlotCard");

		CharactersSlotsContainer = GetNode<Container>("Control/CharactersSlots");
		CharactersSlots = CharactersSlotsContainer.GetChildren().OfType<SlotCard>().ToList();
		CharactersSlot1 = GetNode<SlotCard>("Control/CharactersSlots/CharacterSlotCard1");
		CharactersSlot2 = GetNode<SlotCard>("Control/CharactersSlots/CharacterSlotCard2");
		CharactersSlot3 = GetNode<SlotCard>("Control/CharactersSlots/CharacterSlotCard3");
		CharactersSlot4 = GetNode<SlotCard>("Control/CharactersSlots/CharacterSlotCard4");
		CharactersSlot5 = GetNode<SlotCard>("Control/CharactersSlots/CharacterSlotCard5");

        CostLabel = CostSlotCard.GetNode<Label>("Label");
		LifeLabel = LifeSlotCard.GetNode<Label>("Label");

		LeaderSlotCard.Card.MouseEntered += () => OnCardMouseEntered(LeaderSlotCard.Card);
		LeaderSlotCard.Card.MouseExited += () => OnCardMouseExited(LeaderSlotCard.Card);
		StageSlotCard.Card.MouseEntered += () => OnCardMouseEntered(StageSlotCard.Card);
		StageSlotCard.Card.MouseExited += () => OnCardMouseExited(StageSlotCard.Card);
		DeckSlotCard.Card.MouseEntered += () => OnCardMouseEntered(DeckSlotCard.Card);
		DeckSlotCard.Card.MouseExited += () => OnCardMouseExited(DeckSlotCard.Card);
		TrashSlotCard.Card.MouseEntered += () => OnCardMouseEntered(TrashSlotCard.Card);
		TrashSlotCard.Card.MouseExited += () => OnCardMouseExited(TrashSlotCard.Card);
		CostSlotCard.Card.MouseEntered += () => OnCardMouseEntered(CostSlotCard.Card);
		CostSlotCard.Card.MouseExited += () => OnCardMouseExited(CostSlotCard.Card);
		DonDeckSlotCard.Card.MouseEntered += () => OnCardMouseEntered(DonDeckSlotCard.Card);
		DonDeckSlotCard.Card.MouseExited += () => OnCardMouseExited(DonDeckSlotCard.Card);
		LifeSlotCard.Card.MouseEntered += () => OnCardMouseEntered(LifeSlotCard.Card);
		LifeSlotCard.Card.MouseExited += () => OnCardMouseExited(LifeSlotCard.Card);
		CharactersSlots.ForEach(x =>
		{
			x.Card.MouseEntered += () => OnCardMouseEntered(x.Card);
			x.Card.MouseExited += () => OnCardMouseExited(x.Card);
		});

        DeckSlotCard.Card.Visible = false;
        LifeSlotCard.Card.Visible = false;
        TrashSlotCard.Card.Visible = false;
        DonDeckSlotCard.Card.Visible = false;
        CostSlotCard.Card.Visible = false;
    }

	private void OnResized()
    {
		PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);
	}


	private void Playmat_TrashChanged(Array<CardResource> cards)
	{
		Log.Debug("Trash changed, update card slot");
		TrashSlotCard.Card.Visible = cards.Count > 0;
	}

    public void SetDeck(List<PlayingCard> deck)
	{
		Log.Debug("Deck changed, update card slot");
		_deck = deck;
		DeckSlotCard.Card.Visible = deck.Count > 0;
	}

    public void SetLifes(Stack<PlayingCard> lifeDeck)
	{
		Log.Debug("Life changed, update card slot");
		_lifes = lifeDeck;
		LifeLabel.Text = string.Format(Tr("GAME_LIFE_COUNTER"), lifeDeck.Count);
		LifeSlotCard.Card.Visible = lifeDeck.Count > 0;
	}

    public void SetTrash(List<PlayingCard> trashDeck)
    {
        Log.Debug("Life changed, update card slot");
        _trash = trashDeck;
        TrashSlotCard.Card.Visible = trashDeck.Count > 0;
    }

    private void OnCardMouseEntered(Card card)
	{
		EmitSignal(SignalName.MouseEnterCard, card);
	}

	private void OnCardMouseExited(Card card)
	{
		EmitSignal(SignalName.MouseExitCard, card);
	}

	private void ChangeMessage(string message, string color = "red")
	{
		PlayerArea.PlayerInfo.UpdateMessage(message, color);
	}

    private async void OnCardAction(SlotCard slotCard, GameSlotCardActionResource resource, int id)
    {
		await CallCardAction(slotCard, resource, id);
    }

	public async Task CallCardAction(SlotCard slotCard, GameSlotCardActionResource resource, int id)
	{
        if (Enum.IsDefined(typeof(CardAction), id))
        {
            CardAction action = (CardAction)id;
            Log.Debug("Card action clicked for source {Source} and action {Action}", resource.Source, action);

            switch (action)
            {
                case CardAction.See:
                    SeeCards(slotCard, resource);
                    break;
                case CardAction.Throw:
                    break;
                case CardAction.Discard:
                    break;
                case CardAction.Attack:
					await GameSocketConnector.Instance.GetAttackableCards(slotCard.Card.PlayingCard.Id);
                    break;
                case CardAction.Summon:
                    await GameSocketConnector.Instance.Summon(slotCard.Card.PlayingCard.Id);
                    break;
				case CardAction.GiveDon:
					await GameSocketConnector.Instance.GiveDonCard(slotCard.Card.PlayingCard.Id);
					break;
                default:
                    Log.Error("Card action invalid for id {Id}, not implemented.", id);
                    break;
            }
        }
        else
        {
            Log.Error("Can't parse card action id {Id}, it doesn't exist.", id);
        }
    }

    private void SeeCards(SlotCard slotCard, GameSlotCardActionResource resource)
    {
        Log.Debug("Executing see method for source {Source}", resource.Source);

        switch (resource.Source)
        {
            case CardSource.Deck:
                var cards = _deck.OrderBy(x => Guid.NewGuid()).ToList();
                PlayerArea.Gameboard.ShowCardsDialog(cards.Select(x => x.GetCardResource()).ToList(), resource.Source);
                break;
            case CardSource.Trash:
                PlayerArea.Gameboard.ShowCardsDialog(_trash.Select(x => x.GetCardResource()).ToList(), resource.Source);
                break;
            case CardSource.OpponentTrash:
                PlayerArea.Gameboard.ShowCardsDialog(PlayerArea.Gameboard.OpponentArea.Playmat.GetTrash().Select(x => x.GetCardResource()).ToList(), resource.Source);
                break;
            default:
                Log.Warning("No cards to see because not supported.");
                break;
        }
    }

    public IEnumerable<PlayingCard> GetTrash()
    {
        return (IEnumerable<PlayingCard>) _trash;
    }

    public IEnumerable<PlayingCard> GetLifes()
    {
        return (IEnumerable<PlayingCard>)_lifes;
    }

    public IEnumerable<PlayingCard> GetDeck()
    {
        return (IEnumerable<PlayingCard>)_deck;
    }

    private void UpdateDon()
    {
		CostLabel.Text = string.Format(Tr("GAME_DON_COUNTER"), CardsDonDeck, CardsCostDeck, CardsRestedCostDeck);
		DonDeckSlotCard.Card.Visible = CardsDonDeck > 0;
		CostSlotCard.Card.Visible = CardsCostDeck > 0;
	}

    public void SetCharacters(List<PlayingCard> cards)
    {
		CharactersSlot1.Card.UpdateCard(cards[0]);
		CharactersSlot2.Card.UpdateCard(cards[1]);
		CharactersSlot3.Card.UpdateCard(cards[2]);
		CharactersSlot4.Card.UpdateCard(cards[3]);
		CharactersSlot5.Card.UpdateCard(cards[4]);
    }

	public List<SlotCard> GetCharacters()
	{
		return CharactersSlots.Where(x => x != null && x.Card != null && x.Card.CardResource != null).ToList();
	}
}
