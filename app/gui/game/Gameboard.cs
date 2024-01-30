using Godot;
using OPSPro.app.models;
using OPSProServer.Contracts.Models;
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

	public void ShowCardsDialog(List<CardResource> cardResources, CardSource source)
	{
		SelectCardDialog.SetCards(cardResources, source);
		SelectCardDialog.Cancellable = true;
		SelectCardDialog.MinSelection = 0;
		SelectCardDialog.MaxSelection = 0;
		SelectCardDialog.Title = string.Format(Tr("GAME_VIEW_CARD_TITLE"), cardResources.Count, Tr(source.GetTrKey()).ToLower());
		SelectCardDialog.PopupCentered();
	}

	public async Task<List<SelectCard>> ShowSelectCardDialog(List<SlotCard> cards, int selection, CardAction action, bool cancellable = false)
	{
		var cardsFormated = cards.Select(x => new SelectCard(x)).ToList();
		return await ShowSelectCardDialog(cardsFormated, selection, action, cancellable);
	}

	public async Task<List<SelectCard>> ShowSelectCardDialog(List<SelectCard> cards, int selection, CardAction action, bool cancellable = false)
	{
		return await ShowSelectCardDialog(cards, selection, selection, string.Format(Tr("GAME_SELECT_CARD_TITLE"), selection, Tr(action.GetTrKey()).ToLower()), cancellable);
	}

    public async Task<List<SelectCard>> ShowSelectCardDialog(List<SelectCard> cards, int minSelection, int maxSelection, string title, bool cancellable = false)
    {
        SelectCardDialog.SetCards(cards);
        SelectCardDialog.Cancellable = cancellable;
        SelectCardDialog.MinSelection = minSelection;
		SelectCardDialog.MaxSelection = maxSelection;
        SelectCardDialog.Title = title;
        SelectCardDialog.PopupCentered();

        await ToSignal(SelectCardDialog, SelectCardDialog.SignalName.CloseDialog);

        return SelectCardDialog.GetResult();
    }

	public void UpdateNextPhaseButton(PhaseType phaseType)
	{
		NextPhaseButton.Visible = phaseType != PhaseType.Opponent;
		NextPhaseButton.Text = Tr(phaseType.GetNextTrKey());
	}

	private async void OnNextPhaseButtonPressed()
    {
		await GameSocketConnector.Instance.GoToNextPhase();
    }
}
