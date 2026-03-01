using UnityEngine;
using System.IO;
using System;

public static class SaveSystem
{
    public static readonly string file = Path.Combine(Application.persistentDataPath, "save");

    public static readonly string keyword = "$administrator-developer-password-2026$";

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
        try
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Cannot locate save file");
            }

            using FileStream stream = new(file, FileMode.Open);
            using StreamReader reader = new(stream);
            dataString = reader.ReadToEnd();
            dataString = EncryptDecryptData(dataString);
            PlayerData data = JsonUtility.FromJson<PlayerData>(dataString);
            return data;
        }
        catch (FileNotFoundException exception)
        {
            Debug.LogException(exception);
            return null;
        }
    }

    public static void Delete()
    {
        try
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException("Cannot locate save file.");
            }

            File.Delete(file);
        }
        catch (FileNotFoundException exception)
        {
            Debug.LogException(exception);
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
