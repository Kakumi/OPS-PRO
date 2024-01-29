using Godot;
using OPSPro.app.models;
using OPSProServer.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SelectCardDialog : AcceptDialog
{
	[Export]
	public bool Cancellable { get; set; }

	[Export(PropertyHint.Range, "0,120,")]
	public int Selection { get; set; } = 0;

	[Export(PropertyHint.Range, "0,9999,")]
	public int CardWidth { get; set; } = 207;

	[Export(PropertyHint.Range, "0,9999,")]
	public int CardHeight { get; set; } = 281;

	[Export]
	public PackedScene CardScene { get; set; }

	public List<CardResource> SelectedCard;

	public Container Cards { get; private set; }

	[Signal]
	public delegate void MouseEnterCardEventHandler(Card card);

	[Signal]
	public delegate void MouseExitCardEventHandler(Card card);

	[Signal]
	public delegate void CloseDialogEventHandler(Card card);

	private bool _canceled;

	public override void _Ready()
	{
		Cards = GetNode<Container>("MarginContainer/VBoxContainer/HScrollBar/Cards");
	}

	public void SetCards(List<CardResource> cards, CardSource source)
    {
		SetCards(cards.Select(x => new SelectCard(x, Guid.Empty, source)).ToList());
    }

    public void SetCards(List<SlotCard> cards)
    {
        SetCards(cards.Select(x => new SelectCard(x)).ToList());
    }

    public void SetCards(List<SelectCard> cards)
	{
		var showSource = cards.GroupBy(x => x.Source).Count() != 1;

		var excepts = new List<string>();
		cards.ForEach(x =>
		{
			var instance = CardScene.Instantiate<SlotCardSelector>();
			Cards.AddChild(instance);
			excepts.Add(instance.Name);
			instance.SlotCard.Card.SetCardResource(x.CardResource);
			instance.TargetGuid = x.Id;
			instance.Source = x.Source;
			instance.ShowSource = showSource;
			instance.CustomMinimumSize = new Vector2(CardWidth, CardHeight);

			instance.SlotCard.Options.Disabled = true;
			instance.SlotCard.Card.LeftClickCard += (x) => CardClicked(instance.SlotCard, x);
			instance.SlotCard.Card.MouseEntered += () => EmitSignal(SignalName.MouseEnterCard, instance.SlotCard.Card);
			instance.SlotCard.Card.MouseExited += () => EmitSignal(SignalName.MouseExitCard, instance.SlotCard.Card);
		});

        Cards.GetChildren().Where(x => !excepts.Contains(x.Name)).ToList().ForEach(x => x.QueueFree());
    }

	public List<SlotCardSelector> GetSelecteds()
    {
		return Cards.GetChildren().ToList().OfType<SlotCardSelector>().Where(x => x.SlotCard.Selected).ToList();
	}

	public List<SelectCard> GetResult()
	{
        return Cards.GetChildren().ToList().OfType<SlotCardSelector>().Where(x => x.SlotCard.Selected).Select(x =>
        {
			return new SelectCard(x.SlotCard.Card.CardResource, x.TargetGuid, x.Source);
        }).ToList();
	}

	private void CardClicked(SlotCard instance, CardResource x)
    {
		if (Selection > 0)
		{
			instance.ToggleSelection();

			GetOkButton().Disabled = !Cancellable && GetSelecteds().Count != Selection;
		}
	}

	private void OnCanceled()
    {
		_canceled = true;
    }

	private void OnSelectCardDialogConfirmed()
    {
		if (GetSelecteds().Count == Selection)
        {
			Hide();
        }
    }

	private void OnVisibilityChanged()
	{
		if (!Cancellable && _canceled)
		{
			_canceled = false;
			if (!Visible)
			{
				PopupCentered();
			}
		}

		if (!Visible)
        {
			EmitSignal(SignalName.CloseDialog);
        } else
        {
			GetOkButton().Disabled = Selection > 0 && GetSelecteds().Count != Selection;
		}
	}
}
