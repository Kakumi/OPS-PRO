using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;

public partial class RoomInfo : VBoxContainer
{
	public Guid RoomId { get; private set; }

	public Label Username { get; private set; }
	public Label Description { get; private set; }
	public TextureRect TexturePassword { get; private set; }
	public Button JoinButton { get; private set; }

	public override void _Ready()
	{
		Username = GetNode<Label>("HBoxContainer/Username");
		Description = GetNode<Label>("HBoxContainer/Desc");
		TexturePassword = GetNode<TextureRect>("HBoxContainer/TexturePassword");
		JoinButton = GetNode<Button>("HBoxContainer/JoinButton");
	}

	public void Init(Room room)
    {
		RoomId = room.Id;
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
			await GameSocketConnector.Instance.JoinRoom(RoomId, null);
        } catch(Exception ex)
        {
			Log.Error(ex, ex.Message);
        }
    }
}
