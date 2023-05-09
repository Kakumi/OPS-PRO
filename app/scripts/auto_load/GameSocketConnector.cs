using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using OPSProServer.Contracts.Contracts;
using OPSProServer.Contracts.Events;
using OPSProServer.Contracts.Hubs;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class GameSocketConnector : Node
{
	public static GameSocketConnector Instance { get; private set; }

    private HubConnection _connection;

    public Guid UserId { get; private set; }
    public string Username { get; private set; }

    public bool Connected
    {
        get => _connection.State == HubConnectionState.Connected;
    }

    [Signal]
    public delegate void ConnectionStartedEventHandler();

    [Signal]
    public delegate void UserConnectedEventHandler();

    [Signal]
    public delegate void ConnectionClosedEventHandler();

    [Signal]
    public delegate void ConnectionFailedEventHandler();

    public event EventHandler<Room> RoomUpdated;

    public event EventHandler RoomDeleted;

    public event EventHandler RoomExcluded;

    public event EventHandler GameLaunched;

    public event EventHandler<RockPaperScissorsResult> RPSExecuted;

    public event EventHandler ChooseFirstPlayerToPlay;

    public event EventHandler<Guid> FirstPlayerDecided;

    public override void _Ready()
	{
		Instance = this;

        _connection = new HubConnectionBuilder().WithUrl("http://localhost:5282/ws/game").Build();

        InitReceiver();
    }

    private void InitReceiver()
    {
        _connection.Closed += (error) =>
        {
            Log.Information($"Connection closed (Game server).");

            EmitSignal(SignalName.ConnectionClosed);

            return Task.CompletedTask;
        };

        _connection.On<Room>(nameof(IRoomHubEvent.RoomUpdated), (room) =>
        {
            RoomUpdated?.Invoke(this, room);
        });

        _connection.On(nameof(IRoomHubEvent.RoomDeleted), () =>
        {
            RoomDeleted?.Invoke(this, new EventArgs());
        });

        _connection.On(nameof(IRoomHubEvent.RoomExcluded), () =>
        {
            RoomExcluded?.Invoke(this, new EventArgs());
        });

        _connection.On(nameof(IGameHubEvent.GameLaunched), () =>
        {
            GameLaunched?.Invoke(this, new EventArgs());
        });

        _connection.On<RockPaperScissorsResult>(nameof(IGameHubEvent.RPSExecuted), (rps) =>
        {
            RPSExecuted?.Invoke(this, rps);
        });

        _connection.On(nameof(IGameHubEvent.ChooseFirstPlayerToPlay), () =>
        {
            ChooseFirstPlayerToPlay?.Invoke(this, new EventArgs());
        });

        _connection.On<Guid>(nameof(IGameHubEvent.FirstPlayerDecided), (guid) =>
        {
            FirstPlayerDecided?.Invoke(this, guid);
        });
    }

	public async Task<bool> Login()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _connection.StartAsync();

                Log.Error("Connection started (Game server).");

                EmitSignal(SignalName.ConnectionStarted);
            }
            catch (Exception ex)
            {
                Log.Error("Connection failed (Game server) because {Message}", ex.Message);
                EmitSignal(SignalName.ConnectionFailed);
                return false;
            }
        }
        else
        {
            Log.Warning("Start connection (Game server) but it's already started");
        }

        return true;
    }

    public async Task<bool> LoginAndRegister()
    {
        var success = await Login();
        if (success)
        {
            await Register(SettingsManager.Instance.Config.Username);
        }

        return success;
    }

	public async Task Logout()
    {
        if (_connection.State != HubConnectionState.Disconnected)
        {
            try
            {
                await _connection.StopAsync();
            }
            catch (Exception ex)
            {
                Log.Error("Close connection (Game server) failed because {Message}", ex.Message);
                Log.Error(ex.Message);
            }
        } else
        {
            Log.Warning("Close connection (Game server) but it's already closed");
        }
    }

    public async Task<Guid> Register(string username)
    {
        Log.Information("Register user {Username}", username);
        UserId = await _connection.InvokeAsync<Guid>(nameof(IUserHub.Register), username);
        Username = username;

        EmitSignal(SignalName.UserConnected);

        return UserId;
    }

    public async Task<List<Room>> GetRooms()
    {
        Log.Information($"Gettings Rooms");
        return await _connection.InvokeAsync<List<Room>>(nameof(IRoomHub.GetRooms));
    }

    public async Task<Room> GetRoom()
    {
        Log.Information("Getting Room for user {UserId}", UserId);
        return await _connection.InvokeAsync<Room>(nameof(IRoomHub.GetRoom), UserId);
    }

    public async Task<bool> CreateRoom(string desc, string password)
    {
        Log.Information($"Create Room");
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.CreateRoom), UserId, password, desc);
    }

    public async Task<bool> SetReady(bool ready)
    {
        Log.Information("Set ready to {Ready} inside room", ready);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.SetReady), UserId, ready);
    }

    public async Task<bool> JoinRoom(Guid roomId, string password)
    {
        Log.Information("User {UserId} ask room {RoomId}", UserId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.JoinRoom), UserId, roomId, password);
    }

    public async Task<bool> LeaveRoom()
    {
        Log.Information("User {UserId} leave room", UserId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.LeaveRoom), UserId);
    }

    public async Task<bool> Exclude(Guid opponentId, Guid roomId)
    {
        Log.Information("User {UserId} exclude {OpponentId} from room {RoomId}", UserId, opponentId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.Exclude), UserId, opponentId, roomId);
    }

    public async Task<bool> LaunchGame(Guid roomId)
    {
        Log.Information("User {UserId} launch game for room {RoomId}", UserId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.LaunchGame), roomId);
    }

    public async Task<bool> SetRockPaperScissors(RockPaperScissors rps)
    {
        Log.Information("User {UserId} set rock paper scissors to rps {Rps}", UserId, rps);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.SetRockPaperScissors), UserId, rps);
    }

    public async Task<bool> SetFirstPlayerToPlay(Guid playerId)
    {
        Log.Information("User {UserId} set first player to play to {PlayerId}", UserId, playerId);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.SetFirstPlayer), UserId, playerId);
    }
}
