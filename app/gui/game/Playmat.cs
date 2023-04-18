using Godot;
using Godot.Collections;
using Serilog;
using System.Collections.Generic;
using System.Linq;

public partial class Playmat : PanelContainer
{
	private List<CardResource> Deck { get; set; }
	private List<CardResource> Trash { get; set; }
	private Stack<CardResource> Lifes { get; set; }
	private List<CardResource> DonDeck { get; set; }
	private List<CardResource> CostArea { get; set; }

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
	public delegate void DeckChangedEventHandler(Array<CardResource> cards);

	[Signal]
	public delegate void TrashChangedEventHandler(Array<CardResource> cards);

	[Signal]
	public delegate void LifeChangedEventHandler(Array<CardResource> cards);

	[Signal]
	public delegate void  CharactersChangedEventHandler(Array<CardResource> cards);

	public override void _Ready()
	{
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

        DeckChanged += Playmat_DeckChanged;
        TrashChanged += Playmat_TrashChanged;
        LifeChanged += Playmat_LifeChanged;
	}

	private void OnResized()
    {
		PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);
	}


	private void Playmat_TrashChanged(Array<CardResource> cards)
	{
		Log.Debug("Trash changed, update card slot");
		DeckSlotCard.Card.Visible = cards.Count > 0;
	}

    private void Playmat_DeckChanged(Array<CardResource> cards)
	{
		Log.Debug("Deck changed, update card slot");
		TrashSlotCard.Card.Visible = cards.Count > 0;
	}

    private void Playmat_LifeChanged(Array<CardResource> cards)
	{
		Log.Debug("Life changed, update card slot");
		LifeLabel.Text = string.Format(Tr("GAME_LIFE_COUNTER"), cards.Count);
		LifeSlotCard.Card.Visible = cards.Count > 0;
	}

    public void Init(DeckResource deckResource)
	{
		Log.Information("Initializing game with deck {Name}", deckResource.Name);
		var cards = deckResource.Cards.ToList();
		var leaderCardResource = cards.First(x => x.Key.CardTypeList == CardTypeList.LEADER);
		LeaderSlotCard.Card.SetCardResource(leaderCardResource.Key);

		Deck = new List<CardResource>();
		DeckSlotCard.Card.Hide();
		Trash = new List<CardResource>();
		TrashSlotCard.Card.Hide();
		Lifes = new Stack<CardResource>();
		LifeSlotCard.Card.Hide();
		DonDeck = new List<CardResource>();
		DonDeckSlotCard.Card.Hide();
		CostArea = new List<CardResource>();
		CostSlotCard.Card.Hide();

		var deckCards = cards.Where(x => x.Key.CardTypeList == CardTypeList.CHARACTER || x.Key.CardTypeList == CardTypeList.STAGE || x.Key.CardTypeList == CardTypeList.EVENT);
		foreach(var deckCard in deckCards)
        {
			for(int i = 0; i < deckCard.Value; i++)
            {
				Deck.Add(deckCard.Key);
			}
        }

		ShuffleDeck();

		DrawCard(5);

		RemoveDeckCards(leaderCardResource.Key.Cost).ForEach(x =>
		{
			AddLifeCard(x);
		});
	}

	public void AddDeckCard(CardResource cardResource)
    {
		Deck.Add(cardResource);
		EmitSignal(SignalName.DeckChanged, new Array<CardResource>(Deck));
	}

	private List<CardResource> RemoveDeckCards(int amount = 1)
	{
		if (Deck.Count >= amount)
		{
			Log.Information("Remove {Amount} cards from deck", amount);
			var cards = Deck.Take(amount).ToList();
			Deck.RemoveRange(0, amount);
			EmitSignal(SignalName.DeckChanged, new Array<CardResource>(Deck));

			return cards;
		}

		EmitSignal(SignalName.GameFinished, false);
		return null;
	}

	public void AddLifeCard(CardResource cardResource)
	{
		Log.Information("Add 1 life card");
		Lifes.Push(cardResource);
		EmitSignal(SignalName.LifeChanged, new Array<CardResource>(Lifes));
	}

	public CardResource RemoveLifeCard()
	{
		Log.Information("Remove 1 life card");
		var cardResource = Lifes.Pop();
		EmitSignal(SignalName.LifeChanged, new Array<CardResource>(Lifes));

		return cardResource;
	}

	private List<CardResource> GetCharacters()
    {
		return CharactersSlots.Where(x => x.Card.CardResource != null).Select(x => x.Card.CardResource).ToList();
	}

	public bool AddCharacter(CardResource cardResource)
	{
		var emptySlot = CharactersSlots.FirstOrDefault(x => x.Card.CardResource == null);
		if (emptySlot == null)
        {
			return false;
        }

		emptySlot.Card.SetCardResource(cardResource);

		EmitSignal(SignalName.CharactersChanged, new Array<CardResource>(GetCharacters()));

		return true;
	}

	public void RemoveCharacter(CardResource cardResource)
	{
		EmitSignal(SignalName.CharactersChanged, new Array<CardResource>(GetCharacters()));
	}

	public void ShuffleDeck()
	{
		Log.Information("Shuffle Deck");
		Deck = Deck.OrderBy(a => System.Guid.NewGuid()).ToList();
	}

	public List<CardResource> DrawCard(int amount = 1)
	{
		Log.Information("Draw {Amount} cards", amount);
		var cards = RemoveDeckCards(amount);
		cards.ForEach(x =>
		{
			EmitSignal(SignalName.CardDrawn, x);
		});

		return cards;
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
