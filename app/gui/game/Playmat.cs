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

    //   public void Init(DeckResource deckResource)
    //{
    //	Log.Information("Initializing game with deck {Name}", deckResource.Name);
    //	var cards = deckResource.Cards.ToList();
    //	var leaderCardResource = cards.First(x => x.Key.CardTypeList == CardTypeList.LEADER);
    //	LeaderSlotCard.Card.SetCardResource(leaderCardResource.Key);

    //	_deck = new List<CardResource>();
    //	DeckSlotCard.Card.Hide();
    //	_trash = new List<CardResource>();
    //	TrashSlotCard.Card.Hide();
    //	_lifes = new Stack<CardResource>();
    //	LifeSlotCard.Card.Hide();
    //	CardsDonDeck = 10;
    //	CardsCostDeck = 0;
    //	CardsRestedCostDeck = 0;
    //	DonDeckSlotCard.Card.Hide();
    //	CostSlotCard.Card.Hide();

    //	var deckCards = cards.Where(x => x.Key.CardTypeList == CardTypeList.CHARACTER || x.Key.CardTypeList == CardTypeList.STAGE || x.Key.CardTypeList == CardTypeList.EVENT);
    //	foreach(var deckCard in deckCards)
    //       {
    //		for(int i = 0; i < deckCard.Value; i++)
    //           {
    //			AddDeckCard(deckCard.Key);
    //		}
    //       }

    //	ShuffleDeck();

    //	DrawCard(5);

    //	RemoveDeckCards(leaderCardResource.Key.Cost).ForEach(x =>
    //	{
    //		AddLifeCard(x);
    //	});
    //}

    ////public async Task<bool> SyncPlaymat()
    //   //{
    //	//var playmatSync = new PlaymatSync()
    //	//{
    //	//	UserId = GameSocketConnector.Instance.UserId,
    //	//	Leader = LeaderSlotCard.Guid,
    //	//	Life = LifeSlotCard.Guid,
    //	//	Deck = DeckSlotCard.Guid,
    //	//	Stage = StageSlotCard.Guid,
    //	//	Trash = TrashSlotCard.Guid,
    //	//	Cost = CostSlotCard.Guid,
    //	//	DonDeck = DonDeckSlotCard.Guid,
    //	//	Characters = CharactersSlots.Select(x => x.Guid).ToList()
    //	//};

    //	//return await GameSocketConnector.Instance.SyncBoard(playmatSync);
    ////}

    //public void AddDeckCard(CardResource cardResource)
    //   {
    //	_deck.Add(cardResource);
    //	EmitSignal(SignalName.DeckChanged, new Array<CardResource>(_deck));
    //}

    //private List<CardResource> RemoveDeckCards(int amount = 1)
    //{
    //	if (_deck.Count >= amount)
    //	{
    //		Log.Information("Remove {Amount} cards from deck", amount);
    //		var cards = _deck.Take(amount).ToList();
    //		_deck.RemoveRange(0, amount);
    //		EmitSignal(SignalName.DeckChanged, new Array<CardResource>(_deck));

    //		return cards;
    //	}

    //	EmitSignal(SignalName.GameFinished, false);
    //	return null;
    //}

    //public void AddLifeCard(CardResource cardResource)
    //{
    //	Log.Information("Add 1 life card");
    //	_lifes.Push(cardResource);
    //	EmitSignal(SignalName.LifeChanged, new Array<CardResource>(_lifes));
    //}

    //public CardResource RemoveLifeCard()
    //{
    //	Log.Information("Remove 1 life card");
    //	var cardResource = _lifes.Pop();
    //	EmitSignal(SignalName.LifeChanged, new Array<CardResource>(_lifes));

    //	return cardResource;
    //}

    //private List<CardResource> GetCharacters()
    //   {
    //	return CharactersSlots.Where(x => x.Card.CardResource != null).Select(x => x.Card.CardResource).ToList();
    //}

    //public bool AddCharacter(CardResource cardResource)
    //{
    //	var emptySlot = CharactersSlots.FirstOrDefault(x => x.Card.CardResource == null);
    //	if (emptySlot == null)
    //       {
    //		ChangeMessage(Tr("GAME_CHARACTERS_FULL"));
    //		return false;
    //       }

    //	if (cardResource.Cost > CardsCostDeck)
    //	{
    //		ChangeMessage(Tr("GAME_CHARACTERS_NOT_ENOUGH_DON"));
    //		return false;
    //       }

    //	emptySlot.Card.SetCardResource(cardResource);
    //	UseDonCard(cardResource.Cost);

    //	EmitSignal(SignalName.CharactersChanged, new Array<CardResource>(GetCharacters()));

    //	return true;
    //}

    //public void RemoveCharacter(CardResource cardResource)
    //{
    //	EmitSignal(SignalName.CharactersChanged, new Array<CardResource>(GetCharacters()));
    //}

    //public void ShuffleDeck()
    //{
    //	Log.Information("Shuffle Deck");
    //	_deck = _deck.OrderBy(a => System.Guid.NewGuid()).ToList();
    //}

    //public void DrawDonCard(int amount = 1)
    //   {
    //	if (amount > CardsDonDeck)
    //       {
    //		amount = CardsDonDeck;
    //       } else if (amount < 0)
    //       {
    //		amount = 1;
    //       }

    //	if (amount != 0)
    //	{
    //		Log.Debug("Draw {Amount} from the don deck.", amount);
    //		CardsDonDeck -= amount;
    //		CardsCostDeck += amount;
    //	} else
    //       {
    //		Log.Debug($"Don't draw don card because don deck is empty.");
    //       }
    //   }

    //public bool UseDonCard(int amount = 1)
    //{
    //	if (amount < 0)
    //	{
    //		amount = 1;
    //	}

    //	if (CardsCostDeck >= amount)
    //	{
    //		Log.Debug("Use {Amount} card(s) from the cost area.", amount);
    //		CardsCostDeck -= amount;
    //		CardsRestedCostDeck += amount;

    //		return true;
    //	} else
    //       {
    //		Log.Debug($"Can't use don card because cost deck has not enough don cards.");
    //		ChangeMessage(string.Format(Tr("GAME_NOT_ENOUGH_DON"), amount));
    //       }

    //	return false;
    //}

    //public void UnrestCostDeck()
    //   {
    //	CardsCostDeck += CardsRestedCostDeck;
    //	CardsRestedCostDeck = 0;
    //}

    //public List<CardResource> DrawCard(int amount = 1)
    //{
    //	Log.Information("Draw {Amount} cards", amount);
    //	var cards = RemoveDeckCards(amount);
    //	cards.ForEach(x =>
    //	{
    //		if (x != null)
    //		{
    //			Log.Information($"Card drawn, add it to the hand.");
    //			_ = PlayerArea.Hand.AddCard(x);
    //		}
    //		else
    //		{
    //			Log.Warning($"Card drawn but null (game finished ?)");
    //		}

    //		EmitSignal(SignalName.CardDrawn, x);
    //	});

    //	return cards;
    //}

    //public void TrashCard(CardResource cardResource)
    //   {
    //	_trash.Add(cardResource);
    //	TrashSlotCard.Card.Show();
    //   }

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

    public void OnCardAction(SlotCard slotCard, GameSlotCardActionResource resource, int id)
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
                    break;
                case CardAction.Summon:
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

    //   private void SummonCards(SlotCard slotCard, GameSlotCardActionResource resource)
    //{
    //	Log.Debug("Executing summon method for source {Source}", resource.Source);

    //	if (AddCharacter(slotCard.Card.CardResource))
    //	{
    //		Log.Information("Card '{Name}' summoned", slotCard.Card.CardResource.Name);
    //		slotCard.QueueFree();
    //       } else
    //	{
    //		Log.Warning("Unable to add the card '{Name}' (area is full ?)", slotCard.Card.CardResource.Name);
    //	}
    //   }

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
}
