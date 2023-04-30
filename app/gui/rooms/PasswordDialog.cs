using Godot;
using System;

public partial class PasswordDialog : AcceptDialog
{
	public LineEdit Password { get; private set; }

	[Signal]
	public delegate void PasswordEnteredEventHandler(string password);

	public override void _Ready()
	{
		Password = GetNode<LineEdit>("HBoxContainer/LineEdit");
	}

	private void OnConfirmed()
    {
		EmitSignal(SignalName.PasswordEntered, Password.Text);
		Hide();
    }

	private void OnAboutToPopup()
    {
		Password.Text = null;
    }
}
