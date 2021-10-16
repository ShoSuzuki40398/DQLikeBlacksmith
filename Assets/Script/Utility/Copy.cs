using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Copy
{
    //--------------------------------------------------------------------------------
    // 引数に渡したオブジェクトをディープコピーしたオブジェクトを生成して返す
    // ジェネリックメソッド版
    //--------------------------------------------------------------------------------
    public static T DeepCopy<T>(T target)
    {
        T result;
        BinaryFormatter b = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();

        try
        {
            b.Serialize(mem, target);
            mem.Position = 0;
            result = (T)b.Deserialize(mem);
        }
        finally
        {
            mem.Close();
        }

        return result;
    }
    // 拡張メソッド版
    public static object DeepCopy(this object target)
    {
        object result;
        BinaryFormatter b = new BinaryFormatter();
        MemoryStream mem = new MemoryStream();

        try
        {
            b.Serialize(mem, target);
            mem.Position = 0;
            result = b.Deserialize(mem);
        }
        finally
        {
            mem.Close();
        }

        return result;
    }
}
