using Godot;
using System;

public partial class Card : TextureRect
{
    public CardResource CardResource { get; protected set; }

    [Signal]
    public delegate void LeftClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void RightClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void MiddleClickCardEventHandler(CardResource cardResource);

    public override void _Ready()
    {

    }

    public virtual void CheckAndDownload()
    {
        if (CardResource != null && !CardResource.TextureSet)
        {
            CardResource.StartDownloading();
        }
    }

    public void SetCardResource(CardResource cardResource, bool download = false)
    {
        if (cardResource != null && CardResource?.Id != cardResource?.Id)
        {
            if (CardResource != null)
            {
                CardResource.FrontTextureChanged -= FrontTextureChanged;
            }

            CardResource = cardResource;
            CardResource.FrontTextureChanged += FrontTextureChanged;
            Texture = cardResource.FrontTexture;

            if (!cardResource.TextureSet && download)
            {
                CardResource.StartDownloading();
            }
        }
    }

    private void FrontTextureChanged(Texture2D texture2D)
    {
        Texture = texture2D;
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
