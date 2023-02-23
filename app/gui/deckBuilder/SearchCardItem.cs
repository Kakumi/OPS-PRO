using Godot;

public class SearchCardItem : PanelContainer
{
    public Card Card { get; protected set; }
    public VBoxContainer TextContainer { get; protected set; }
    public Label CardNameLabel { get; protected set; }
    public Label CardInfoLabel { get; protected set; }

    [Signal]
    public delegate void ClickCard(Card card);

    public override void _Ready()
    {
        Card = GetNode<Card>("HBoxContainer/Card");
        TextContainer = GetNode<VBoxContainer>("HBoxContainer/TextContainer");
        CardNameLabel = GetNode<Label>("HBoxContainer/TextContainer/CardName");
        CardInfoLabel = GetNode<Label>("HBoxContainer/TextContainer/CardInfo");

        this.ConnectIfMissing("mouse_entered", this, nameof(MouseEntered));
        this.ConnectIfMissing("mouse_exited", this, nameof(MouseExited));
    }

    public void MouseEntered()
    {
        Card.CheckAndDownload();

        var stylebox = new StyleBoxFlat();
        stylebox.BgColor = new Color().FromRGBA(0, 0, 0, 0.7f);
        Set("custom_styles/panel", stylebox);
    }

    private void MouseExited()
    {
        var stylebox = new StyleBoxFlat();
        stylebox.BgColor = new Color().FromRGBA(0, 0, 0, 0f);
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
            if (inputButton.ButtonIndex == (int) ButtonList.Left && !inputButton.Pressed)
            {
                EmitSignal(nameof(ClickCard), Card);
            }
        }
    }

    public void OnTextContainerResized()
    {
        if (Card != null && TextContainer != null)
        {
            Card.RectMinSize = new Vector2(Card.RectMinSize.x, TextContainer.RectSize.y);
        }
    }
}
