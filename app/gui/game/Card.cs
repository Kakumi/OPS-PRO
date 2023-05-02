using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Card : TextureRect
{
    public CardResource CardResource { get; protected set; }
    public bool Rested { get; protected set; }
    public bool Flipped { get; protected set; }
    public Dictionary<int, StatDuration> CustomAttack { get; protected set; }
    public Dictionary<int, StatDuration> CustomCost { get; protected set; }
    public bool Destructable { get; protected set; }

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
        Rested = false;
        Flipped = false;
        Destructable = false;

        CustomAttack = new Dictionary<int, StatDuration>();
        CustomCost = new Dictionary<int, StatDuration>();
    }

    public void SetCardResource(CardResource cardResource, bool download = false)
    {
        if (cardResource == null)
        {
            Texture = null;
            CardResource = null;
            EmitSignal(SignalName.CardResourceUpdated, cardResource);
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

            EmitSignal(SignalName.CardResourceUpdated, cardResource);
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

    public void SetDestructable(bool destructable)
    {
        Destructable = destructable;
    }

    public int GetCustomPower()
    {
        return CustomAttack.Keys.Sum();
    }

    public int GetTotalPower()
    {
        return CardResource.Power + GetCustomPower();
    }

    public int GetCustomCost()
    {
        return CustomCost.Keys.Sum();
    }

    public int GetTotalCost()
    {
        return CardResource.Cost + GetCustomCost();
    }

    public void RemoveStatDuration(StatDuration type)
    {
        CustomAttack = CustomAttack.Where(kv => kv.Value != type).ToDictionary(kv => kv.Key, kv => kv.Value);
        CustomCost = CustomCost.Where(kv => kv.Value != type).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
