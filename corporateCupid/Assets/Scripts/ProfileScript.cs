using System;
using System.Collections.Generic;
using UnityEngine;

public class ProfileScript
{
    string characterName;
    int age;
    string profession;
    List<(string, int)> preferences;
    public ProfileScript(string newName, List<(string, int)> newPreferences)
    {
        characterName = newName;
        age = 22;
        profession = "Bell Ringer";
        preferences = newPreferences;
        for (int i = 0; i < preferences.Count; i++)
        {
            Debug.Log(preferences[i].Item1 + " " + preferences[i].Item2);
        }
    }
    public List<(string, int)> GetPreferences()
    {
        return preferences;
    }
}
