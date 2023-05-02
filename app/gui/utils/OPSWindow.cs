using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

public partial class OPSWindow : Window
{
	[Export]
	public bool Cancellable { get; set; }

	[Export]
	public string Message { get; set; }

	public Label Label { get; set; }
	public Container Buttons { get; set; }

	[Signal]
	public delegate void WindowClosedEventHandler();

	public override void _Ready()
	{
		Label = GetNode<Label>("PanelContainer/MarginContainer/VBoxContainer/Label");
		Buttons = GetNode<Container>("PanelContainer/MarginContainer/VBoxContainer/Buttons");
		Label.Text = Tr(Message);
	}

	public void ClearButtons()
    {
		Buttons.GetChildren().OfType<Node>().ToList().ForEach(x => x.QueueFree());
    }

	public Button AddButton(string text)
    {
		var button = new Button();

		Buttons.AddChild(button);

		button.Text = text;

		return button;
    }

	private void OnCloseRequested()
    {
		if (Cancellable)
        {
			Close();
        }
    }

	public void Close()
    {
		Hide();
		EmitSignal(SignalName.WindowClosed);
	}

	public void Show(string text, Action action = null)
	{
		ClearButtons();
		Label.Text = Tr(text);

		if (action != null)
		{
			var buttonOk = AddButton("OK");
			buttonOk.Pressed += action;
		}

		PopupCentered();
	}
}
