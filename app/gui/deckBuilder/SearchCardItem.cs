using Godot;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class SearchCardItem : HBoxContainer
{
    public Card Card { get; protected set; }
    public VBoxContainer TextContainer { get; protected set; }
    public Label CardNameLabel { get; protected set; }
    public Label CardInfoLabel { get; protected set; }

    [Signal]
    public delegate void ClickCard(Card card);

    public override void _Ready()
    {
        Card = GetNode<Card>("Card");
        TextContainer = GetNode<VBoxContainer>("TextContainer");
        CardNameLabel = GetNode<Label>("TextContainer/CardName");
        CardInfoLabel = GetNode<Label>("TextContainer/CardInfo");

        this.ConnectIfMissing("mouse_entered", this, nameof(MouseEntered));
    }

    public void MouseEntered()
    {
        Card.CheckAndDownload();
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
