using Godot;
using System;
using System.Threading.Tasks;

public partial class Card : TextureRect
{
    public CardResource CardResource { get; protected set; }

    [Signal]
    public delegate void LeftClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void RightClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void MiddleClickCardEventHandler(CardResource cardResource);

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
        if (CardResource != null && !CardManager.Instance.TextureExists(CardResource))
        {
            NotifierManager.Instance.Send("get_card_texture", CardResource, true);
        }
    }

    private void CardTextureReceived(object[] obj)
    {
        if (obj.Length > 1 && obj[0] is CardResource && obj[1] is Texture2D)
        {
            var cardResource = (CardResource)obj[0];
            var texture = (Texture2D)obj[1];

            if (cardResource.Id == cardResource.Id)
            {
                Texture = texture;
                _textureSet = true;
            }
        }
    }

    public void SetcardResource(CardResource cardResource, bool download = false)
    {
        if (cardResource != null && CardResource?.Id != cardResource?.Id)
        {
            CardResource = cardResource;
            _textureSet = false;

            Texture = CardManager.Instance.GetBackTexture(cardResource);
        }

        if (download && !CardManager.Instance.TextureExists(cardResource))
        {
            _textureSet = false;
        }

        if (!_textureSet)
        {
            NotifierManager.Instance.Send("get_card_texture", cardResource, download);
        }
    }

    public void OnGuiInput(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseButton)
        {
            InputEventMouseButton inputButton = inputEvent as InputEventMouseButton;
            if (!inputButton.Pressed)
            {
                if (inputButton.ButtonIndex == MouseButton.Left)
                {
                    EmitSignal(SignalName.LeftClickCard, CardResource);
                } else if (inputButton.ButtonIndex == MouseButton.Right)
                {
                    EmitSignal(SignalName.RightClickCard, CardResource);
                } else if (inputButton.ButtonIndex == MouseButton.Middle)
                {
                    EmitSignal(SignalName.MiddleClickCard, CardResource);
                }
            }
        }
    }
}
