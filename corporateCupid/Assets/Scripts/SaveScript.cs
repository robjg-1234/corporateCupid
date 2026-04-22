using UnityEngine;
using System.IO;
using System;

public class SaveScript
{
    public static SaveInfo _saveData = new SaveInfo();
    [Serializable]
    public struct SaveInfo
    {
        public SaveData PlayerData;
    }

    public static string GetSaveFilePath()
    {
        string path = Application.persistentDataPath + "/data"+".player";
        return path;
    }

    public static bool SaveFileExists()
    {
        return File.Exists(GetSaveFilePath());
    }

    public static void DeleteSaveFile()
    {
        File.Delete(GetSaveFilePath());
    }
    public static void Save()
    {
        SaveFile();
        File.WriteAllText(GetSaveFilePath(), JsonUtility.ToJson(_saveData));
    }
    private static void SaveFile()
    {
        GameplayScript.instance.Save(ref _saveData.PlayerData);
    }

    public static void Load()
    {
        string fileContent = File.ReadAllText(GetSaveFilePath());
        _saveData = JsonUtility.FromJson<SaveInfo>(fileContent);
        LoadFile();
    }
    private static void LoadFile()
    {
        GameplayScript.instance.Load(_saveData.PlayerData);
    }
}
