using Godot;
using Microsoft.AspNetCore.SignalR.Client;
using OPSProServer.Contracts.Events;
using OPSProServer.Contracts.Hubs;
using OPSProServer.Contracts.Models;
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
    public DeckResource DeckResource { get; internal set; }

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

    public event EventHandler<SecureRoom> RoomUpdated;
    public event EventHandler RoomDeleted;
    public event EventHandler RoomExcluded;
    public event EventHandler<Guid> GameStarted;
    public event EventHandler<RPSResult> RPSExecuted;
    public event EventHandler ChooseFirstPlayerToPlay;
    public event EventHandler<Guid> FirstPlayerDecided;
    public event EventHandler<Game> BoardUpdated;
    public event EventHandler RockPaperScissorsStarted;
    public event EventHandler<UserAlertMessage> AlertReceived;
    public event EventHandler<UserGameMessage> GameMessageReceived;
    public event EventHandler<Guid> GameFinished;
    public event EventHandler<bool> WaitOpponent;
    public event EventHandler<FlowActionRequest> FlowRequested;

    public override void _Ready()
	{
		Instance = this;

        //TODO URL
        //_connection = new HubConnectionBuilder().WithUrl("http://26.80.66.111:5000/ws/game").Build();
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

        _connection.On<SecureRoom>(nameof(IRoomHubEvent.RoomUpdated), (room) =>
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

        _connection.On<Guid>(nameof(IGameHubEvent.GameStarted), (userToStart) =>
        {
            GameStarted?.Invoke(this, userToStart);
        });

        _connection.On<RPSResult>(nameof(IGameHubEvent.RPSExecuted), (rps) =>
        {
            RPSExecuted?.Invoke(this, rps);
        });

        _connection.On(nameof(IGameHubEvent.ChooseFirstPlayerToPlay), () =>
        {
            ChooseFirstPlayerToPlay?.Invoke(this, new EventArgs());
        });

        _connection.On<Game>(nameof(IGameHubEvent.BoardUpdated), (game) =>
        {
            BoardUpdated?.Invoke(this, game);
        });

        _connection.On(nameof(IGameHubEvent.RockPaperScissorsStarted), () =>
        {
            RockPaperScissorsStarted?.Invoke(this, new EventArgs());
        });

        _connection.On<UserAlertMessage>(nameof(IGameHubEvent.UserAlertMessage), (userAlertMessage) =>
        {
            AlertReceived?.Invoke(this, userAlertMessage);
        });

        _connection.On<UserGameMessage>(nameof(IGameHubEvent.UserGameMessage), (userGameMessage) =>
        {
            GameMessageReceived?.Invoke(this, userGameMessage);
        });

        _connection.On<Guid>(nameof(IGameHubEvent.GameFinished), (result) =>
        {
            GameFinished?.Invoke(this, result);
        });

        _connection.On<bool>(nameof(IGameHubEvent.WaitOpponent), (result) =>
        {
            WaitOpponent?.Invoke(this, result);
        });

        _connection.On<FlowActionRequest>(nameof(IGameHubEvent.FlowActionRequest), (result) =>
        {
            FlowRequested?.Invoke(this, result);
        });
    }

	public async Task<bool> Login()
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            try
            {
                await _connection.StartAsync();

                Log.Information("Connection started (Game server).");

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

    public async Task<List<SecureRoom>> GetRooms()
    {
        Log.Information($"Gettings Rooms");
        return await _connection.InvokeAsync<List<SecureRoom>>(nameof(IRoomHub.GetRooms));
    }

    public async Task<SecureRoom> GetRoom()
    {
        Log.Information("Getting Room for user {UserId}", UserId);
        return await _connection.InvokeAsync<SecureRoom>(nameof(IRoomHub.GetRoom));
    }

    public async Task<bool> CreateRoom(string desc, string password)
    {
        Log.Information($"Create Room");
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.CreateRoom), password, desc);
    }

    public async Task<bool> SetReady()
    {
        if (DeckResource != null)
        {
            var cards = DeckResource.GetCardsId();
            Log.Information("Set ready for User {UserId} with deck '{Name}' inside room", UserId, DeckResource.Name);
            return await _connection.InvokeAsync<bool>(nameof(IRoomHub.SetReady), DeckResource.Name, cards);
        }

        return false;
    }

    public async Task<bool> JoinRoom(Guid roomId, string password)
    {
        Log.Information("User {UserId} ask room {RoomId}", UserId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.JoinRoom), roomId, password);
    }

    public async Task<bool> LeaveRoom()
    {
        Log.Information("User {UserId} leave room", UserId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.LeaveRoom));
    }

    public async Task<bool> Exclude(Guid opponentId, Guid roomId)
    {
        Log.Information("User {UserId} exclude {OpponentId} from room {RoomId}", UserId, opponentId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IRoomHub.Exclude), opponentId, roomId);
    }

    public async Task<bool> LaunchGame(Guid userToStart)
    {
        Log.Information("User {UserId} launch game and set User ID '{UserToStart}' to start.", UserId, userToStart);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.LaunchGame), userToStart);
    }

    public async Task<bool> StartRPS(Guid roomId)
    {
        Log.Information("User {UserId} launch rock paper scissors for room '{RoomId}'.", UserId, roomId);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.LaunchRockPaperScissors), roomId);
    }

    public async Task<bool> SetRockPaperScissors(RPSChoice rps)
    {
        Log.Information("User {UserId} set rock paper scissors to rps {Rps}", UserId, rps);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.SetRockPaperScissors), rps);
    }

    public async Task<bool> GoToNextPhase()
    {
        Log.Information("User {UserId} ask to go next phase", UserId);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.NextPhase));
    }

    public async Task<bool> Summon(Guid handCardId)
    {
        Log.Information("User {UserId} want to summon card {HandCardId}.", UserId, handCardId);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.Summon), handCardId);
    }

    public async Task<bool> GetAttackableCards(Guid attacker)
    {
        Log.Information("User {UserId} ask for attackable characters with {Attacker}.", UserId, attacker);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.GetAttackableCards), attacker);
    }

    public async Task<bool> GiveDonCard(Guid target)
    {
        Log.Information("User {UserId} want to give DON!! card to {Target}.", UserId, target);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.GiveDonCard), target);
    }

    public async Task<bool> ResolveFlow(Guid actionId, bool accepted)
    {
        return await ResolveFlow(actionId, accepted, new List<Guid>());
    }

    public async Task<bool> ResolveFlow(Guid actionId, bool accepted, List<Guid> cardsId)
    {
        Log.Information("User {UserId} resolve action {ActionId} with {Count} cards.", UserId, actionId, cardsId.Count);
        return await _connection.InvokeAsync<bool>(nameof(IGameHub.ResolveFlow), new FlowActionResponse(actionId, accepted, cardsId));
    }
}
