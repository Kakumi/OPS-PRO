using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Gameboard : VBoxContainer
{
	[Export]
	public NodePath GameViewPath { get; set; }

	public GameView GameView { get; set; }

	public SelectCardDialog SelectCardDialog { get; private set; }
	public PlayerArea OpponentArea { get; private set; }
	public PlayerArea PlayerArea { get; private set; }
	public Button NextPhaseButton { get; private set; }
	public int TurnCounter { get; private set; }

	public override async void _Ready()
	{
		GameView = GetNode<GameView>(GameViewPath);

		SelectCardDialog = GetNode<SelectCardDialog>("SelectCardDialog");

		OpponentArea = GetNode<PlayerArea>("OpponentArea");
		PlayerArea = GetNode<PlayerArea>("PlayerArea");

		OpponentArea.Playmat.GameFinished += OpponentPlaymat_GameFinished;
		PlayerArea.Playmat.GameFinished += MyPlaymat_GameFinished;

		NextPhaseButton = GetNode<Button>("ButtonsContainer/MarginContainer/HBoxContainer/GameButtons/NextPhaseButton");

		TurnCounter = 0;
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
		GameView.Quit();
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

	public async Task<List<Tuple<CardResource, Guid, CardSelectorSource>>> ShowSelectCardDialog(List<SlotCard> cards, int selection, CardSelectorAction action, bool cancellable = false)
	{
		var cardsFormated = cards.Select(x => new Tuple<CardResource, Guid, CardSelectorSource>(x.Card.CardResource, x.Guid, x.CardActionResource.Source)).ToList();
		return await ShowSelectCardDialog(cardsFormated, selection, action, cancellable);
	}

	public async Task<List<Tuple<CardResource, Guid, CardSelectorSource>>> ShowSelectCardDialog(List<Tuple<CardResource, Guid, CardSelectorSource>> cards, int selection, CardSelectorAction action, bool cancellable = false)
	{
		SelectCardDialog.SetCards(cards);
		SelectCardDialog.Cancellable = cancellable;
		SelectCardDialog.Selection = selection;
		SelectCardDialog.Title = string.Format(Tr("GAME_SELECT_CARD_TITLE"), selection, Tr(action.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();

		await ToSignal(SelectCardDialog, SelectCardDialog.SignalName.CloseDialog);

		return SelectCardDialog.GetResult();
	}

	private async void TestPopup()
	{
		//ShowCardsDialog(PlayerArea.Hand.Hand.GetChildren().OfType<SlotCard>().Select(x => x.Card.CardResource).ToList(), CardSelectorSource.Hand);
		var result = await ShowSelectCardDialog(PlayerArea.Hand.Hand.GetChildren().OfType<SlotCard>().Select(x =>
        {
			return new Tuple<CardResource, Guid, CardSelectorSource>(x.Card.CardResource, x.Guid, x.CardActionResource.Source);
		}).ToList(), 2, CardSelectorAction.Throw);
	}

	private async void OnNextPhaseButtonPressed()
    {
		await PlayerArea.UpdatePhase(PlayerArea.CurrentPhase.NextPhase());
    }

	public void IncrementTurn()
    {
		TurnCounter++;
    }

	public void PrepareGameboard(bool isFirst)
    {
		Task.Run(async () =>
		{
			try
			{
				//TODO Synchroniser la main
				//TODO Synchroniser le board (les ids sont déjà fait)
				//TODO Synchroniser les infos du terrain (nombre de vie, ...)

				//await PlayerArea.Playmat.SyncPlaymat();
				PlayerArea.Playmat.Init(GameSocketConnector.Instance.DeckResource);

				if (isFirst)
				{
					await PlayerArea.UpdatePhase(new DrawPhase());
				}
				else
				{
					await PlayerArea.UpdatePhase(new OpponentPhase());
				}
			} catch(Exception ex)
            {
				Log.Error(ex, ex.Message);
				GameView.ShowPopup(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), () => GameView.OPSWindow.Close());
            }
		});
	}
}
