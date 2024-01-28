using Godot;
using OPSProServer.Contracts.Models;
using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class Extension
{
    public static T SearchOne<T>(this Godot.Collections.Array<Node> array) where T : Node
    {
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                return (T)enumerator.Current;
            }
            else if (enumerator.Current is Node)
            {
                var value = SearchOne<T>((enumerator.Current as Node).GetChildren());
                if (value != default(T))
                {
                    return value;
                }
            }
        }

        return default(T);
    }

    public static List<T> SearchChild<T>(this Godot.Collections.Array<Node> array) where T : Node
    {
        var list = new List<T>();

        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add(enumerator.Current as T);
            }
        }

        return list;
    }

    public static List<T> SearchAll<T>(this Godot.Collections.Array<Node> array) where T : Node
    {
        var list = new List<T>();
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add(enumerator.Current as T);
            }
            else if (enumerator.Current is Node)
            {
                list.AddRange(SearchAll<T>(enumerator.Current.GetChildren()));
            }
        }

        return list;
    }

    public static List<T> ToList<T>(this Array array)
    {
        var list = new List<T>();
        var enumerator = array.GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Current is T)
            {
                list.Add((T)enumerator.Current);
            }
        }

        return list;
    }

    public static void ConnectIfMissing(this Node node, string signal, GodotObject target, string method, uint flags = 0)
    {
        var callable = new Callable(target, new StringName(method));
        if (!node.IsConnected(signal, callable))
        {
            node.Connect(new StringName(signal), callable, flags);
        }
    }

    public static T SearchParent<T>(this Node node) where T : Node
    {
        T result = null;
        Node parent = node;
        do
        {
            parent = parent.GetParent();
            if (parent is T)
            {
                result = parent as T;
            }
        } while (parent != null && result == null);

        return result;
    }

    public static List<string> GetFiles(this DirAccess directory, string pattern)
    {
        List<string> files = new List<string>();
        Regex regex = new Regex(pattern);

        Log.Information($"Getting files at {directory.GetCurrentDir()} and pattern: {pattern}");

        directory.ListDirBegin();
        string file;
        while((file = directory.GetNext()) != null)
        {
            Log.Debug($"Found file {file}");
            var filepath = Path.Combine(directory.GetCurrentDir(), file.Replace(".import", string.Empty));

            if (string.IsNullOrEmpty(file))
            {
                break;
            }

            if (!directory.CurrentIsDir() && regex.IsMatch(file) && !files.Contains(filepath))
            {
                Log.Debug($"Valid pattern for file {file}");
                files.Add(filepath);
            }
        }

        return files;
    }

    public static Variant ToVariant(this object obj)
    {
        if (obj.GetType() == typeof(bool))
            return Variant.CreateFrom((bool) obj);
        if (obj.GetType() == typeof(char))
            return Variant.CreateFrom((char)obj);
        if (obj.GetType() == typeof(sbyte))
            return Variant.CreateFrom((sbyte)obj);
        if (obj.GetType() == typeof(short))
            return Variant.CreateFrom((short)obj);
        if (obj.GetType() == typeof(int))
            return Variant.CreateFrom((int)obj);
        if (obj.GetType() == typeof(long))
            return Variant.CreateFrom((long)obj);
        if (obj.GetType() == typeof(byte))
            return Variant.CreateFrom((byte)obj);
        if (obj.GetType() == typeof(ushort))
            return Variant.CreateFrom((ushort)obj);
        if (obj.GetType() == typeof(uint))
            return Variant.CreateFrom((uint)obj);
        if (obj.GetType() == typeof(ulong))
            return Variant.CreateFrom((ulong)obj);
        if (obj.GetType() == typeof(float))
            return Variant.CreateFrom((float)obj);
        if (obj.GetType() == typeof(double))
            return Variant.CreateFrom((double)obj);
        if (obj.GetType() == typeof(string))
            return Variant.CreateFrom((string)obj);

        return default(Variant);
    }

    public static string GetTrKey(this CardSource source)
    {
        switch (source)
        {
            case CardSource.Hand: return "GAME_SOURCE_HAND";
            case CardSource.Deck: return "GAME_SOURCE_DECK";
            case CardSource.Trash: return "GAME_SOURCE_TRASH";
            case CardSource.Life: return "GAME_SOURCE_LIFE";
            case CardSource.DonDeck: return "GAME_SOURCE_DONDECK";
            case CardSource.CostDeck: return "GAME_SOURCE_COSTDECK";
            case CardSource.Board: return "GAME_SOURCE_BOARD";
            case CardSource.OpponentHand: return "GAME_SOURCE_OPPONENT_HAND";
            case CardSource.OpponentDeck: return "GAME_SOURCE_OPPONENT_DECK";
            case CardSource.OpponentTrash: return "GAME_SOURCE_OPPONENT_TRASH";
            case CardSource.OpponentLife: return "GAME_SOURCE_OPPONENT_LIFE";
            case CardSource.OpponentDonDeck: return "GAME_SOURCE_OPPONENT_DONDECK";
            case CardSource.OpponentCostDeck: return "GAME_SOURCE_OPPONENT_COSTDECK";
            case CardSource.OpponentBoard: return "GAME_SOURCE_OPPONENT_BOARD";
            case CardSource.Character: return "";
            case CardSource.Leader: return "";
            case CardSource.OpponentCharacter: return "";
            case CardSource.OpponentLeader: return "";
        }

        return "none";
    }

    public static string GetTrKey(this CardAction action)
    {
        switch (action)
        {
            case CardAction.See: return "GAME_ACTION_SEE";
            case CardAction.Throw: return "GAME_ACTION_TRASH";
            case CardAction.Discard: return "GAME_ACTION_DISCARD";
            case CardAction.Attack: return "GAME_ACTION_ATTACK";
            case CardAction.Summon: return "GAME_ACTION_SUMMON";
        }

        return "none";
    }

    public static string GetTrKey(this RPSChoice rps)
    {
        switch (rps)
        {
            case RPSChoice.Rock: return "GAME_RPS_ROCK";
            case RPSChoice.Paper: return "GAME_RPS_PAPER";
            case RPSChoice.Scissors: return "GAME_RPS_SCISSORS";
        }

        return "none";
    }

    public static string GetTrKey(this PhaseType phase)
    {
        switch (phase)
        {
            case PhaseType.Refresh: return "PHASE_REFRESH";
            case PhaseType.Don: return "PHASE_DON";
            case PhaseType.Draw: return "PHASE_DRAW";
            case PhaseType.Main: return "PHASE_MAIN";
            case PhaseType.End: return "PHASE_END";
            case PhaseType.Opponent: return "PHASE_OPPONENT";
        }

        return "none";
    }

    public static string GetTrKeyDesc(this PhaseType phase)
    {
        switch (phase)
        {
            case PhaseType.Refresh: return "PHASE_REFRESH_DESC";
            case PhaseType.Don: return "PHASE_DON_DESC";
            case PhaseType.Draw: return "PHASE_DRAW_DESC";
            case PhaseType.Main: return "PHASE_MAIN_DESC";
            case PhaseType.End: return "PHASE_END_DESC";
            case PhaseType.Opponent: return "PHASE_OPPONENT_DESC";
        }

        return "none";
    }

    public static string GetNextTrKey(this PhaseType phase)
    {
        switch (phase)
        {
            case PhaseType.Refresh: return "NEXT_PHASE_REFRESH";
            case PhaseType.Don: return "NEXT_PHASE_DON";
            case PhaseType.Draw: return "NEXT_PHASE_DRAW";
            case PhaseType.Main: return "NEXT_PHASE_MAIN";
            case PhaseType.End: return "NEXT_PHASE_END";
            case PhaseType.Opponent: return "NEXT_PHASE_OPPONENT";
        }

        return "none";
    }

    public static CardResource GetCardResource(this PlayingCard card)
    {
        return CardManager.Instance.Cards.FirstOrDefault(x => x.Id == card.CardInfo.Id);
    }
}
