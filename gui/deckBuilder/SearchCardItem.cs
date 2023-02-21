using Godot;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class SearchCardItem : HBoxContainer
{
    protected TextureRect Image { get; set; }
    protected Label CardNameLabel { get; set; }
    protected Label CardInfoLabel { get; set; }
    public CardInfo CardInfo { get; set; }

    public override void _Ready()
    {
        Image = GetNode<TextureRect>("TextureRect");
        CardNameLabel = GetNode<Label>("VBoxContainer/CardName");
        CardInfoLabel = GetNode<Label>("VBoxContainer/CardInfo");

        UpdateCardInfo(CardInfo);

        this.ConnectIfMissing("mouse_entered", this, nameof(MouseEntered));

        CardInfoPanel cardInfoPanel = GetTree().CurrentScene.GetChildren().SearchOne<CardInfoPanel>();
        if (cardInfoPanel != null)
        {
            this.ConnectIfMissing("mouse_entered", cardInfoPanel, nameof(CardInfoPanel.ShowCardInfo), new Godot.Collections.Array(CardInfo));
        }
    }

    public void MouseEntered()
    {
        UpdateCardInfo(CardInfo, true);
    }

    public void UpdateCardInfo(CardInfo cardInfo, bool download = false)
    {
        CardInfo = cardInfo;

        if (download)
        {
            Task.Run(async () =>
            {
                Image.Texture = await CardManager.Instance.DownloadAndGetTexture(cardInfo);
            });
        } else
        {
            Image.Texture = CardManager.Instance.GetTexture(cardInfo);
        }

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
                GD.Print("click !");
            }
        }
    }
}
