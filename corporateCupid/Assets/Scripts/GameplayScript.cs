using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameplayScript : MonoBehaviour
{
    //[SerializeField] GameObject pause;
    [SerializeField] GameObject profilePrefab;
    [SerializeField] TMP_Text clockTime;
    [SerializeField] public Vector2 deskCenter;
    [SerializeField] public Vector2 size;
    [SerializeField] CabinetScript cabinetRef;
    [SerializeField] IntermissionScript fades;
    public static GameplayScript instance;
    public static Player player;
    public static SubmitScript mailInstance;
    public int currentProfiles = 0;
    string[] preferences = {"Movies","Astrology","Programming","Cars","Video Games","Trains","Winter","Travelling","Reading","Music","Social Events","Animals","Sports","Hiking","Cooking" };
    string[] maleNames = { "Liam","Noah", "Oliver","Elijah","William","James","Benjamin","Lucas","Henry","Alexander","Mason","Michael","Ethan","Daniel","Jacob","Logan","Jackson","Levi","Sebastian","John","Jack","Owen","Theodore","Aiden","Samuel"};
    string[] femaleNames = { "Olivia", "Emma", "Ava", "Charlotte", "Sophia", "Amelia", "Isabella", "Mia","Evelyn","Harper","Camila", "Abigail", "Gianna", "Luna", "Ella", "Elizabeth", "Sofia", "Emily", "Avery", "Mila", "Scarlett", "Eleanor", "Madison", "Layla", "Penelope" };
    string[] surnames = { "Smith", "Johnson", "Williams", "Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez","Hernandez","Lopez","Gonzales","Wilson","Anderson","Thomas","Taylor","Moore","Jackson","Martin","Lee","Perez","Thompson","White","Harris" };
    //List<ProfileScript> People = new List<ProfileScript>();
    Queue<ProfileScript> availableProfiles = new Queue<ProfileScript>();
    public Action<int, int> objectInteracted;
    public Action dayEnded;

    //Day and player information
    public bool clockedIn = false;
    public bool dayGoing = false;
    public int day;
    public int profilesMatched = 0;
    public int profilesShredded = 0;
    public float overallScore = 0;
    public float dailyScore = 0;
    public int dailyMatch = 0;
    public int dailyShred = 0;
    //Allows for dynamic time manipulation higher equals faster
    public float timeMultiplier;
    float time = 0;
    int batchSize = 5;
    //Contains the moments for the batch drops of the day: IGT seconds
    List<int> profileDrop = new(0);

    


    void Start()
    {
        instance = this;
        // --Temp--
        
        //Randomize choosing
    }
    //private void Update()
    //{
    //    if (!dayGoing)
    //    {
    //        if (Keyboard.current.spaceKey.wasPressedThisFrame)
    //        {
    //            StartCoroutine(DayControl());
    //        }
    //    }
    //}
    /// <summary>
    /// A global call to update rendering priority.
    /// </summary>
    public void CallInteraction(int targetPrev)
    {
        objectInteracted?.Invoke(targetPrev, currentProfiles);
    }

    /// <summary>
    /// Randomly selects Preferences and assigns a value, three positives and three negatives.
    /// </summary>
    List<(string, int)> RandomizePreferences(int forceLike = -1)
    {
        List<(string, int)> newList = new List<(string, int)>();
        int[] selection = new int[] { -1,-1,-1,-1,-1,-1};
        for (int i = 0; i<3;i++)
        {
            int newSelection = Random.Range(0, preferences.Length);
            if (forceLike > -1)
            {
                newSelection = forceLike;
                forceLike = -1;
            }
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
    public void SetTime(int hour, int minute)
    {
        string tempText = "";
        if (hour > 9)
        {
            tempText = hour + ":";
        }
        else
        {
            tempText = "0" + hour + ":";
        }
        if (minute > 9)
        {
            tempText += minute.ToString();
        }
        else
        {
            tempText += "0" + minute;
        }
        clockTime.text = tempText;
    }
    /// <summary>
    /// Coroutine that handles the day cycle, including batch drops, and displaying the score.
    /// </summary>
    public System.Collections.IEnumerator DayControl()
    {
        //Add a fade into desk space and wait for clock in card to be placed.
        //Temp: creates profiles.
        //Fetch day
        
        FetchDay();
        dayGoing = true;
        while (!clockedIn)
        {
            yield return null;
        }
        if (day == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                PaperScript pfRef = Instantiate(profilePrefab, deskCenter, Quaternion.identity).GetComponent<PaperScript>();
                yield return new WaitForEndOfFrame();
                cabinetRef.individualUnits[i].SaveProfile(pfRef);
            }
        }
        Debug.Log("Clocked In");
        //Oh no!!!!!!!, the table! It's broken!
        int hour = 9;
        int minute = 0;
        int totalTime = 0;
        int minuteSecond = 0;
        //Control time for when we add pauses which would stop the in-game timer.
        //Current timer lasts 8 minutes.
        while (totalTime < 480)
        {
            time += timeMultiplier*Time.deltaTime;
            if (time > 1)
            {
                minuteSecond += 1;
                if (minuteSecond > 9)
                {
                    minute += 10;
                    minuteSecond = 0;
                }
                totalTime += 1;
                
                if (minute > 59)
                {
                    hour += 1;
                    minute = 0;
                }
                SetTime(hour, minute);
                time = 0;
            }
            if (profileDrop.Contains(totalTime))
            {
                profileDrop.Remove(totalTime);
                Vector3 pos = deskCenter;
                for (int i = 0; i < batchSize; i++)
                {
                    Instantiate(profilePrefab, pos, Quaternion.identity);
                    pos.x += 0.1f;
                }
            }
            yield return null;
        }
        profileDrop.Clear();
        dayGoing = false;
        overallScore += dailyScore;
        profilesMatched += dailyMatch;
        //Part of the game that will handle end of day report.
        if (profilesMatched > 0)
        {
            Debug.Log(overallScore / profilesMatched);
        }
        else
        {
            Debug.Log("No matches");
        }
        //Add fade out to results
        //pause.SetActive(true);
        StartCoroutine(fades.FadeOut());
    }
    /// <summary>
    /// Create profiles and setup day characteristics (waves and time)
    /// </summary>
    void FetchDay()
    {
        switch (day)
        {
            case 0:
                batchSize = 5;
                profileDrop.Add(0);
                profileDrop.Add(240);
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 1),("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 2), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 2), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 3), ("Astrology", 2), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 1), ("Cars", -2), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 2), ("Cars", -2), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 2), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -3), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -3) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 2), ("Astrology", 3), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 2), ("Astrology", 1), ("Cars", -1), ("Video Games", -3), ("Trains", -1) }));
                availableProfiles.Enqueue(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 2), ("Astrology", 3), ("Cars", -3), ("Video Games", -1), ("Trains", -1) }));
                break;
            default:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 7;
                int previousLink = -1;
                for (int i = 0; i < 21; i++)
                {
                    string makeshiftName = "";
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        rand = Random.Range(0, maleNames.Length);
                        makeshiftName += maleNames[rand] + " ";
                    }
                    else
                    {
                        rand = Random.Range(0, femaleNames.Length);
                        makeshiftName += femaleNames[rand] + " ";
                    }
                    rand = Random.Range(0, surnames.Length);
                    makeshiftName += surnames[rand];
                    List<(string, int)> chosenPreferences = RandomizePreferences(previousLink);
                    int j = 0;
                    int randomChoice = Random.Range(0, 3);
                    if (Random.Range(0, 1f) <= 0.4f)
                    {
                        for (j = 0; j < preferences.Length; j++)
                        {
                            if (preferences[j].Equals(chosenPreferences[randomChoice].Item1))
                            {
                                previousLink = j;
                                break;
                            }
                        }
                        Debug.Log("" + chosenPreferences[randomChoice].Item1 + " : " + makeshiftName);
                    }
                    else
                    {
                        previousLink = -1;
                    }
                    ProfileScript newProfile = new(makeshiftName,chosenPreferences);
                    
                    
                    availableProfiles.Enqueue(newProfile);
                }
                break;
        }
    }

    public Vector2 ReturnToDesk(Vector3 originalPos)
    {
        float top = deskCenter.y + size.y / 2f;
        float bottom = deskCenter.y - size.y / 2f;
        float left = instance.deskCenter.x - size.x / 2f;
        float right = deskCenter.x + size.x / 2f;
        if (originalPos.x < left)
        {
            originalPos.x = left;
        }
        if (originalPos.x > right)
        {
            originalPos.x = right;
        }
        if (originalPos.y < bottom)
        {
            originalPos.y = bottom;
        }
        if (originalPos.y > top)
        {
            originalPos.y = top;
        }
        return originalPos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lightGoldenRod;
        Gizmos.DrawWireCube(deskCenter, size);
    }
}