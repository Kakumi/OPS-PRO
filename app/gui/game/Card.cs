using Godot;
using OPSProServer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Card : TextureRect
{
    public CardResource CardResource { get; protected set; }
    public PlayingCard PlayingCard { get; protected set; }

    [Signal]
    public delegate void LeftClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void RightClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void MiddleClickCardEventHandler(CardResource cardResource);

    [Signal]
    public delegate void CardResourceUpdatedEventHandler(CardResource cardResource);

    public override void _Ready()
    {
        base._Ready();
    }

    public void UpdateCard(PlayingCard playingCard)
    {
        if (playingCard == null)
        {
            SetCardResource(null);
        } else
        {
            var cardResource = playingCard.GetCardResource();
            if (cardResource != null)
            {
                PlayingCard = playingCard;
                SetCardResource(cardResource, true);
            }
        }
    }

    public void SetCardResource(CardResource cardResource, bool download = false)
    {
        if (cardResource == null)
        {
            Texture = null;
            CardResource = null;
            PlayingCard = null;
        } else
        {
            if (CardResource?.Id != cardResource?.Id)
            {
                if (PlayingCard == null)
                {
                    PlayingCard = new PlayingCard(cardResource.Generate());
                }

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

            UpdateFlip();
            UpdateRest();
        }

        EmitSignal(SignalName.CardResourceUpdated, cardResource);
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
        if (PlayingCard != null && !PlayingCard.Flipped)
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

    public void UpdateRest()
    {
        if (PlayingCard.Rested)
        {
            RotationDegrees = 90;
        } else
        {
            RotationDegrees = 0;
        }
    }

    public void UpdateFlip()
    {
        if (PlayingCard.Flipped)
        {
            Texture = CardResource.BackTexture;
        } else
        {
            Texture = CardResource.FrontTexture;
        }
    }

    public int GetCustomPower()
    {
        return PlayingCard.GetCustomPower();
    }

    public int GetTotalPower()
    {
        return PlayingCard.GetTotalPower();
    }

    public int GetCustomCost()
    {
        return PlayingCard.GetCustomCost();
    }

    public int GetTotalCost()
    {
        return PlayingCard.GetTotalCost();
    }
}
