using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameplayScript : MonoBehaviour
{
    [SerializeField] GameObject pause;
    [SerializeField] GameObject profilePrefab;
    [SerializeField] GameObject clockSprite;
    [SerializeField] Sprite[] circleStages;
    public static GameplayScript instance;
    public static Player player;
    public static SubmitScript mailInstance;
    public int currentProfiles = 0;
    string[] preferences = {"Food", "Fashion", "Alcohol", "Literature", "Philosophy", "Maths", "Music", "Astrology", "Movies", "Anime", "Bugs", "Games", "Partying", "Long Walk on the Beach" };
    string[] names = {"Cupid"};
    string[] lastNames = { "Cupidson" };
    List<ProfileScript> People = new List<ProfileScript>();
    Queue<ProfileScript> availableProfiles = new Queue<ProfileScript>();
    public Action<int, int> objectInteracted;

    //Day and player information
    public bool dayGoing = false;
    int day = 0;
    public int profilesMatched = 0;
    int profilesShredded = 0;
    public float overallScore = 0;
    float time = 0;
    int batchSize = 5;
    int waves = 3;

    


    void Start()
    {
        instance = this;
        // --Temp--
        for (int i = 0; i < 21;i++)
        {
            ProfileScript newProfile= new("Cupid Cupdison", RandomizePreferences());
            People.Add(newProfile);
            availableProfiles.Enqueue(newProfile);
        }
        //Randomize choosing

        //Possibly generate a list of possible Profiles at the beginning of the day in queue format so that we can introduce "Scripted Profiles"
    }
    private void Update()
    {
        if (!dayGoing)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                StartCoroutine(DayControl());
            }
        }
    }
    /// <summary>
    /// A global call to update rendering priority.
    /// </summary>
    public void CallInteraction(int targetPrev)
    {
        if (objectInteracted != null)
        {
            objectInteracted(targetPrev,currentProfiles);
        }
    }

    /// <summary>
    /// Randomly selects Preferences and assigns a value, three positives and three negatives.
    /// </summary>
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

    /// <summary>
    /// Checks if an integer array contains a specific value and returns true if it does.
    /// </summary>
    bool ContainsNumber(int[] array, int num)
    {
        bool found = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (num == array[i])
            {
                found = true;
                break;
            }
        }
        return found;
    }
    /// <summary>
    /// Chooses the profiles attached to the letters.
    /// </summary>
    public ProfileScript PickProfile()
    {
        currentProfiles += 1;
        return availableProfiles.Dequeue();
    }

    /// <summary>
    /// Coroutine that handles the day cycle, including batch drops, and displaying the score.
    /// </summary>
    System.Collections.IEnumerator DayControl()
    {
        pause.SetActive(false);
        int stage = 0;
        SpriteRenderer clockSprite = this.clockSprite.GetComponent<SpriteRenderer>();
        dayGoing = true;
        while (stage < 6)
        {
            if (stage == 0 || stage == 3)
            {
                Vector3 pos = new Vector3(-1, -2, 0);
                for (int i = 0; i < batchSize; i++)
                {
                    Instantiate(profilePrefab, pos, Quaternion.identity);
                    pos.x += 0.1f;
                }
            }
            clockSprite.sprite = circleStages[stage];
            stage += 1;
            yield return new WaitForSeconds(60);
            Debug.Log("Hour passes");
        }
        clockSprite.sprite = circleStages[stage];
        dayGoing = false;
        if (profilesMatched > 0)
        {
            Debug.Log(overallScore / profilesMatched);
        }
        else
        {
            Debug.Log("No matches");
        }
            pause.SetActive(true);
    }
}