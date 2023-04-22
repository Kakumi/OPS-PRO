using Godot;
using Serilog;
using System;
using System.Linq;

public partial class SlotCard : TextureRect
{
	private GameSlotCardActionResource _cardActionResource;
	[Export]
	public GameSlotCardActionResource CardActionResource
    {
		get => _cardActionResource;
		set
        {
			_cardActionResource = value;
			if (Options != null)
            {
				CardActionUpdated(value);
			}
        }
	}

	private bool _border;
	[Export]
	public bool Border
    {
		get => _border;
		set
        {
			_border = value;
			SelfModulate = new Color(SelfModulate.R, SelfModulate.G, SelfModulate.B, value ? 1f : 0f);
        }
    }

	public Guid Guid { get; private set; }
	public Card Card { get; private set; }
	public bool Selected { get; private set; }
	public MenuButton Options { get; private set; }

	[Signal]
	public delegate void OnInvokCardEventHandler(CardResource cardResource);

	public override void _Ready()
	{
		Guid = Guid.NewGuid();
		Card = GetNode<Card>("MarginContainer/Card");
		Options = GetNode<MenuButton>("Options");
		Options.GetPopup().IdPressed += OnOptionsPressed;
		Options.GetPopup().VisibilityChanged += OnOptionsPopupVisibilityChanged;

		Selected = false;
		CardActionUpdated(CardActionResource);
	}

	private void CardActionUpdated(GameSlotCardActionResource gameSlotCardActionResource)
    {
		var validResource = gameSlotCardActionResource != null && gameSlotCardActionResource.Actions != null && gameSlotCardActionResource.Actions.Count > 0;
		Options.Disabled = !validResource;

		if (validResource)
		{
			Options.GetPopup().Clear();
			gameSlotCardActionResource.Actions.ToList().ForEach(x =>
			{
				Options.GetPopup().AddItem(Tr(x.GetTrKey()), (int)x);
			});
		}
	}

	private void OnLeftClickCard(CardResource cardResource)
	{
		if (!Options.Disabled)
        {
			Options.ShowPopup();
		}
	}

	public void ToggleSelection()
    {
		Selected = !Selected;
		if (Selected)
        {
			Select();
        } else
        {
			Unselect();
        }
    }

	public void Select()
    {
		SelfModulate = new Color(255, 0, 0, 1);
	}

	public void Unselect()
	{
		SelfModulate = new Color(255, 255, 255, 1);
	}

	private void OnOptionsPressed(long id)
	{
		if (id == 0)
        {
			EmitSignal(SignalName.OnInvokCard, Card.CardResource);
        }
	}

	private void OnOptionsPopupVisibilityChanged()
	{
		if (Options.GetPopup().Visible)
		{
			Select();
		}
		else
		{
			Unselect();
		}
	}
}
