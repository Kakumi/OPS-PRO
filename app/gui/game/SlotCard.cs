using Godot;
using OPSProServer.Contracts.Models;
using Serilog;
using System;
using System.Linq;

public partial class SlotCard : TextureRect
{
	[Export]
	public GameSlotCardActionResource CardActionResource { get; set; }

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

	public Guid Guid { get; internal set; }
	public Card Card { get; private set; }
	public bool Selected { get; private set; }
	public MenuButton Options { get; private set; }

	[Signal]
	public delegate void CardActionEventHandler(SlotCard slotCard, GameSlotCardActionResource resource, int id);

	public override void _Ready()
	{
		Guid = Guid.NewGuid();
		Card = GetNode<Card>("MarginContainer/Card");
		Options = GetNode<MenuButton>("Options");
		Options.GetPopup().IdPressed += OnOptionsPressed;
		Options.GetPopup().VisibilityChanged += OnOptionsPopupVisibilityChanged;

		Selected = false;
	}
	
	public void CardActionUpdated(IPhase phase)
    {
		if (phase != null)
        {
			var hasOptions = CardActionResource != null && CardActionResource.Actions.Any(x => phase.IsActionAllowed(CardActionResource.Source, x));
			Options.Disabled = !hasOptions;

			if (hasOptions)
			{
				Options.GetPopup().Clear();
				CardActionResource.Actions.ToList().ForEach(x =>
				{
					Options.GetPopup().AddItem(Tr(x.GetTrKey()), (int)x);
					var index = Options.GetPopup().GetItemIndex((int)x);
					Options.GetPopup().SetItemDisabled(index, !phase.IsActionAllowed(CardActionResource.Source, x));
				});
			}
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
		EmitSignal(SignalName.CardAction, this, CardActionResource, (int) id);
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
