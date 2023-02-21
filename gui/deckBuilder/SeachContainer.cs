using Godot;
using Serilog;

public class SeachContainer : VBoxContainer
{
    protected PackedScene CardInfoScene { get; set; }
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

    public override void _Ready()
    {
        CardInfoScene = GD.Load<PackedScene>("res://gui/deckBuilder/SearchCardItem.tscn");

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

        ColorOptions.AddItem("All");
        SetOptions.AddItem("All");
        TypeOptions.AddItem("All");
        CardTypeOptions.AddItem("All");

        CardManager.Instance.GetColors().ForEach(x => ColorOptions.AddItem(x));
        CardManager.Instance.GetSets().ForEach(x => SetOptions.AddItem(x));
        CardManager.Instance.GetTypes().ForEach(x => TypeOptions.AddItem(x));
        CardManager.Instance.GetCardTypes().ForEach(x => CardTypeOptions.AddItem(x));

        SearchCardNumberResult.Text = "Results: 0";
    }

    public void OnSearchButtonPressed()
    {
        Log.Debug("Search cards button pressed");

        Cards.GetChildren().QueueFreeAll();

        var selectedColor = ColorOptions.Selected <= 0 ? null : ColorOptions.GetItemText(ColorOptions.Selected);
        var selectedSet = SetOptions.Selected <= 0 ? null : SetOptions.GetItemText(SetOptions.Selected);
        var selectedType = TypeOptions.Selected <= 0 ? null : TypeOptions.GetItemText(TypeOptions.Selected);
        var seletecCardType = CardTypeOptions.Selected <= 0 ? null : CardTypeOptions.GetItemText(CardTypeOptions.Selected);

        var cards = CardManager.Instance.Search(SearchText.Text, CostText.Value, CounterText.Value, PowerText.Value, selectedColor, selectedSet, selectedType, seletecCardType);
        SearchCardNumberResult.Text = $"Results: {cards.Count}";
        cards.ForEach(card =>
        {
            var cardInstance = CardInfoScene.Instance<SearchCardItem>();
            cardInstance.CardInfo = card;
            Cards.AddChild(cardInstance);
        });
    }

    public void OnResetButtonPressed()
    {
        SearchText.Text = "";
        CostText.Value = 0;
        CounterText.Value = 0;
        PowerText.Value = 0;

        ColorOptions.Text = "Color";
        ColorOptions.Selected = -1;
        SetOptions.Text = "Set";
        SetOptions.Selected = -1;
        TypeOptions.Text = "Type";
        TypeOptions.Selected = -1;
        CardTypeOptions.Text = "Card Type";
        CardTypeOptions.Selected = -1;

        SearchCardNumberResult.Text = "Results: 0";

        Cards.GetChildren().QueueFreeAll();
    }
}
