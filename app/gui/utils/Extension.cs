using Godot;
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

    public static string GetTrKey(this CardSelectorSource source)
    {
        switch (source)
        {
            case CardSelectorSource.Hand: return "GAME_SOURCE_HAND";
            case CardSelectorSource.Deck: return "GAME_SOURCE_DECK";
            case CardSelectorSource.Trash: return "GAME_SOURCE_TRASH";
            case CardSelectorSource.Life: return "GAME_SOURCE_LIFE";
            case CardSelectorSource.DonDeck: return "GAME_SOURCE_DONDECK";
            case CardSelectorSource.CostDeck: return "GAME_SOURCE_COSTDECK";
            case CardSelectorSource.Board: return "GAME_SOURCE_BOARD";
            case CardSelectorSource.OpponentHand: return "GAME_SOURCE_OPPONENT_HAND";
            case CardSelectorSource.OpponentDeck: return "GAME_SOURCE_OPPONENT_DECK";
            case CardSelectorSource.OpponentTrash: return "GAME_SOURCE_OPPONENT_TRASH";
            case CardSelectorSource.OpponentLife: return "GAME_SOURCE_OPPONENT_LIFE";
            case CardSelectorSource.OpponentDonDeck: return "GAME_SOURCE_OPPONENT_DONDECK";
            case CardSelectorSource.OpponentCostDeck: return "GAME_SOURCE_OPPONENT_COSTDECK";
            case CardSelectorSource.OpponentBoard: return "GAME_SOURCE_OPPONENT_BOARD";
        }

        return "none";
    }

    public static string GetTrKey(this CardSelectorAction action)
    {
        switch (action)
        {
            case CardSelectorAction.See: return "GAME_ACTION_SEE";
            case CardSelectorAction.Throw: return "GAME_ACTION_TRASH";
            case CardSelectorAction.Discard: return "GAME_ACTION_DISCARD";
            case CardSelectorAction.Attack: return "GAME_ACTION_ATTACK";
            case CardSelectorAction.Summon: return "GAME_ACTION_SUMMON";
        }

        return "none";
    }
}
