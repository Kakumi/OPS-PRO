using Godot;
using Serilog;
using System;

public partial class RoomSelector : VBoxContainer
{
	[Export]
	public PackedScene RoomInfoScene { get; set; }

	public Label PlayerLabel { get; private set; }
	public CheckBox ShowPasswords { get; private set; }
	public Label NoRoomMessage { get; private set; }
	public Container RoomsContainer { get; private set; }

	private Config _config;

	public override void _Ready()
	{
		PlayerLabel = GetNode<Label>("InfoContainer/MarginContainer/HBoxContainer/Player/Label");
		ShowPasswords = GetNode<CheckBox>("InfoContainer/MarginContainer/HBoxContainer/Settings/ShowPasswords");
		NoRoomMessage = GetNode<Label>("PanelContainer/Messages/NoRoom");
		RoomsContainer = GetNode<Container>("PanelContainer/MarginContainer/ScrollContainer/RoomsContainer");

		_config = new Config();
		SettingsManager.Instance.Init(_config);
		UpdateUsername();
	}

	private void UpdateUsername()
    {
		PlayerLabel.Text = string.Format(Tr("ROOMS_CONNECTED_USER"), _config.Username);
    }

	private void OnCreatePressed()
    {

    }

	private void OnRefreshPressed()
    {

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

    }
}
