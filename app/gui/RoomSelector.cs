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
	public CreateRoomDialog CreateRoomDialog { get; private set; }
	public RoomDialog RoomDialog { get; private set; }
	public OPSWindow OPSWindow { get; private set; }

	public Config Config { get; private set; }

	public override void _ExitTree()
	{
		if (GameSocketConnector.Instance.Connected)
		{
			Task.Run(async () =>
			{
				await GameSocketConnector.Instance.Logout();
			});
		}

		GameSocketConnector.Instance.ConnectionStarted -= Instance_ConnectionStarted;
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
		CreateRoomDialog = GetNode<CreateRoomDialog>("CreateRoomDialog");
		RoomDialog = GetNode<RoomDialog>("RoomDialog");
		OPSWindow = GetNode<OPSWindow>("OPSWindow");

		GameSocketConnector.Instance.ConnectionStarted += Instance_ConnectionStarted;
		GameSocketConnector.Instance.RoomDeleted += Instance_RoomDeleted;
		GameSocketConnector.Instance.RoomUpdated += Instance_RoomUpdated;
		GameSocketConnector.Instance.RoomExcluded += Instance_RoomExcluded;

		Config = new Config();
		SettingsManager.Instance.Init(Config);

		UpdateUsername();

		if (GameSocketConnector.Instance.Connected)
        {
			Task.Run(async () => await InitConnection());
        } else
        {
			Task.Run(async () =>
			{
				ShowPopup("ROOMS_CONNECTING_POPUP");
				var logged = await GameSocketConnector.Instance.Login();
				if (logged)
				{
					await GameSocketConnector.Instance.Register(Config.Username);

					OPSWindow.Close();
				}
			});
		}
	}

    #region Connection Events

    private async Task InitConnection()
	{
		ShowPasswords.Disabled = false;
		CreateButton.Disabled = false;
		RefreshButton.Disabled = false;

		UpdateUsername();

		await RefreshRooms();
	}

	private async void Instance_ConnectionStarted()
	{
		try
        {
			await InitConnection();
		} catch(Exception ex)
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

    #endregion

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

	private void UpdateUsername()
    {
		PlayerLabel.Text = string.Format(Tr("ROOMS_USER"), Config?.Username ?? "...");
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

    private void OnQuitPressed()
    {
        try
		{
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
