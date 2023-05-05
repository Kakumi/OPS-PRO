using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class MainMenu : Control
{
    public Button DeckBuilder { get; protected set; }
    public Button Settings { get; protected set; }
    public Button Quit { get; protected set; }
    public Settings SettingsContainer { get; protected set; }
    public OPSWindow OPSWindow { get; private set; }

    public override void _ExitTree()
    {
        CardManager.Instance.CardLoaded -= OnCardLoaded;
        CardManager.Instance.LoadFailed -= CardLoadFailed;

        base._ExitTree();
    }

    public override void _EnterTree()
    {
        if (GameSocketConnector.Instance.Connected)
        {
            Task.Run(async () =>
            {
                await GameSocketConnector.Instance.Logout();
            });
        }

        base._EnterTree();
    }

    public override void _Ready()
    {
        DeckBuilder = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/DeckBuilder");
        Settings = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Settings");
        Quit = GetNode<Button>("PanelContainer/MarginContainer/VBoxContainer/Quit");
        SettingsContainer = GetNode<Settings>("Settings");
        OPSWindow = GetNode<OPSWindow>("OPSWindow");

        CardManager.Instance.CardLoaded += OnCardLoaded;
        CardManager.Instance.LoadFailed += CardLoadFailed;
        if (!CardManager.Instance.Loaded)
        {
            OPSWindow.Title = Tr("AUTOLOAD_CARDS_LOAD_TITLE");
            OPSWindow.Show("AUTOLOAD_CARDS_LOAD_MESSAGE");
        }

        DeckBuilder.Text = Tr(DeckBuilder.Text);
        Settings.Text = Tr(Settings.Text);
        Quit.Text = Tr(Quit.Text);
    }

    private void OnCardLoaded()
    {
        OPSWindow.Close();
    }

    private void CardLoadFailed()
    {
        OPSWindow.Show("AUTOLOAD_CARDS_LOAD_FAILED_DESERIALIZE", () => OnQuitPressed());
    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }

    public void OnSettingsPressed()
    {
        SettingsContainer.Show();
    }
}
