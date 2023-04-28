using Godot;
using System;

public partial class RoomInfo : VBoxContainer
{
	public Guid RoomId { get; set; }

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

	public void SetUsername(string username)
    {
		Username.Text = string.Format(Tr("ROOMS_CONNECTED_USER"), username);
    }
}
