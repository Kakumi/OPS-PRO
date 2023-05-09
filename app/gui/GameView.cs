using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;
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

	private void InitConnection(Room room)
	{
		OPSWindow.Close();
		RPSWindow.PopupCentered();

		Gameboard.OpponentArea.UserId = room.GetOpponent(GameSocketConnector.Instance.UserId).Id;
		Gameboard.PlayerArea.UserId = GameSocketConnector.Instance.UserId;
	}

	private async void Instance_ChooseFirstPlayerToPlay(object sender, EventArgs e)
	{
		RPSWindow.Hide();

		var result = await OPSWindow.Ask(Tr("GAME_CHOOSE_FIRST"));
		if (result)
        {
			await GameSocketConnector.Instance.SetFirstPlayerToPlay(Gameboard.PlayerArea.UserId);
        } else
		{
			await GameSocketConnector.Instance.SetFirstPlayerToPlay(Gameboard.OpponentArea.UserId);
		}
	}

	private void FirstPlayerDecided(object sender, Guid firstPlayer)
	{
		RPSWindow.Hide();

		Gameboard.PlayerArea.FirstToPlay = firstPlayer == GameSocketConnector.Instance.UserId;
		Gameboard.OpponentArea.FirstToPlay = !Gameboard.PlayerArea.FirstToPlay;

		if (Gameboard.PlayerArea.FirstToPlay)
        {
			OPSWindow.Show(Tr("GAME_FIRST_TO_PLAY"), () => OPSWindow.Close());
        } else
		{
			OPSWindow.Show(Tr("GAME_SECOND_TO_PLAY"), () => OPSWindow.Close());
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
