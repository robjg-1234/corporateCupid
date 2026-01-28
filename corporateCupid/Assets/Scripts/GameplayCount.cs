using System.Collections.Generic;
using UnityEngine;

public class GameplayCount : MonoBehaviour
{
    string[] preferences = {"Food", "Fashion", "Alcohol", "Literature", "Philosophy", "Maths", "Music", "Astrology", "Movies", "Anime", "Bugs", "Games", "Partying", "Long Walk on the Beach" };
    List<ProfileScript> People = new List<ProfileScript>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 2;i++)
        {
            ProfileScript newProfile = new ProfileScript();
            newProfile.CreateProfile("Test", RandomizePreferences());
            People.Add(newProfile);
        }
        float val = ScoringScript.CalculateScores(People[0].GetPreferences(), People[1].GetPreferences());
        Debug.Log(val);
        //Randomize choosing
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<(string, int)> RandomizePreferences()
    {
        List<(string, int)> newList = new List<(string, int)>();
        int[] selection = new int[] { -1,-1,-1,-1,-1,-1};
        for (int i = 0; i<3;i++)
        {
            int newSelection = Random.Range(0, preferences.Length);
            while (ContainsNumber(selection, newSelection))
            {
                newSelection++;
                if (newSelection == preferences.Length)
                {
                    newSelection = 0;
                }
            }
            int quality =  Random.Range(1, 4);
            newList.Add((preferences[newSelection],quality));
            selection[i] = newSelection;
        }
        for (int i = 3; i < 6; i++)
        {
            int newSelection = Random.Range(0, preferences.Length);
            while (ContainsNumber(selection, newSelection))
            {
                newSelection++;
                if (newSelection == preferences.Length)
                {
                    newSelection = 0;
                }
            }
            int quality = Random.Range(-3, 0);
            newList.Add((preferences[newSelection], quality));
            selection[i] = newSelection;
        }
        return newList;
    }

    bool ContainsNumber(int[] myArray, int num)
    {
        bool found = false;
        for (int i = 0; i <myArray.Length; i++)
        {
            if (num == myArray[i])
            {
                found = true;
                break;
            }
        }
        return found;
    }
}