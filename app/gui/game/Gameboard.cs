using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Gameboard : VBoxContainer
{
	[Export]
	public NodePath GameViewPath { get; set; }

	public GameView GameView { get; set; }

	public SelectCardDialog SelectCardDialog { get; private set; }
	public PlayerArea OpponentArea { get; private set; }
	public PlayerArea PlayerArea { get; private set; }

	public override void _Ready()
	{
		GameView = GetNode<GameView>(GameViewPath);

		SelectCardDialog = GetNode<SelectCardDialog>("SelectCardDialog");

		OpponentArea = GetNode<PlayerArea>("OpponentArea");
		PlayerArea = GetNode<PlayerArea>("PlayerArea");

		OpponentArea.Playmat.GameFinished += OpponentPlaymat_GameFinished;
		PlayerArea.Playmat.GameFinished += MyPlaymat_GameFinished;

		var deck = DeckManager.Instance.LoadDecks().Where(x => x.IsValid()).First();
		PlayerArea.Playmat.Init(deck);
	}

    private void MyPlaymat_GameFinished(bool victory)
    {
        throw new NotImplementedException();
	}

	private void OpponentPlaymat_GameFinished(bool victory)
	{
		throw new NotImplementedException();
	}

	private void OnSelectDialogMouseEntered(Card card)
	{
		GameView.CardInfoPanel.ShowcardResource(card);
	}

	private void OnSelectDialogMouseExited(Card card)
	{
		GameView.CardInfoPanel.ShowcardResource(card);
	}


	private void Test1()
    {
		PlayerArea.Playmat.LeaderSlotCard.Card.ToggleFlip();
    }

    private void Test2()
    {
		PlayerArea.Playmat.LeaderSlotCard.Card.ToggleRest();
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
		OpponentArea.PlayerInfo.Visible = !OpponentArea.PlayerInfo.Visible;
		PlayerArea.PlayerInfo.Visible = !PlayerArea.PlayerInfo.Visible;
	}

	private void TestDraw()
    {
		PlayerArea.Playmat.DrawCard();
	}

	public void ShowCardsDialog(List<CardResource> cardResources, CardSelectorSource source)
	{
		SelectCardDialog.SetCards(cardResources, source);
		SelectCardDialog.Cancellable = true;
		SelectCardDialog.Selection = 0;
		SelectCardDialog.Title = string.Format(Tr("GAME_VIEW_CARD_TITLE"), cardResources.Count, Tr(source.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();
	}

	public void ShowSelectCardDialog(List<Tuple<CardResource, Guid, CardSelectorSource>> cards, int selection, CardSelectorAction action, Action<List<Tuple<CardResource, Guid, CardSelectorSource>>> command, bool cancellable = false)
	{
		SelectCardDialog.SetCards(cards);
		SelectCardDialog.Cancellable = cancellable;
		SelectCardDialog.Selection = selection;
		SelectCardDialog.Title = string.Format(Tr("GAME_SELECT_CARD_TITLE"), selection, Tr(action.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();
	}

	private void TestPopup()
	{
		ShowCardsDialog(PlayerArea.Hand.Hand.GetChildren().OfType<SlotCard>().Select(x => x.Card.CardResource).ToList(), CardSelectorSource.Hand);
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
