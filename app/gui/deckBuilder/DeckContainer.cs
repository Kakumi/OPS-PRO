using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class DeckContainer : VBoxContainer
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
    public PanelContainer DeckPanelContainer { get; protected set; }
    public GridContainer Deck { get; protected set; }
    public Label CardsNumber { get; protected set; }
    public Label CardsTypes { get; protected set; }

    private List<DeckResource> _decks;
    public List<DeckResource> Decks
    {
        get => _decks ?? (_decks = new List<DeckResource>());
        set
        {
            _decks = value;
            UpdateDecksOptions();
        }
    }

    [Signal]
    public delegate void MouseEnterCardEventHandler(Card card);

    [Signal]
    public delegate void MouseExitCardEventHandler(Card card);

    public override void _Ready()
    {
        DecksOptions = GetNode<OptionButton>("PanelContainer/DeckManagerContainer/VBoxContainer2/HBoxContainer3/DecksOptions");
        DeckName = GetNode<LineEdit>("PanelContainer/DeckManagerContainer/VBoxContainer2/HBoxContainer3/DeckName");
        DeckStatus = GetNode<RichTextLabel>("PanelContainer/DeckManagerContainer/MarginContainer/VBoxContainer/DeckStatus");
        InfoMessage = GetNode<RichTextLabel>("PanelContainer/DeckManagerContainer/MarginContainer/VBoxContainer/InfoMessage");
        CardsNumber = GetNode<Label>("DeckContainer/PanelContainer/MarginContainer/DeckInfo/CardsNumber");
        CardsTypes = GetNode<Label>("DeckContainer/PanelContainer/MarginContainer/DeckInfo/CardsTypes");
        DeckPanelContainer = GetNode<PanelContainer>("DeckContainer/DeckPanelContainer");
        Deck = DeckPanelContainer.GetNode<GridContainer>("Deck");

        Decks = DeckManager.Instance.LoadDecks();

        var searchNode = GetNode(SearchContainerPath);
        if (searchNode is SearchContainer)
        {
            SearchContainer = searchNode as SearchContainer;
            SearchContainer.ClickCard += CardClicked;
        }
    }

    public void CardClicked(Card card)
    {
        AddCard(card.CardResource);
    }

    private void UpdateDecksOptions()
    {
        DecksOptions.Clear();
        DecksOptions.Text = Tr("DECKBUILDER_DECKS");
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

    public void SelectDeck(DeckResource deck)
    {
        var foundId = -1;
        for(int i = 0; i < DecksOptions.ItemCount; i++)
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

    private DeckResource GetSelectedDeck()
    {
       return Decks.FirstOrDefault(x => DecksOptions.GetItemText(DecksOptions.Selected) == x.Name);
    }

    public void OnSavePressed()
    {
        try
        {
            InfoMessage.Text = null;

            var deck = GetSelectedDeck();
            if (deck != null)
            {
                DeckManager.Instance.Save(deck, DeckName.Text);
                DecksOptions.SetItemText(DecksOptions.Selected, deck.Name);
                ChangeInfoMessage(string.Format(Tr("DECKBUILDER_SAVED", deck.Name)), "green");
            }
            else
            {
                ChangeInfoMessage(Tr("DECKBUILDER_SAVED_FAILED_SELECTED"), "red");
            }
        } catch(Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_SAVED_FAILED_EX"), ex.Message), "red");
        }
    }

    public void OnCreatePressed()
    {
        try
        {
            InfoMessage.Text = null;

            var deck = DeckManager.Instance.Create(DeckName.Text);
            StoreNewDeck(deck);

            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_CREATED"), deck.Name), "green");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_CREATED_FAILED_EX"), ex.Message), "red");
        }
    }

    public void OnDeletePressed()
    {
        try
        {
            InfoMessage.Text = null;

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
                    DecksOptions.Text = Tr("DECKBUILDER_DECKS");
                    DeckName.Text = string.Empty;
                }

                ChangeInfoMessage(Tr("DECKBUILDER_DELETED"), "green");
            }
            else
            {
                ChangeInfoMessage(Tr("DECKBUILDER_DELETE_FAILED_SELECTED"), "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_DELETED_FAILED_EX"), ex.Message), "red");
        }
    }

    public void OnClearPressed()
    {
        try
        {
            InfoMessage.Text = null;

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
            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_CLEARED_FAILED_EX"), ex.Message), "red");
        }
    }

    public void OnDuplicatePressed()
    {
        try
        {
            InfoMessage.Text = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                var newDeck = DeckManager.Instance.Duplicate(selectedDeck);
                StoreNewDeck(newDeck);
                ChangeInfoMessage(string.Format(Tr("DECKBUILDER_DUPLICATED"), newDeck.Name), "green");
            } else
            {
                ChangeInfoMessage(Tr("DECKBUILDER_DUPLICATED_FAILED_SELECTED"), "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_DUPLICATED_FAILED_EX"), ex.Message), "red");
        }
    }

    private void StoreNewDeck(DeckResource deck)
    {
        Decks.Add(deck);
        UpdateDecksOptions();
        SelectDeck(deck);
    }

    private void ChangeInfoMessage(string message, string color = "white")
    {
        InfoMessage.Text = $"[color={color}]{message}[/color]";
    }

    private void UpdateStatus(DeckResource deck)
    {
        DeckStatus.Text = string.Format(Tr("DECKBUILDER_STATUS"), deck.IsValid() ? $"[color=green]{Tr("DECKBUILDER_STATUS_VALID")}[/color]" : $"[color=red]{Tr("DECKBUILDER_STATUS_INVALID")}[/color]");

        CardsNumber.Text = $"Cards: {deck.NumberOfCards}";

        var sBuilderTypes = new StringBuilder();
        sBuilderTypes.Append($"{Tr("DECKBUILDER_LEADER")}{deck.NumberOfCardsTypes(CardTypeList.LEADER)}").Append(" | ");
        sBuilderTypes.Append($"{Tr("DECKBUILDER_CHARACTERS")}{deck.NumberOfCardsTypes(CardTypeList.CHARACTER)}").Append(" | ");
        sBuilderTypes.Append($"{Tr("DECKBUILDER_EVENT")}{deck.NumberOfCardsTypes(CardTypeList.STAGE)}").Append(" | ");
        sBuilderTypes.Append($"{Tr("DECKBUILDER_STAGE")}{deck.NumberOfCardsTypes(CardTypeList.EVENT)}");
        CardsTypes.Text = sBuilderTypes.ToString();
    }

    private void UpdateDeckView(DeckResource deck)
    {
        DeckName.Text = deck.Name;

        Deck.GetChildren().ToList().ForEach(x => x.QueueFree());
        deck.Cards.ToList().ForEach(x =>
        {
            for(int i = 0; i < x.Value; i++)
            {
                AddCardDeckView(x.Key);
            }
        });

        UpdateStatus(deck);
    }

    private void AddCardDeckView(CardResource cardResource)
    {
        var card = CardPackedScene.Instantiate<Card>();
        Deck.AddChild(card);
        card.SetCardResource(cardResource, true);
        card.LeftClickCard += (c) => DeckCardLeftClicked(cardResource, card);
        card.RightClickCard += (c) => DeckCardRightClicked(cardResource, card);
        card.MiddleClickCard += (c) => DeckCardMiddleClicked(cardResource, card);
        card.MouseEntered += () => CardMouseEntered(card);
        card.MouseExited += () => CardMouseExited(card);
    }

    public void AddCard(CardResource cardResource)
    {
        try
        {
            InfoMessage.Text = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                Log.Information($"Add 1 card {cardResource.Id} to deck {selectedDeck.Name}.");

                var error = CanCardAdded(selectedDeck, cardResource);
                if (error == ErrorAddCard.Ok)
                {
                    selectedDeck.AddCard(cardResource, 1);
                    AddCardDeckView(cardResource);
                    UpdateStatus(selectedDeck);
                } else
                {
                    Log.Warning($"Cart cannot be added because of error {error}");
                    switch (error)
                    {
                        case ErrorAddCard.NullObject:
                            ChangeInfoMessage(Tr("DECKBUILDER_ADD_ERROR_NULL"), "red");
                            break;
                        case ErrorAddCard.LeaderAlreadyExist:
                            ChangeInfoMessage(Tr("DECKBUILDER_ADD_ERROR_LEADER_EXIST"), "red");
                            break;
                        case ErrorAddCard.DeckFull:
                            ChangeInfoMessage(Tr("DECKBUILDER_ADD_ERROR_DECK_FULL"), "red");
                            break;
                        case ErrorAddCard.CardMaxAmount:
                            ChangeInfoMessage(Tr("DECKBUILDER_ADD_ERROR_MAX_CARD"), "red");
                            break;
                        default:
                            ChangeInfoMessage(string.Format(Tr("DECKBUILDER_ADD_ERROR_UNKNOWN"), error), "red");
                            break;
                    }
                }
            }
            else
            {
                ChangeInfoMessage(Tr("DECKBUILDER_ADD_FAILED_SELECTED"), "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("﻿DECKBUILDER_ADD_FAILED_EX"), ex.Message), "red");
        }
    }

    private void DeckCardLeftClicked(CardResource cardResource, Card card)
    {
        try
        {
            InfoMessage.Text = null;

            var selectedDeck = GetSelectedDeck();
            if (selectedDeck != null)
            {
                Log.Information($"Remove 1 card {cardResource.Id} from deck {selectedDeck.Name}.");
                selectedDeck.RemoveCard(cardResource);
                UpdateStatus(selectedDeck);
                card.QueueFree();
            }
            else
            {
                ChangeInfoMessage(Tr("DECKBUILDER_REMOVE_FAILED_SELECTED"), "red");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("﻿DECKBUILDER_REMOVE_FAILED_EX"), ex.Message), "red");
        }
    }

    private void DeckCardMiddleClicked(CardResource cardResource, Card card)
    {

    }

    private void DeckCardRightClicked(CardResource cardResource, Card card)
    {
        AddCard(cardResource);
    }

    private void CardMouseEntered(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    private void CardMouseExited(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

    protected ErrorAddCard CanCardAdded(DeckResource deck, CardResource cardResource)
    {
        if (deck == null || cardResource == null)
        {
            return ErrorAddCard.NullObject;
        }

        if (cardResource.CardTypeList == CardTypeList.LEADER)
        {
            if (deck.Cards.Count(x => x.Key.CardTypeList == cardResource.CardTypeList) != 0)
            {
                return ErrorAddCard.LeaderAlreadyExist;
            }

            return ErrorAddCard.Ok;
        }

        if (deck.NumberOfCardsTypes(CardTypeList.STAGE, CardTypeList.EVENT, CardTypeList.CHARACTER) >= 50)
        {
            return ErrorAddCard.DeckFull;
        }

        if (deck.Cards.ContainsKey(cardResource) && deck.Cards[cardResource] >= 4)
        {
            return ErrorAddCard.CardMaxAmount;
        }

        return ErrorAddCard.Ok;
    }

    public void OnQuitPressed()
    {
        try
        {
            var parent = this.SearchParent<DeckBuilder>();
            if (parent == null)
            {
                Log.Error("DeckBuilder not found, can't close pane.");
                ChangeInfoMessage(Tr($"DECKBUILDER_QUIT_FAILED_PARENT"), "red");
            } else
            {
                parent?.QueueFree();
                AppInstance.Instance.ShowMainMenu();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    public void OnHandTesterPressed()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
    }

    public void OnExportDeckPressed()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            ChangeInfoMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), "red");
        }
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
