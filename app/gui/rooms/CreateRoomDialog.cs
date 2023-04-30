using Godot;
using Serilog;
using System;

public partial class CreateRoomDialog : ConfirmationDialog
{
	public LineEdit Description { get; private set; }
	public LineEdit Password { get; private set; }
	public RichTextLabel Message { get; private set; }

	[Signal]
	public delegate void RoomCreatedEventHandler();

	[Signal]
	public delegate void WindowClosedEventHandler();

	public override void _Ready()
	{
		Description = GetNode<LineEdit>("VBoxContainer/HBoxContainer/Description");
		Password = GetNode<LineEdit>("VBoxContainer/HBoxContainer2/Password");
		Message = GetNode<RichTextLabel>("VBoxContainer/Message");
	}

	private void OnCanceled()
    {
		EmitSignal(SignalName.WindowClosed);
    }

	private async void OnConfirmedPressed()
    {
		try
		{
			Message.Show();
			UpdateMessage(Tr("ROOMS_CREATING"), "white");
			var success = await GameSocketConnector.Instance.CreateRoom(Description.Text, Password.Text);
			if (success)
            {
				EmitSignal(SignalName.RoomCreated);
				Hide();
            } else
            {
				UpdateMessage(Tr("ROOMS_CREATE_FAILED"));
			}
        } catch(Exception ex)
        {
			Log.Error(ex, ex.Message);
			UpdateMessage(string.Format(Tr("GENERAL_ERROR_OCCURED"), ex.Message));
		}
    }

	private void UpdateMessage(string msg, string color = "red")
	{
		Message.Text = $"[center][color={color}]{msg}[/color][/center]";
	}

	private void OnAboutToPopup()
    {
		Description.Text = null;
		Password.Text = null;
		Message.Text = null;
		Message.Hide();
	}
}
