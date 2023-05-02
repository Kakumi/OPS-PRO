using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class RoomSelector : VBoxContainer
{
	[Export]
	public PackedScene RoomInfoScene { get; set; }

	private IReadOnlyList<Room> _rooms;

	public Label PlayerLabel { get; private set; }
	public CheckBox ShowPasswords { get; private set; }
	public Label Message { get; private set; }
	public Container RoomsContainer { get; private set; }
	public Button CreateButton { get; private set; }
	public Button RefreshButton { get; private set; }
	public OPSWindow OPSWindow { get; private set; }
	public CreateRoomDialog CreateRoomDialog { get; private set; }
	public RoomDialog RoomDialog { get; private set; }

	private Config _config;

	public override void _ExitTree()
	{
		GameSocketConnector.Instance.ConnectionClosed -= ConnectionClosed;
		GameSocketConnector.Instance.ConnectionFailed -= ConnectionFailed;
		GameSocketConnector.Instance.RoomDeleted -= Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomUpdated -= Instance_RoomUpdated;
		GameSocketConnector.Instance.RoomExcluded -= Instance_RoomExcluded;

		base._ExitTree();
	}

	public override void _Ready()
	{
		PlayerLabel = GetNode<Label>("InfoContainer/MarginContainer/HBoxContainer/Player/Label");
		ShowPasswords = GetNode<CheckBox>("InfoContainer/MarginContainer/HBoxContainer/Settings/ShowPasswords");
		Message = GetNode<Label>("PanelContainer/Messages/Label");
		RoomsContainer = GetNode<Container>("PanelContainer/MarginContainer/ScrollContainer/RoomsContainer");
		CreateButton = GetNode<Button>("InfoContainer/MarginContainer/HBoxContainer/Buttons/Create");
		RefreshButton = GetNode<Button>("InfoContainer/MarginContainer/HBoxContainer/Buttons/Refresh");
		OPSWindow = GetNode<OPSWindow>("OPSWindow");
		CreateRoomDialog = GetNode<CreateRoomDialog>("CreateRoomDialog");
		RoomDialog = GetNode<RoomDialog>("RoomDialog");

		GameSocketConnector.Instance.ConnectionClosed += ConnectionClosed;
		GameSocketConnector.Instance.ConnectionFailed += ConnectionFailed;
		GameSocketConnector.Instance.RoomDeleted += Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomUpdated += Instance_RoomUpdated;
		GameSocketConnector.Instance.RoomExcluded += Instance_RoomExcluded;

		_config = new Config();
		SettingsManager.Instance.Init(_config);
		UpdateUsername();

		OPSWindow.Label.Text = Tr("ROOMS_CONNECTING_POPUP");

		Task.Run(async () =>
		{
			CreateRoomDialog.Hide();
			RoomDialog.Hide();

			OPSWindow.PopupCentered();
			var logged = await GameSocketConnector.Instance.Login();
			if (logged)
			{
				await GameSocketConnector.Instance.Register(_config.Username);

				ShowPasswords.Disabled = false;
				CreateButton.Disabled = false;
				RefreshButton.Disabled = false;
				OPSWindow.Close();

				await RefreshRooms();
			}
		});
	}

    private async Task RefreshRooms()
	{
        _rooms = await GameSocketConnector.Instance.GetRooms();
        UpdateRooms();
    }

    private void UpdateRooms()
    {
		RoomsContainer.GetChildren().ToList().ForEach(x => x.QueueFree());
		foreach(var room in _rooms)
        {
			if (!room.UsePassword || ShowPasswords.ButtonPressed)
            {
				var roomInfo = RoomInfoScene.Instantiate<RoomInfo>();
				RoomsContainer.AddChild(roomInfo);
				roomInfo.Init(room);
                roomInfo.ClickJoinRoom += RoomInfo_ClickJoinRoom;
                roomInfo.JoinRoomResult += RoomInfo_JoinRoomResult;
			}
		}

		if (RoomsContainer.GetChildCount() == 0)
		{
			Message.Text = Tr("ROOMS_NO_ROOM");
		}
		else
		{
			Message.Text = null;
		}
	}

    private void RoomInfo_ClickJoinRoom()
    {
		ShowPopup("ROOMS_JOINING");
    }

    private void RoomInfo_JoinRoomResult(bool success)
    {
        if (!success)
		{
			ShowPopup("ROOMS_JOINED_FAILED", () => OPSWindow.Close());
		}
    }

    private void ConnectionClosed()
	{
		ShowPopup("ROOMS_CONNECTION_CLOSED", ConnectionClosed_Ok_Pressed);
	}

	private void ConnectionFailed()
	{
		ShowPopup("ROOMS_CONNECTION_FAILED", ConnectionClosed_Ok_Pressed);
	}

	private void ConnectionClosed_Ok_Pressed()
	{
		OnQuitPressed();
	}

	private void UpdateUsername()
    {
		PlayerLabel.Text = string.Format(Tr("ROOMS_USER"), _config.Username);
    }

	private void OnCreatePressed()
	{
		RoomDialog.Hide();
		OPSWindow.Hide();
		CreateRoomDialog.PopupCentered();
	}

	private async void OnRefreshPressed()
    {
		try
        {
			await RefreshRooms();
		} catch(Exception ex)
        {
			Log.Error(ex, ex.Message);
        }
	}

    private async void OnQuitPressed()
    {
        try
		{
			await GameSocketConnector.Instance.Logout();
			QueueFree();
			AppInstance.Instance.ShowMainMenu();
		}
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
        }
    }

	private void OnShowPasswordsToggled(bool activated)
    {
		UpdateRooms();
    }

	private async void OnRoomCreated()
	{
		try
		{
			await RefreshRooms();

			OPSWindow.Hide();
			CreateRoomDialog.Hide();
			RoomDialog.Room = _rooms.First(x => x.Creator.Id == GameSocketConnector.Instance.UserId);
			RoomDialog.PopupCentered();
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
			await RefreshRooms();
		});
	}

	private void Instance_RoomExcluded(object sender, EventArgs e)
	{
		ShowPopup(Tr("ROOMS_EXCLUDED"), async () =>
		{
			OPSWindow.Close();
			await RefreshRooms();
		});
	}

	private void Instance_RoomUpdated(object sender, Room e)
	{
		OPSWindow.Hide();
		CreateRoomDialog.Hide();
		RoomDialog.Room = e;
		
		if (!RoomDialog.Visible)
		{
			RoomDialog.PopupCentered();
		}
	}

	private void ShowPopup(string text, Action action = null)
	{
		RoomDialog.Hide();
		CreateRoomDialog.Hide();

		OPSWindow.Show(text, action);
	}
}
