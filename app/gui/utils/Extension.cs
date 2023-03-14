using Godot;
using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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

        directory.ListDirBegin();
        string file;
        while((file = directory.GetNext()) != null)
        {
            if (string.IsNullOrEmpty(file))
            {
                break;
            }

            if (!directory.CurrentIsDir() && regex.IsMatch(file))
            {
                files.Add(Path.Combine(directory.GetCurrentDir(), file));
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
}
