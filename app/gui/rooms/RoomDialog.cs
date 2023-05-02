using Godot;
using OPSProServer.Contracts.Contracts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RoomDialog : Window
{
	[Export]
	public bool Cancellable { get; set; }

	private List<DeckResource> _decks;
	private Room _room;
	public Room Room
    {
		get => _room;
		set
        {
			_room = value;
			UpdateMenu();
		}
    }

	public Label CreatorLabel { get; private set; }
	public CheckBox CreatorCheckbox { get; private set; }
	public Container OpponentInfo { get; private set; }
	public Container OpponentInfoEmpty { get; private set; }
	public Label OpponentLabel { get; private set; }
	public CheckBox OpponentCheckbox { get; private set; }
	public Button OpponentExcludeButton { get; private set; }
	public OptionButton DeckOptions { get; private set; }
	public Button ReadyButton { get; private set; }
	public Button StartButton { get; private set; }

	[Signal]
	public delegate void WindowClosedEventHandler();

	public override void _Ready()
	{
		var creatorContainer = GetNode<Container>("PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/CreatorInfo");
		CreatorLabel = creatorContainer.GetNode<Label>("Label");
		CreatorCheckbox = creatorContainer.GetNode<CheckBox>("CheckBox");

		OpponentInfo = GetNode<Container>("PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/OpponentInfo");
		OpponentLabel = OpponentInfo.GetNode<Label>("Label");
		OpponentCheckbox = OpponentInfo.GetNode<CheckBox>("CheckBox");
		OpponentExcludeButton = OpponentInfo.GetNode<Button>("Button");

		OpponentInfoEmpty = GetNode<Container>("PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/OpponentInfoEmpty");

		DeckOptions = GetNode<OptionButton>("PanelContainer/MarginContainer/VBoxContainer/VBoxContainer/DeckSelectContainer/DeckOptions");

		ReadyButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Buttons/RightContainer/Ready");
		StartButton = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Buttons/RightContainer/Start");

		_decks = new List<DeckResource>();
	}

	private void OnAboutToPopup()
	{
		UpdateMenu();

		_decks = DeckManager.Instance.LoadDecks().Where(x => x.IsValid()).ToList();
		DeckOptions.Clear();
		foreach (var deck in _decks)
		{
			DeckOptions.AddItem(deck.Name);
		}

		DeckOptions.Selected = -1;
	}

    private void UpdateMenu()
    {
		CreatorLabel.Text = Room.Creator.Username;
		CreatorCheckbox.ButtonPressed = Room.CreatorReady;

		OpponentLabel.Text = Room.Opponent?.Username;
		OpponentCheckbox.ButtonPressed = Room.OpponentReady;
		OpponentExcludeButton.Visible = Room.Creator.Id == GameSocketConnector.Instance.UserId;
		StartButton.Visible = OpponentExcludeButton.Visible;
		StartButton.Disabled = !Room.CreatorReady || !Room.OpponentReady;

		OpponentInfo.Visible = Room.Opponent != null;
		OpponentInfoEmpty.Visible = !OpponentInfo.Visible;
	}

	private bool IsReady()
    {
		if (Room.Creator.Id == GameSocketConnector.Instance.UserId)
        {
			return Room.CreatorReady;
        }

		return Room.OpponentReady;
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

	private async void OnReadyPressed()
    {
		try
        {
			await GameSocketConnector.Instance.SetReady(!IsReady());
        } catch(Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private async void OnStartPressed()
	{
		try
		{

		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private async void OnQuitPressed()
	{
		try
		{
			await GameSocketConnector.Instance.LeaveRoom();
			Close();
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private async void OnExcludePressed()
	{
		try
		{
			if (Room.Opponent != null)
			{
				await GameSocketConnector.Instance.Exclude(Room.Opponent.Id, Room.Id);
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, ex.Message);
		}
	}

	private void OnDeckSelected(int index)
    {
		var deckName = DeckOptions.GetItemText(index);
		var deck = _decks.FirstOrDefault(x => x.Name == deckName);

		ReadyButton.Disabled = deck == null || !deck.IsValid();
	}
}
