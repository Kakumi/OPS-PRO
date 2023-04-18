using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Gameboard : VBoxContainer
{
	public SelectCardDialog SelectCardDialog { get; private set; }
	public Playmat OpponentPlaymat { get; private set; }
	public HandContainer OpponentHandContainer { get; private set; }
	public GamePlayerInfo OpponentPlayerInfo { get; private set; }
	public Playmat MyPlaymat { get; private set; }
	public HandContainer MyHandContainer { get; private set; }
	public GamePlayerInfo MyPlayerInfo { get; private set; }

	[Signal]
	public delegate void MouseEnterCardEventHandler(Card card);

	[Signal]
	public delegate void MouseExitCardEventHandler(Card card);

	public override void _Ready()
	{
		SelectCardDialog = GetNode<SelectCardDialog>("SelectCardDialog");

		OpponentPlaymat = GetNode<Playmat>("OpponentHBox/OpponentSide/OpponentPlaymat");
		OpponentHandContainer = GetNode<HandContainer>("OpponentHandContainer");
		OpponentPlayerInfo = GetNode<GamePlayerInfo>("OpponentHBox/OpponentPlayerInfo");
		MyPlaymat = GetNode<Playmat>("MyHBox/MySide/MyPlaymat");
		MyHandContainer = GetNode<HandContainer>("MyHandContainer");
		MyPlayerInfo = GetNode<GamePlayerInfo>("MyHBox/MyPlayerInfo");

		OpponentPlaymat.MouseEnterCard += OpponentPlaymat_MouseEnterCard;
        OpponentPlaymat.MouseExitCard += OpponentPlaymat_MouseExitCard;
		OpponentPlaymat.GameFinished += OpponentPlaymat_GameFinished;

		MyPlaymat.MouseEnterCard += MyPlaymat_MouseEnterCard;
        MyPlaymat.MouseExitCard += MyPlaymat_MouseExitCard;
		MyPlaymat.CardDrawn += MyPlaymat_CardDrawn;
        MyPlaymat.GameFinished += MyPlaymat_GameFinished;

        //MyHandContainer.InvokeCard += MyHandContainer_InvokeCard;

		var deck = DeckManager.Instance.LoadDecks().Where(x => x.IsValid()).First();
		MyPlaymat.Init(deck);
	}

    private void MyHandContainer_InvokeCard(SlotCard slotCard)
    {
		if (MyPlaymat.AddCharacter(slotCard.Card.CardResource))
		{
			slotCard.QueueFree();
		}
	}

    private void MyPlaymat_GameFinished(bool victory)
    {
        throw new NotImplementedException();
	}

	private void OpponentPlaymat_GameFinished(bool victory)
	{
		throw new NotImplementedException();
	}

	private void MyPlaymat_CardDrawn(CardResource cardResource)
    {
		if (cardResource != null)
        {
			Log.Information($"Card drawn, add it to the hand.");
			var slotCard = MyHandContainer.AddCard(cardResource);
			slotCard.MouseEntered += () => MyPlaymat_MouseEnterCard(slotCard.Card);
			slotCard.MouseExited += () => MyPlaymat_MouseExitCard(slotCard.Card);
			slotCard.OnInvokCard += (x) => MyHandContainer_InvokeCard(slotCard);
		} else
        {
			Log.Warning($"Card drawn but null (game finished ?)");
		}
	}

    private void OpponentPlaymat_MouseEnterCard(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    private void OpponentPlaymat_MouseExitCard(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

    private void MyPlaymat_MouseEnterCard(Card card)
    {
        EmitSignal(SignalName.MouseEnterCard, card);
    }

    private void MyPlaymat_MouseExitCard(Card card)
    {
        EmitSignal(SignalName.MouseExitCard, card);
    }

	private void OnSelectDialogMouseEntered(Card card)
	{
		EmitSignal(SignalName.MouseEnterCard, card);
	}

	private void OnSelectDialogMouseExited(Card card)
	{
		EmitSignal(SignalName.MouseExitCard, card);
	}


	private void Test1()
    {
        MyPlaymat.LeaderSlotCard.Card.ToggleFlip();
    }

    private void Test2()
    {
        MyPlaymat.LeaderSlotCard.Card.ToggleRest();
	}

	public void OnQuitPressed()
	{
		try
		{
			var parent = this.SearchParent<GameView>();
			if (parent == null)
			{
				Log.Error("GameView not found, can't close pane.");
			}
			else
			{
				parent?.QueueFree();
				AppInstance.Instance.ShowMainMenu();
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private void OnToggleInfoPressed()
	{
		Log.Debug($"Toggle players info area");
		OpponentPlayerInfo.Visible = !OpponentPlayerInfo.Visible;
		MyPlayerInfo.Visible = !MyPlayerInfo.Visible;
	}

	private void TestDraw()
    {
		MyPlaymat.DrawCard();
	}

	private void ShowCardsDialog(List<CardResource> cardResources, CardSelectorSource source)
	{
		SelectCardDialog.SetCards(cardResources, source);
		SelectCardDialog.Cancellable = true;
		SelectCardDialog.Selection = 0;
		SelectCardDialog.Title = string.Format(Tr("GAME_VIEW_CARD_TITLE"), cardResources.Count, Tr(source.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();
	}

	private void ShowSelectCardDialog(List<Tuple<CardResource, Guid, CardSelectorSource>> cards, int selection, CardSelectorAction action, bool cancellable = false)
	{
		SelectCardDialog.SetCards(cards);
		SelectCardDialog.Cancellable = cancellable;
		SelectCardDialog.Selection = selection;
		SelectCardDialog.Title = string.Format(Tr("GAME_SELECT_CARD_TITLE"), selection, Tr(action.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();
	}

	private void TestPopup()
	{
		ShowCardsDialog(MyHandContainer.Hand.GetChildren().OfType<SlotCard>().Select(x => x.Card.CardResource).ToList(), CardSelectorSource.Hand);
    }

	private void OnSelectCardDialogConfirmed()
    {
		if (SelectCardDialog.Selection > 0)
        {
			SelectCardDialog.Visible = SelectCardDialog.Selection == SelectCardDialog.GetSelecteds().Count();
		} else
        {
			SelectCardDialog.Hide();
		}
	}
}
