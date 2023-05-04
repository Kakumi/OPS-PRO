using Godot;
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

	public override void _ExitTree()
	{
		GameSocketConnector.Instance.ConnectionStarted -= Instance_ConnectionStarted;
		GameSocketConnector.Instance.RoomDeleted -= Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomExcluded -= Instance_RoomExcluded;

		base._ExitTree();
	}

	public override void _Ready()
	{
		Gameboard = GetNode<Gameboard>("VBoxContainer/Gameboard");
		CardInfoPanel = GetNode<CardInfoPanel>("CardInfoPanel");
		OPSWindow = GetNode<OPSWindow>("OPSWindow");

		Task.Run(async () =>
		{
			var logged = GameSocketConnector.Instance.Connected;
			if (!logged)
			{
				ShowPopup("ROOMS_CONNECTING_POPUP");
				logged = await GameSocketConnector.Instance.Login();
			}

			if (logged)
            {
				OPSWindow.Close();
				//ShowPopup("ROOMS_GETTING_DATA");
			} else
            {
				ShowPopup("ROOMS_CONNECTION_FAILED", () => Quit());
			}
		});
	}

	#region Connection Events

	private async Task InitConnection()
	{

	}

	private async void Instance_ConnectionStarted()
	{
		try
		{
			await InitConnection();
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private void Instance_RoomDeleted(object sender, EventArgs e)
	{
		ShowPopup("ROOMS_DELETED", async () =>
		{
			OPSWindow.Close();
		});
	}

	private void Instance_RoomExcluded(object sender, EventArgs e)
	{
		ShowPopup(Tr("ROOMS_EXCLUDED"), async () =>
		{
			OPSWindow.Close();
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
