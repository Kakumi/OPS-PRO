using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public class DeckContainer : VBoxContainer
{
    [Export]
    public NodePath SearchContainerPath { get; set; }

    [Export]
    public PackedScene CardPackedScene { get; set; }

    public OptionButton DecksOptions { get; private set; }
    public LineEdit DeckName { get; private set; }
    public RichTextLabel DeckStatus { get; private set; }
    public RichTextLabel InfoMessage { get; private set; }
    public SearchContainer SearchContainer { get; set; }
    public GridContainer Deck { get; protected set; }

    private List<Deck> _decks;
    public List<Deck> Decks
    {
        get => _decks ?? (_decks = new List<Deck>());
        set
        {
            _decks = value;
            UpdateDecksOptions();
        }
    }

    [Signal]
    public delegate void MouseEnterCard(Card card);

    [Signal]
    public delegate void MouseExitCard(Card card);

    public override void _Ready()
    {
        DecksOptions = GetNode<OptionButton>("PanelContainer/DeckManagerContainer/VBoxContainer2/HBoxContainer3/DecksOptions");
        DeckName = GetNode<LineEdit>("PanelContainer/DeckManagerContainer/VBoxContainer2/HBoxContainer3/DeckName");
        DeckStatus = GetNode<RichTextLabel>("PanelContainer/DeckManagerContainer/MarginContainer/VBoxContainer/DeckStatus");
        InfoMessage = GetNode<RichTextLabel>("PanelContainer/DeckManagerContainer/MarginContainer/VBoxContainer/InfoMessage");
        Deck = GetNode<GridContainer>("DeckContainer/PanelContainer2/Deck");

        Decks = DeckManager.Instance.LoadDecks();

        var searchNode = GetNode(SearchContainerPath);
        if (searchNode is SearchContainer)
        {
            SearchContainer = searchNode as SearchContainer;
            SearchContainer.ConnectIfMissing(nameof(SearchContainer.ClickCard), this, nameof(CardClicked));
        }
    }

    public void CardClicked(Card card)
    {
        AddCard(card.CardInfo);
    }

    private void UpdateDecksOptions()
    {
        DecksOptions.Clear();
        DecksOptions.Text = "Decks";
        Decks.ForEach(d => DecksOptions.AddItem(d.Name));

        if (Decks.Count > 0)
        {
            UpdateDeckView(Decks.First());
        }
    }

    public void OnDecksOptionsItemSelected(int idx)
    {
        var deck = Decks.FirstOrDefault(x => DecksOptions.GetItemText(idx) == x.Name);
        if (deck != null)
        {
            UpdateDeckView(deck);
        }
    }

    public void SelectDeck(Deck deck)
    {
        var foundId = -1;
        for(int i = 0; i < DecksOptions.GetItemCount(); i++)
        {
            var deckOption = DecksOptions.GetItemText(i);
            if (deck.Name == deckOption)
            {
                foundId = i;
                break;
            }
        }

        if (foundId != -1)
        {
            DecksOptions.Select(foundId);
            UpdateDeckView(deck);
        }
    }

    private Deck GetSelectedDeck()
    {
       return Decks.FirstOrDefault(x => DecksOptions.GetItemText(DecksOptions.Selected) == x.Name);
    }

    public void OnSavePressed()
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var deck = GetSelectedDeck();
            if (deck != null)
            {
                DeckManager.Instance.Save(deck, DeckName.Text);
                DecksOptions.SetItemText(DecksOptions.Selected, deck.Name);
                ChangeInfoMessage($"Deck {deck.Name} has been saved.", "green");
            }
            else
            {
                ChangeInfoMessage($"No deck selected, unable to save. Please select one from the list below.", "red");
            }
        } catch(Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to save this deck because {ex.Message}.", "red");
        }
    }

    public void OnCreatePressed()
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var deck = DeckManager.Instance.Create(DeckName.Text);
            StoreNewDeck(deck);

            ChangeInfoMessage($"Deck {deck.Name} has been created.", "green");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to create this deck because {ex.Message}.", "red");
        }
    }

    public void OnDeletePressed()
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var deck = GetSelectedDeck();
            if (deck != null)
            {
                DeckManager.Instance.Delete(deck);
                Decks.Remove(deck);
                DecksOptions.RemoveItem(DecksOptions.Selected);
                if (Decks.Count > 0)
                {
                    SelectDeck(Decks.First());
                } else
                {
                    DecksOptions.Text = "Decks";
                    DeckName.Text = string.Empty;
                }

                ChangeInfoMessage($"Deck {deck.Name} has been deleted.", "green");
            }
            else
            {
                ChangeInfoMessage($"No deck selected, unable to delete. Please select one from the list below.", "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to delete this deck because {ex.Message}.", "red");
        }
    }

    public void OnClearPressed()
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var deck = GetSelectedDeck();
            if (deck != null)
            {
                DeckManager.Instance.Clear(deck);
                UpdateDeckView(deck);
                ChangeInfoMessage($"Deck {deck.Name} has been clear but not saved.", "green");
            }
            else
            {
                ChangeInfoMessage($"No deck selected, unable to clear. Please select one from the list below.", "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to clear this deck because {ex.Message}.", "red");
        }
    }

    public void OnDuplicatePressed()
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                var newDeck = DeckManager.Instance.Duplicate(selectedDeck);
                StoreNewDeck(newDeck);
                ChangeInfoMessage($"Deck {newDeck.Name} has been created.", "green");
            } else
            {
                ChangeInfoMessage($"No deck selected, unable to duplicate. Please select one from the list below.", "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to duplicate this deck because {ex.Message}.", "red");
        }
    }

    private void StoreNewDeck(Deck deck)
    {
        Decks.Add(deck);
        UpdateDecksOptions();
        SelectDeck(deck);
    }

    private void ChangeInfoMessage(string message, string color = "white")
    {
        InfoMessage.BbcodeText = $"[color={color}]{message}[/color]";
    }

    private void UpdateStatus()
    {

    }

    private void UpdateDeckView(Deck deck)
    {
        DeckName.Text = deck.Name;

        Deck.GetChildren().QueueFreeAll();
        deck.Cards.ToList().ForEach(x =>
        {
            for(int i = 0; i < x.Value; i++)
            {
                AddCardDeckView(x.Key);
            }
        });
    }

    private void AddCardDeckView(CardInfo cardInfo)
    {
        var card = CardPackedScene.Instance<Card>();
        Deck.AddChild(card);
        card.SetCardInfo(cardInfo, true);
        card.ConnectIfMissing(nameof(Card.ClickCard), this, nameof(DeckCardClicked), new Godot.Collections.Array(card));
        card.ConnectIfMissing("mouse_entered", this, nameof(CardMouseEntered), new Godot.Collections.Array(card));
        card.ConnectIfMissing("mouse_exited", this, nameof(CardMouseExited), new Godot.Collections.Array(card));
    }

    public void AddCard(CardInfo cardInfo)
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                Log.Information($"Add 1 card {cardInfo.Id} to deck {selectedDeck.Name}.");

                var error = CanCardAdded(selectedDeck, cardInfo);
                if (error == ErrorAddCard.Ok)
                {
                    selectedDeck.AddCard(cardInfo, 1);
                    AddCardDeckView(cardInfo);
                } else
                {
                    Log.Warning($"Cart cannot be added because of error {error}");
                    switch (error)
                    {
                        case ErrorAddCard.NullObject:
                            ChangeInfoMessage($"An error occurred, objets are null.", "red");
                            break;
                        case ErrorAddCard.LeaderAlreadyExist:
                            ChangeInfoMessage($"This card cannot be added because a leader already exists.", "red");
                            break;
                        case ErrorAddCard.DeckFull:
                            ChangeInfoMessage($"This card cannot be added because the deck is full.", "red");
                            break;
                        case ErrorAddCard.CardMaxAmount:
                            ChangeInfoMessage($"This card cannot be added because the max amount for this card is reached.", "red");
                            break;
                        default:
                            ChangeInfoMessage($"An error occurred, error {error} not supported", "red");
                            break;
                    }
                }
            }
            else
            {
                ChangeInfoMessage($"No deck selected, unable to add this card. Please select one from the list below.", "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to add this card because {ex.Message}.", "red");
        }
    }

    private void DeckCardClicked(CardInfo cardInfo, Card card)
    {
        try
        {
            InfoMessage.BbcodeText = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                Log.Information($"Remove 1 card {cardInfo.Id} from deck {selectedDeck.Name}.");
                selectedDeck.RemoveCard(cardInfo);
                card.QueueFree();
            }
            else
            {
                ChangeInfoMessage($"No deck selected, unable to remove this card. Please select one from the list below.", "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage($"An error occured, unable to remove this card because {ex.Message}.", "red");
        }
    }

    private void CardMouseEntered(Card card)
    {
        EmitSignal(nameof(MouseEnterCard), card);
    }

    private void CardMouseExited(Card card)
    {
        EmitSignal(nameof(MouseExitCard), card);
    }

    protected ErrorAddCard CanCardAdded(Deck deck, CardInfo cardInfo)
    {
        if (deck == null || cardInfo == null)
        {
            return ErrorAddCard.NullObject;
        }

        if (cardInfo.CardTypeList == CardTypeList.LEADER)
        {
            if (deck.Cards.Count(x => x.Key.CardTypeList == cardInfo.CardTypeList) != 0)
            {
                return ErrorAddCard.LeaderAlreadyExist;
            }

            return ErrorAddCard.Ok;
        }

        if (deck.NumberOfCardsTypes(CardTypeList.STAGE, CardTypeList.EVENT, CardTypeList.CHARACTER) >= 50)
        {
            return ErrorAddCard.DeckFull;
        }

        if (deck.Cards.ContainsKey(cardInfo) && deck.Cards[cardInfo] >= 4)
        {
            return ErrorAddCard.CardMaxAmount;
        }

        return ErrorAddCard.Ok;
    }

    protected enum ErrorAddCard
    {
        Ok,
        NullObject,
        LeaderAlreadyExist,
        DeckFull,
        CardMaxAmount
    }
}
