using Godot;
using System;

public partial class SearchCardItem : PanelContainer
{
    public Card Card { get; protected set; }
    public VBoxContainer TextContainer { get; protected set; }
    public Label CardNameLabel { get; protected set; }
    public Label CardInfoLabel { get; protected set; }

    [Signal]
    public delegate void ClickCardEventHandler(Card card);

    public override void _Ready()
    {
        Card = GetNode<Card>("HBoxContainer/Card");
        TextContainer = GetNode<VBoxContainer>("HBoxContainer/TextContainer");
        CardNameLabel = GetNode<Label>("HBoxContainer/TextContainer/CardName");
        CardInfoLabel = GetNode<Label>("HBoxContainer/TextContainer/CardInfo");

        MouseEntered += MouseEnteredCard;
        MouseEntered += MouseExitCard;
    }

    public void MouseEnteredCard()
    {
        Card.CheckAndDownload();

        var stylebox = new StyleBoxFlat();
        stylebox.BgColor = new Color(0, 0, 0, 0.7f);
        Set("custom_styles/panel", stylebox);
    }

    private void MouseExitCard()
    {
        var stylebox = new StyleBoxFlat();
        stylebox.BgColor = new Color(0, 0, 0, 0f);
        Set("custom_styles/panel", stylebox);
    }

    public void UpdateCardInfo(CardInfo cardInfo, bool download = false)
    {
        Card.SetCardInfo(cardInfo, download);
        CardNameLabel.Text = cardInfo.Name;
        CardInfoLabel.Text = cardInfo.Set;
    }

    public void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton inputButton = inputEvent as InputEventMouseButton;
            if (inputButton.ButtonIndex == MouseButton.Left && !inputButton.Pressed)
            {
                EmitSignal(SignalName.ClickCard, Card);
            }
        }
    }

    public void OnTextContainerResized()
    {
        if (Card != null && TextContainer != null)
        {
            Card.CustomMinimumSize = new Vector2(Card.CustomMinimumSize.X, TextContainer.Size.Y);
        }
    }
}
