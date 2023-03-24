using DiscordRPC;
using DiscordRPC.Logging;
using Godot;
using Serilog;
using System;

public partial class DiscordManager : Node
{
	private DiscordRpcClient _client;

	public DiscordManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;

		_client = new DiscordRpcClient("1088873310911205508");

		_client.Logger = new DiscordLogger() { Level = LogLevel.Info };

		_client.OnReady += (sender, e) =>
		{
			Log.Information("Dicord RPC Received Ready from user {0}", e.User.Username);
		};

		_client.OnPresenceUpdate += (sender, e) =>
		{
			Log.Information("Dicord RPC Received Update! {0}", e.Presence);
		};

		_client.Initialize();

		_client.SetPresence(new RichPresence()
		{
			Details = "Chillin' in Main Menu",
			State = "https://discord.com/TODO - In development",
			Assets = new Assets()
			{
				LargeImageKey = "image_large",
				LargeImageText = "OPS Pro is still in development",
				SmallImageKey = "image_small"
			}
		});
	}

    public override void _ExitTree()
    {
        base._ExitTree();
		_client?.Dispose();
	}

	public void UpdateDetails(string details)
	{
		_client.UpdateState(details);
	}

	public void UpdateState(string state)
	{
		_client.UpdateState(state);
	}

	public void UpdateSmallAsset(string key, string text = null)
	{
		_client.UpdateSmallAsset(key, text);
	}

	public void UpdateLargeAsset(string key, string text = null)
    {
		_client.UpdateLargeAsset(key, text);
    }
}
