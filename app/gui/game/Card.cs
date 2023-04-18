using Godot;
using System;

public partial class Card : TextureRect
{
    public CardResource CardResource { get; protected set; }
    public bool Rested { get; protected set; }
    public bool Flipped { get; protected set; }

    [Signal]
    public delegate void LeftClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void RightClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void MiddleClickCardEventHandler(CardResource cardResource);

    public override void _Ready()
    {
        Rested = false;
        Flipped = false;
    }

    public void SetCardResource(CardResource cardResource, bool download = false)
    {
        if (cardResource == null)
        {
            Texture = null;
            CardResource = null;
        } else if (CardResource?.Id != cardResource?.Id)
        {
            if (CardResource != null)
            {
                CardResource.FrontTextureChanged -= FrontTextureChanged;
            }

            CardResource = cardResource;
            CardResource.FrontTextureChanged += FrontTextureChanged;
            Texture = cardResource.FrontTexture;
            if (Flipped)
            {
                ToggleFlip();
            }

            if (Rested)
            {
                ToggleRest();
            }

            if (!cardResource.TextureSet && download)
            {
                CardResource.StartDownloading();
            }
        }
    }

    #region Download & Input

    public virtual void CheckAndDownload()
    {
        if (CardResource != null && !CardResource.TextureSet)
        {
            CardResource.StartDownloading();
        }
    }

    private void FrontTextureChanged(Texture2D texture2D)
    {
        if (!Flipped)
        {
            Texture = texture2D;
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

    #endregion

    private void OnResized()
    {
        PivotOffset = new Vector2(Size.X / 2, Size.Y / 2);
    }

    public void ToggleRest()
    {
        if (Rested)
        {
            RotationDegrees = 0;
        } else
        {
            RotationDegrees = 90;
        }

        Rested = !Rested;
    }

    public void ToggleFlip()
    {
        if (Flipped)
        {
            Texture = CardResource.FrontTexture;
        } else
        {
            Texture = CardResource.BackTexture;
        }

        Flipped = !Flipped;
    }
}
