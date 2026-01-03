using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static readonly string file = Path.Combine(Application.persistentDataPath, "save");

    public static readonly string keyword = "administrator-developer-password-2026";

    public static string dataString = string.Empty;

    public static void Save(ref Player player)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(file));
        dataString = JsonUtility.ToJson(new PlayerData(player));
        dataString = EncryptDecryptData(dataString);
        using FileStream stream = new(file, FileMode.Create);
        using StreamWriter writer = new(stream);
        writer.Write(dataString);
    }

    public static PlayerData Load()
    {
        if (File.Exists(file))
        {
            using FileStream stream = new(file, FileMode.Open);
            using StreamReader reader = new(stream);
            dataString = reader.ReadToEnd();
            dataString = EncryptDecryptData(dataString);
            PlayerData data = JsonUtility.FromJson<PlayerData>(dataString);
            return data;
        }
        else
        {
            Debug.LogError("The file does not exist.");
            return null;
        }
    }

    public static void Delete()
    {
        if (File.Exists(file))
        {
            File.Delete(file);
        }
    }

    public static string EncryptDecryptData(string data)
    {
        string result = string.Empty;

        for (int i = 0; i < data.Length; i++)
        {
            // XOR Cipher
            result += (char)(data[i] ^ (keyword[i % keyword.Length]));
        }

        return result;
    }
}
