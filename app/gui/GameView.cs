using Godot;
using OPSPro.app.models;
using OPSProServer.Contracts.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class GameView : HBoxContainer
{
	[Export]
	public PackedScene QuitScenePath { get; set; }

	public Gameboard Gameboard { get; private set; }
	public CardInfoPanel CardInfoPanel { get; private set; }
	public OPSWindow OPSWindow { get; private set; }
	public RPSWindow RPSWindow { get; private set; }
	public Label Title { get; private set; }
	public Game GameState { get; private set; }

	public override void _ExitTree()
	{
		Task.Run(async () =>
		{
			await GameSocketConnector.Instance.LeaveRoom();
		});

		GameSocketConnector.Instance.ConnectionClosed -= Instance_ConnectionClosed;
		GameSocketConnector.Instance.ConnectionFailed -= Instance_ConnectionFailed;
		GameSocketConnector.Instance.RoomDeleted -= Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomExcluded -= Instance_RoomExcluded;
		GameSocketConnector.Instance.ChooseFirstPlayerToPlay -= Instance_ChooseFirstPlayerToPlay;
		GameSocketConnector.Instance.FirstPlayerDecided -= FirstPlayerDecided;
		GameSocketConnector.Instance.GameStarted -= GameStarted;
		GameSocketConnector.Instance.BoardUpdated -= BoardUpdated;
        GameSocketConnector.Instance.AlertReceived -= AlertReceived;
        GameSocketConnector.Instance.GameMessageReceived -= GameMessageReceived;
        GameSocketConnector.Instance.GameFinished -= GameFinished;
        GameSocketConnector.Instance.WaitOpponent -= WaitOpponent;
        GameSocketConnector.Instance.FlowRequested -= FlowRequested;

        base._ExitTree();
	}

    public override void _Ready()
	{
		Gameboard = GetNode<Gameboard>("VBoxContainer/Gameboard");
		CardInfoPanel = GetNode<CardInfoPanel>("CardInfoPanel");
		OPSWindow = GetNode<OPSWindow>("OPSWindow");
		RPSWindow = GetNode<RPSWindow>("RPSWindow");
        Title = GetNode<Label>("VBoxContainer/PanelContainer/Label");

		GameSocketConnector.Instance.ConnectionClosed += Instance_ConnectionClosed;
		GameSocketConnector.Instance.ConnectionFailed += Instance_ConnectionFailed;
		GameSocketConnector.Instance.RoomDeleted += Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomExcluded += Instance_RoomExcluded;
        GameSocketConnector.Instance.ChooseFirstPlayerToPlay += Instance_ChooseFirstPlayerToPlay;
		GameSocketConnector.Instance.FirstPlayerDecided += FirstPlayerDecided;
        GameSocketConnector.Instance.GameStarted += GameStarted;
        GameSocketConnector.Instance.BoardUpdated += BoardUpdated;
		GameSocketConnector.Instance.AlertReceived += AlertReceived;
        GameSocketConnector.Instance.GameMessageReceived += GameMessageReceived;
        GameSocketConnector.Instance.GameFinished += GameFinished;
        GameSocketConnector.Instance.WaitOpponent += WaitOpponent;
        GameSocketConnector.Instance.FlowRequested += FlowRequested;


        PrepareGame();
	}

    private void GameStarted(object sender, Guid e)
    {
		RPSWindow.Hide();
    }

    private void PrepareGame()
	{
		Task.Run(async () =>
		{
			var logged = GameSocketConnector.Instance.Connected;
			if (!logged)
			{
				ShowPopup("ROOMS_CONNECTING_POPUP");
				logged = await GameSocketConnector.Instance.LoginAndRegister();
			}

			if (logged)
			{
				ShowPopup("ROOMS_GETTING_DATA");

				var room = await GameSocketConnector.Instance.GetRoom();
				if (room != null)
				{
					OPSWindow.Close();
					InitConnection(room);
				} else
				{
					ShowPopup("ROOMS_NOT_CONNECTED", () => Quit());
				}
			}
			else
			{
				ShowPopup("ROOMS_CONNECTION_FAILED", () => Quit());
			}
		});
	}

	private void UpdatePlayerBoard(PlayerArea playerArea, PlayerGameInformation playerGameInformation)
	{
		var isOwner = playerGameInformation.User.Id == GameSocketConnector.Instance.UserId;

        playerArea.Playmat.CardsDonDeck = playerGameInformation.DonDeck;
		playerArea.Playmat.CardsCostDeck = playerGameInformation.DonAvailable;
		playerArea.Playmat.CardsRestedCostDeck = playerGameInformation.DonRested;
        playerArea.Playmat.LeaderSlotCard.Card.UpdateCard(playerGameInformation.Leader);
		playerArea.Playmat.LeaderSlotCard.Card.IsOwner = isOwner;
        playerArea.Playmat.StageSlotCard.Card.UpdateCard(playerGameInformation.Stage);
        playerArea.Playmat.StageSlotCard.Card.IsOwner = isOwner;
        playerArea.Playmat.SetDeck(playerGameInformation.Deck);
		playerArea.Playmat.SetLifes(playerGameInformation.Lifes);
		playerArea.Playmat.SetTrash(playerGameInformation.Trash);
		playerArea.Hand.SetCards(playerGameInformation.Hand);
		playerArea.Hand.GetCards().ForEach(x => x.Card.IsOwner = isOwner);
		playerArea.UpdateSlotCardsAction(playerGameInformation.CurrentPhase);
		playerArea.Playmat.SetCharacters(playerGameInformation.Characters.ToList());
    }

    private void BoardUpdated(object sender, Game game)
	{
		GameState = game;
        var myGameInfo = game.GetMyPlayerInformation(GameSocketConnector.Instance.UserId);
		var opponentGameInfo = game.GetOpponentPlayerInformation(GameSocketConnector.Instance.UserId);
		UpdatePlayerBoard(Gameboard.PlayerArea, myGameInfo);
		UpdatePlayerBoard(Gameboard.OpponentArea, opponentGameInfo);
		Gameboard.UpdateNextPhaseButton(myGameInfo.CurrentPhase.PhaseType);

        Title.Text = string.Format(Tr("GAME_TITLE_VS"), game.CreatorGameInformation.User.Username, game.OpponentGameInformation.User.Username);
        Gameboard.PlayerArea.PlayerInfo.Update(game, myGameInfo);
        Gameboard.OpponentArea.PlayerInfo.Update(game, opponentGameInfo);
    }

    private void AlertReceived(object sender, UserAlertMessage e)
    {
		var message = string.Format(Tr(e.CodeMessage), e.Args);
		Log.Warning("Alert received: {Message}", message);
		ShowPopup(message, () => OPSWindow.Close());
    }

    private void InitConnection(SecureRoom room)
	{
		OPSWindow.Close();
		RPSWindow.PopupCentered();

		Gameboard.OpponentArea.UserId = room.GetOpponent(GameSocketConnector.Instance.UserId).Id;
		Gameboard.PlayerArea.UserId = GameSocketConnector.Instance.UserId;
	}

	private async void Instance_ChooseFirstPlayerToPlay(object sender, EventArgs e)
	{
		try
		{
			RPSWindow.Hide();

			var result = await OPSWindow.Ask(Tr("GAME_CHOOSE_FIRST"));
			if (result)
			{
				await GameSocketConnector.Instance.LaunchGame(Gameboard.PlayerArea.UserId);
			}
			else
			{
				await GameSocketConnector.Instance.LaunchGame(Gameboard.OpponentArea.UserId);
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
			ShowPopup(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), () => OPSWindow.Close());
		}
	}

	private void FirstPlayerDecided(object sender, Guid firstPlayer)
	{
		try
        {
			RPSWindow.Hide();

			Gameboard.PlayerArea.FirstToPlay = firstPlayer == GameSocketConnector.Instance.UserId;
			Gameboard.OpponentArea.FirstToPlay = !Gameboard.PlayerArea.FirstToPlay;

			if (Gameboard.PlayerArea.FirstToPlay)
			{
				ShowPopup(Tr("GAME_FIRST_TO_PLAY"), () => OPSWindow.Close());
			}
			else
			{
				ShowPopup(Tr("GAME_SECOND_TO_PLAY"), () => OPSWindow.Close());
			}
		} catch(Exception ex)
		{
			Log.Error(ex, ex.Message);
			ShowPopup(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message), () => OPSWindow.Close());
		}
	}

	#region Connection Events

	private void Instance_ConnectionClosed()
	{
		ShowPopup("SERVER_CONNECTION_CLOSED", () =>
		{
			Quit();
		});
	}

	private void Instance_ConnectionFailed()
	{
		ShowPopup("SERVER_CONNECTION_FAILED", () =>
		{
			Quit();
		});
	}

	private void Instance_RoomDeleted(object sender, EventArgs e)
	{
		ShowPopup("ROOMS_DELETED", () =>
		{
			Quit();
		});
	}

	private void Instance_RoomExcluded(object sender, EventArgs e)
	{
		ShowPopup(Tr("ROOMS_EXCLUDED"), () =>
		{
			Quit();
		});
	}

	#endregion

	public void Quit()
	{
		try
		{
			QueueFree();
			AppInstance.Instance.ShowPackedScene(QuitScenePath);
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	public void ShowPopup(string text, Action action = null, bool hideOthers = true)
	{
		if (hideOthers)
		{

		}

		OPSWindow.Show(text, action);
	}

	public async Task<bool> AskPopup(string text, bool hideOthers = true)
	{
		if (hideOthers)
		{

		}

		return await OPSWindow.Ask(text);
    }

    private void WaitOpponent(object sender, bool e)
    {
		if (e)
        {
            OPSWindow.Show("GAME_WAIT_OPPONENT");
        } else
		{
			OPSWindow.Close();
		}
    }

    private void GameMessageReceived(object sender, UserGameMessage e)
    {
        NotifierManager.Instance.Send("godot_game", string.Format(Tr(e.CodeMessage), e.Args));
    }

    private void GameFinished(object sender, Guid e)
    {
        throw new NotImplementedException();
    }

    private async void FlowRequested(object sender, FlowActionRequest e)
    {
        if (e.Type == FlowActionType.Question)
        {
            var result = await AskPopup(string.Format(Tr(e.CodeMessage), e.Args));
            await GameSocketConnector.Instance.ResolveFlow(e.FlowActionId, result);
        }
        else
        {
            var cards = GetAllCards(e.CardsId);
            var result = await Gameboard.ShowSelectCardDialog(cards, e.MinSelection, e.MaxSelection, string.Format(Tr(e.CodeMessage), e.Args), e.Cancellable);
            await GameSocketConnector.Instance.ResolveFlow(e.FlowActionId, result.Count > 0, result.Select(x => x.Id).ToList());
        }
    }

    private List<SelectCard> GetAllCards(List<Guid> ids)
	{
		var list = new List<SelectCard>();
		list.AddRange(GetAllCards(Gameboard.PlayerArea, ids));
		list.AddRange(GetAllCards(Gameboard.OpponentArea, ids));

		return list;
    }

	private SelectCard GetSelectCard(PlayingCard card, CardSource source)
	{
		var resource = card.GetCardResource();

        return new SelectCard(resource, card.Id, source);
    }


    private List<SelectCard> GetAllCards(PlayerArea playerArea, List<Guid> ids)
	{
		var list = new List<SelectCard>();
		if (ids.Contains(playerArea.Playmat.LeaderSlotCard.Card.PlayingCard.Id))
		{
			list.Add(GetSelectCard(playerArea.Playmat.LeaderSlotCard.Card.PlayingCard, CardSource.Leader));
        }
        if (playerArea.Playmat.StageSlotCard.Card.PlayingCard != null && ids.Contains(playerArea.Playmat.StageSlotCard.Card.PlayingCard.Id))
        {
            list.Add(GetSelectCard(playerArea.Playmat.StageSlotCard.Card.PlayingCard, CardSource.Stage));
        }
		foreach(var characterSlot in playerArea.Playmat.CharactersSlots)
        {
            if (characterSlot.Card != null && characterSlot.Card.PlayingCard != null && ids.Contains(characterSlot.Card.PlayingCard.Id))
            {
                list.Add(GetSelectCard(characterSlot.Card.PlayingCard, CardSource.Character));
            }
        }
		foreach(var card in playerArea.Playmat.GetDeck())
		{
			if (ids.Contains(card.Id))
            {
                list.Add(GetSelectCard(card, CardSource.Deck));
            }
        }
        foreach (var card in playerArea.Playmat.GetLifes())
        {
            if (ids.Contains(card.Id))
            {
                list.Add(GetSelectCard(card, CardSource.Life));
            }
        }
        foreach (var card in playerArea.Playmat.GetTrash())
        {
            if (ids.Contains(card.Id))
            {
                list.Add(GetSelectCard(card, CardSource.Trash));
            }
        }
        foreach (var card in playerArea.Hand.GetCards())
        {
            if (card.Card != null && card.Card.PlayingCard != null && ids.Contains(card.Card.PlayingCard.Id))
            {
                list.Add(GetSelectCard(card.Card.PlayingCard, CardSource.Hand));
            }
        }

		return list;
    }
}
