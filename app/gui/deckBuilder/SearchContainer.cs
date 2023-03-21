using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SearchContainer : VBoxContainer
{
    [Export]
    public PackedScene CardInfoScene { get; set; }

    protected VBoxContainer Cards { get; set; }
    protected LineEdit SearchText { get; set; }
    protected SpinBox CostText { get; set; }
    protected SpinBox CounterText { get; set; }
    protected SpinBox PowerText { get; set; }
    protected OptionButton ColorOptions { get; set; }
    protected OptionButton SetOptions { get; set; }
    protected OptionButton TypeOptions { get; set; }
    protected OptionButton CardTypeOptions { get; set; }
    protected Label SearchCardNumberResult { get; set; }

    [Signal]
    public delegate void ClickCardEventHandler(Card card);

    [Signal]
    public delegate void MouseEnterCardEventHandler(Card card);

    [Signal]
    public delegate void MouseExitCardEventHandler(Card card);

    public override void _Ready()
    {
        SearchText = GetNode<LineEdit>("SearchPanelContainer/SearchContainer/Column2/SearchText");
        CostText = GetNode<SpinBox>("SearchPanelContainer/SearchContainer/Column2/CostText");
        CounterText = GetNode<SpinBox>("SearchPanelContainer/SearchContainer/Column2/CounterText");
        PowerText = GetNode<SpinBox>("SearchPanelContainer/SearchContainer/Column2/PowerText");

        ColorOptions = GetNode<OptionButton>("SearchPanelContainer/SearchContainer/Column1/ColorOptions");
        SetOptions = GetNode<OptionButton>("SearchPanelContainer/SearchContainer/Column1/SetOptions");
        TypeOptions = GetNode<OptionButton>("SearchPanelContainer/SearchContainer/Column1/TypeOptions");
        CardTypeOptions = GetNode<OptionButton>("SearchPanelContainer/SearchContainer/Column1/CardTypeOptions");

        SearchCardNumberResult = GetNode<Label>("VBoxContainer/SearchResultPanelContainer/MarginContainer/DeckInfo/SearchCardNumberResult");
        Cards = GetNode<VBoxContainer>("VBoxContainer/PanelContainer/MarginContainer/SearchResultCardsContainer/Cards");

        CostText.Prefix = Tr(CostText.Prefix);
        CounterText.Prefix = Tr(CounterText.Prefix);
        PowerText.Prefix = Tr(PowerText.Prefix);

        CardManager.Instance.GetColors().ForEach(x => ColorOptions.AddItem(x));
        CardManager.Instance.GetSets().ForEach(x => SetOptions.AddItem(x));
        CardManager.Instance.GetTypes().ForEach(x => TypeOptions.AddItem(x));
        CardManager.Instance.GetCardTypes().ForEach(x => CardTypeOptions.AddItem(x));

        OnResetButtonPressed();
    }

    public void OnSearchButtonPressed()
    {
        try
        {
            Log.Debug("Search cards button pressed");

            Cards.GetChildren().ToList().ForEach(x => x.QueueFree());

            var selectedColor = ColorOptions.Selected < 0 ? null : ColorOptions.GetItemText(ColorOptions.Selected);
            var selectedSet = SetOptions.Selected < 0 ? null : SetOptions.GetItemText(SetOptions.Selected);
            var selectedType = TypeOptions.Selected < 0 ? null : TypeOptions.GetItemText(TypeOptions.Selected);
            var seletecCardType = CardTypeOptions.Selected < 0 ? null : CardTypeOptions.GetItemText(CardTypeOptions.Selected);

            var cards = CardManager.Instance.Search(SearchText.Text, CostText.Value, CounterText.Value, PowerText.Value, selectedColor, selectedSet, selectedType, seletecCardType);
            SearchCardNumberResult.Text = string.Format(Tr("SEARCH_RESULTS"), cards.Count);
            cards.ForEach(card =>
            {
                var cardInstance = CardInfoScene.Instantiate<SearchCardItem>();
                Cards.AddChild(cardInstance);
                cardInstance.UpdatecardResource(card);

                cardInstance.ClickCard += CardClicked;
                cardInstance.MouseEntered += () => CardMouseEntered(cardInstance.Card);
                cardInstance.MouseExited += () => CardMouseExited(cardInstance.Card);
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

    public void CardClicked(Card card)
    {
        EmitSignal(SignalName.ClickCard, card);
    }

    public void CardMouseEntered(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    public void CardMouseExited(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

    public void OnResetButtonPressed()
    {
        SearchText.Text = "";
        CostText.Value = 0;
        CounterText.Value = 0;
        PowerText.Value = 0;

        ColorOptions.Selected = -1;
        SetOptions.Selected = -1;
        TypeOptions.Selected = -1;
        CardTypeOptions.Selected = -1;

        ColorOptions.Text = Tr("SEARCH_COLOR");
        SetOptions.Text = Tr("SEARCH_SET");
        TypeOptions.Text = Tr("SEARCH_TYPE");
        CardTypeOptions.Text = Tr("SEARCH_CARDTYPE");

        SearchCardNumberResult.Text = string.Format(Tr("SEARCH_RESULTS"), 0); ;

        Cards.GetChildren().ToList().ForEach(x => x.QueueFree());
    }
}
