using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameplayScript : MonoBehaviour
{
    //[SerializeField] GameObject pause;
    [SerializeField] GameObject profilePrefab;
    [SerializeField] GameObject clockSprite;
    [SerializeField] Sprite[] circleStages;
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
    public int day = 0;
    public int profilesMatched = 0;
    public int profilesShredded = 0;
    public float overallScore = 0;
    float time = 0;
    int batchSize = 5;
    int[] scheduledDrops = new int[] { -1,-1,-1,-1,-1,-1, -1};

    


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
        int stage = 0;
        SpriteRenderer clockSprite = this.clockSprite.GetComponent<SpriteRenderer>();
        while (stage < 6)
        {
            time += Time.deltaTime;
            //Control time for when we add pauses which would stop the in-game timer.
            //Current timer lasts 8 minutes.
            if (time > 80f)
            {
                time = 0;
                stage++;
            }
            if (scheduledDrops[stage] == 1)
            {
                scheduledDrops[stage] = -1;
                Vector3 pos = deskCenter;
                for (int i = 0; i < batchSize; i++)
                {
                    Instantiate(profilePrefab, pos, Quaternion.identity);
                    pos.x += 0.1f;
                }
            }
            clockSprite.sprite = circleStages[stage];
            yield return null;
        }
        clockSprite.sprite = circleStages[stage];
        dayGoing = false;
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
                scheduledDrops[0] = 1;
                scheduledDrops[3] = 1; 
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
                scheduledDrops[0] = 1;
                scheduledDrops[2] = 1;
                scheduledDrops[4] = 1;
                batchSize = 5;
                for (int i = 0; i < 15; i++)
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
                    ProfileScript newProfile = new(makeshiftName, RandomizePreferences());
                    
                    availableProfiles.Enqueue(newProfile);
                }
                break;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.lightGoldenRod;
        Gizmos.DrawWireCube(deskCenter, size);
    }
}