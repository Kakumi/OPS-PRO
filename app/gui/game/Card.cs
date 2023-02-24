using Godot;
using System;
using System.Threading.Tasks;

public class Card : TextureRect
{
    public CardInfo CardInfo { get; protected set; }

    [Signal]
    public delegate void LeftClickCard(CardInfo cardInfo);

    [Signal]
    public delegate void RightClickCard(CardInfo cardInfo);

    [Signal]
    public delegate void MiddleClickCard(CardInfo cardInfo);

    private bool _textureSet = false;

    public override void _Ready()
    {

    }

    public override void _EnterTree()
    {
        base._EnterTree();
        NotifierManager.Instance.Listen("receive_card_texture", CardTextureReceived);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        NotifierManager.Instance.StopListener("receive_card_texture", CardTextureReceived);
    }

    public virtual void CheckAndDownload()
    {
        if (CardInfo != null && !CardManager.Instance.TextureExists(CardInfo))
        {
            NotifierManager.Instance.Send("get_card_texture", CardInfo, true);
        }
    }

    private void CardTextureReceived(object[] obj)
    {
        if (obj.Length > 1 && obj[0] is CardInfo && obj[1] is Texture)
        {
            var cardInfo = (CardInfo)obj[0];
            var texture = (Texture)obj[1];

            if (cardInfo.Id == CardInfo.Id)
            {
                Texture = texture;
                _textureSet = true;
            }
        }
    }

    public void SetCardInfo(CardInfo cardInfo, bool download = false)
    {
        if (cardInfo != null && cardInfo?.Id != CardInfo?.Id)
        {
            CardInfo = cardInfo;
            _textureSet = false;

            Texture = CardManager.Instance.GetBackTexture(cardInfo);
        }

        if (download && !CardManager.Instance.TextureExists(cardInfo))
        {
            _textureSet = false;
        }

        if (!_textureSet)
        {
            NotifierManager.Instance.Send("get_card_texture", cardInfo, download);
        }
    }

    public void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton inputButton = inputEvent as InputEventMouseButton;
            if (!inputButton.Pressed)
            {
                if (inputButton.ButtonIndex == (int)ButtonList.Left)
                {
                    EmitSignal(nameof(LeftClickCard), CardInfo);
                } else if (inputButton.ButtonIndex == (int)ButtonList.Right)
                {
                    EmitSignal(nameof(RightClickCard), CardInfo);
                } else if (inputButton.ButtonIndex == (int)ButtonList.Middle)
                {
                    EmitSignal(nameof(MiddleClickCard), CardInfo);
                }
            }
        }
    }
}
