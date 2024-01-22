using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace AppShearLagEffect.Utils;

public static class ConvertUtil
{
    public static string ArrayToString<T>(T[] array, char separator = '+') where T : struct
    {
        if (array is null || array.Length == 0)
            return string.Empty;
        return string.Join(separator, array.Select(x =>
        {
            return Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Double => ((double)(object)x).ToString("0"),
                _ => x.ToString()
            };
        }));
    }

    public static byte[] ArrayToBytes<T>(T[] array) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var result = new byte[array.Length * size];
        for (int i = 0; i < array.Length; i++)
        {
            var item = array[i];
            var bytes = Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Boolean => BitConverter.GetBytes((bool)(object)item),
                TypeCode.Char => BitConverter.GetBytes((char)(object)item),
                TypeCode.Int32 => BitConverter.GetBytes((Int32)(object)item),
                TypeCode.Int64 => BitConverter.GetBytes((Int64)(object)item),
                TypeCode.Double => BitConverter.GetBytes((double)(object)item),
                _ => throw new NotImplementedException(),
            };
            Array.Copy(bytes, 0, result, i * size, size);
        }
        return result;
    }

    public static T[] StringToArray<T>(string str, char separator = '+') where T : struct
    {
        if (string.IsNullOrWhiteSpace(str))
            return Array.Empty<T>();
        var items = str.Split(separator);
        var result = new T[items.Length];
        for (var i = 0; i < result.Length; i++)
        {
            result[i] = Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Boolean => (T)(object)Convert.ToBoolean(items[i]),
                TypeCode.Char => (T)(object)Convert.ToChar(items[i]),
                TypeCode.Int32 => (T)(object)Convert.ToInt32(items[i]),
                TypeCode.Int64 => (T)(object)Convert.ToUInt64(items[i]),
                TypeCode.Double => (T)(object)Convert.ToDouble(items[i]),
                _ => throw new NotImplementedException()
            };
        }
        return result;
    }

    public static T[] BytesToArray<T>(byte[] bytes) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        if (bytes.Length % size != 0)
        {
            throw new ArgumentException("bytes的长度必须" + size + "的是整数倍");
        }
        var result = new T[bytes.Length / size];
        for (var i = 0; i < result.Length; i++)
        {
            var segment = new ArraySegment<byte>(bytes, i * size, size);
            var item = Type.GetTypeCode(typeof(T)) switch
            {
                TypeCode.Boolean => (T)(object)BitConverter.ToBoolean(segment.ToArray(), 0),
                TypeCode.Char => (T)(object)BitConverter.ToChar(segment.ToArray(), 0),
                TypeCode.Int32 => (T)(object)BitConverter.ToInt32(segment.ToArray(), 0),
                TypeCode.Int64 => (T)(object)BitConverter.ToInt64(segment.ToArray(), 0),
                TypeCode.Double => (T)(object)BitConverter.ToDouble(segment.ToArray(), 0),
                _ => throw new NotImplementedException()
            };
            result[i] = item;
        }
        return result;
    }

}
