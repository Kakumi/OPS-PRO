using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;
using System.Threading.Tasks;

public partial class RoomInfo : VBoxContainer
{
	public Room Room { get; private set; }

	public Label Username { get; private set; }
	public Label Description { get; private set; }
	public TextureRect TexturePassword { get; private set; }
	public Button JoinButton { get; private set; }
	public PasswordDialog PasswordDialog { get; private set; }

	[Signal]
	public delegate void ClickJoinRoomEventHandler();

	[Signal]
	public delegate void JoinRoomResultEventHandler(bool success);

	public override void _Ready()
	{
		Username = GetNode<Label>("HBoxContainer/Username");
		Description = GetNode<Label>("HBoxContainer/Desc");
		TexturePassword = GetNode<TextureRect>("HBoxContainer/TexturePassword");
		JoinButton = GetNode<Button>("HBoxContainer/JoinButton");
		PasswordDialog = GetNode<PasswordDialog>("PasswordDialog");
	}

	public void Init(Room room)
    {
		Room = room;
		Username.Text = room.Creator?.Username;
		Description.Text = room.Description;
		Description.TooltipText = room.Description;
		if (!room.UsePassword)
        {
			TexturePassword.SelfModulate = new Color(TexturePassword.SelfModulate.R, TexturePassword.SelfModulate.G, TexturePassword.SelfModulate.B, 0f);
		}

		if (room.Creator.Id == GameSocketConnector.Instance.UserId)
        {
			JoinButton.Disabled = true;
        }
    }

	private async void OnJoinPressed()
	{
		try
		{
			if (Room.UsePassword)
			{
				PasswordDialog.PopupCentered();
			}
			else
			{
				await JoinRoom(null);
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
    }

	private async void OnPasswordEntered(string password)
	{
		try
		{
			await JoinRoom(password);
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private async Task JoinRoom(string password)
	{
		EmitSignal(SignalName.ClickJoinRoom);
		var success = await GameSocketConnector.Instance.JoinRoom(Room.Id, password);
		EmitSignal(SignalName.JoinRoomResult, success);
	}
}
