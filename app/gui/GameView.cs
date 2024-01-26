using Godot;
using OPSProServer.Contracts.Models;
using Serilog;
using System;
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
		GameSocketConnector.Instance.BoardUpdated -= BoardUpdated;

		base._ExitTree();
	}

    public override void _Ready()
	{
		Gameboard = GetNode<Gameboard>("VBoxContainer/Gameboard");
		CardInfoPanel = GetNode<CardInfoPanel>("CardInfoPanel");
		OPSWindow = GetNode<OPSWindow>("OPSWindow");
		RPSWindow = GetNode<RPSWindow>("RPSWindow");

		GameSocketConnector.Instance.ConnectionClosed += Instance_ConnectionClosed;
		GameSocketConnector.Instance.ConnectionFailed += Instance_ConnectionFailed;
		GameSocketConnector.Instance.RoomDeleted += Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomExcluded += Instance_RoomExcluded;
        GameSocketConnector.Instance.ChooseFirstPlayerToPlay += Instance_ChooseFirstPlayerToPlay;
		GameSocketConnector.Instance.FirstPlayerDecided += FirstPlayerDecided;
		GameSocketConnector.Instance.BoardUpdated += BoardUpdated;

		PrepareGame();
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
		playerArea.Playmat.CardsDonDeck = playerGameInformation.DonDeck;
		playerArea.Playmat.CardsCostDeck = playerGameInformation.DonAvailable;
		playerArea.Playmat.CardsRestedCostDeck = playerGameInformation.DonRested;
        playerArea.Playmat.LeaderSlotCard.Card.UpdateCard(playerGameInformation.Leader);
		playerArea.Playmat.StageSlotCard.Card.UpdateCard(playerGameInformation.Stage);
		playerArea.Playmat.SetDeck(playerGameInformation.Deck);
		playerArea.Playmat.SetLifes(playerGameInformation.Lifes);
		playerArea.Playmat.SetTrash(playerGameInformation.Trash);
		playerArea.Hand.SetCards(playerGameInformation.Hand);
		playerArea.UpdateSlotCardsAction(playerGameInformation.CurrentPhase);
    }

    private void BoardUpdated(object sender, Game game)
	{
		var myGameInfo = game.GetMyPlayerInformation(GameSocketConnector.Instance.UserId);
		var opponentGameInfo = game.GetOpponentPlayerInformation(GameSocketConnector.Instance.UserId);
		UpdatePlayerBoard(Gameboard.PlayerArea, myGameInfo);
		UpdatePlayerBoard(Gameboard.OpponentArea, opponentGameInfo);
        //if (e.UserId != GameSocketConnector.Instance.UserId)
        //{
        //	Gameboard.OpponentArea.Playmat.LeaderSlotCard.Guid = e.Leader;
        //	Gameboard.OpponentArea.Playmat.LifeSlotCard.Guid = e.Life;
        //	Gameboard.OpponentArea.Playmat.DeckSlotCard.Guid = e.Deck;
        //	Gameboard.OpponentArea.Playmat.StageSlotCard.Guid = e.Stage;
        //	Gameboard.OpponentArea.Playmat.TrashSlotCard.Guid = e.Trash;
        //	Gameboard.OpponentArea.Playmat.CostSlotCard.Guid = e.Cost;
        //	Gameboard.OpponentArea.Playmat.DonDeckSlotCard.Guid = e.DonDeck;
        //	for (int i = 0; i < e.Characters.Count; i++)
        //	{
        //		if (i < Gameboard.OpponentArea.Playmat.CharactersSlots.Count)
        //		{
        //			Gameboard.OpponentArea.Playmat.CharactersSlots[i].Guid = e.Characters[i];
        //		}
        //	}
        //}
    }

	private void InitConnection(Room room)
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

			//Gameboard.PrepareGameboard(firstPlayer == GameSocketConnector.Instance.UserId);
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
}
