using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
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
    [SerializeField] GameObject pauseMenu;
    [NonSerialized] public static GameplayScript instance;
    [NonSerialized] public static Player player;
    [NonSerialized] public static SubmitScript mailInstance;
    [NonSerialized] public static ShredderScript shredInstance;
    public int currentProfiles = 0;
    string[] preferences = {"Movies","Astrology","Programming","Cars","Video Games","Trains","Winter","Travelling","Reading","Music","Social Events","Animals","Sports","Hiking","Cooking" };
    string[] maleNames = { "Liam","Noah", "Oliver","Elijah","William","James","Benjamin","Lucas","Henry","Alexander","Mason","Michael","Ethan","Daniel","Jacob","Logan","Jackson","Levi","Sebastian","John","Jack","Owen","Theodore","Aiden","Samuel"};
    string[] femaleNames = { "Olivia", "Emma", "Ava", "Charlotte", "Sophia", "Amelia", "Isabella", "Mia","Evelyn","Harper","Camila", "Abigail", "Gianna", "Luna", "Ella", "Elizabeth", "Sofia", "Emily", "Avery", "Mila", "Scarlett", "Eleanor", "Madison", "Layla", "Penelope" };
    string[] surnames = { "Smith", "Johnson", "Williams", "Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez","Hernandez","Lopez","Gonzales","Wilson","Anderson","Thomas","Taylor","Moore","Jackson","Martin","Lee","Perez","Thompson","White","Harris" };
    //List<ProfileScript> People = new List<ProfileScript>();
    //Queue<ProfileScript> availableProfiles = new Queue<ProfileScript>();
    List<ProfileScript> availableProfiles = new List<ProfileScript>();
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
    public float validScore = 0;
    public int money = 15;
    //Allows for dynamic time manipulation higher equals faster
    bool jumpToNext = false;
    public float timeMultiplier;
    float time = 0;
    int batchSize = 5;
    [NonSerialized] public bool paused = false;
    bool debugPaused = false;
    //Contains the moments for the batch drops of the day: IGT seconds
    List<int> profileDrop = new(0);

    void Start()
    {
        instance = this;
    }
    private void Update()
    {
        if (dayGoing)
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                debugPaused = !debugPaused;
                Debug.Log("Time paused: " + debugPaused);
            }
            else if (Keyboard.current.oKey.wasPressedThisFrame)
            {
                SummonProfiles();
            }
        }
    }
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
    /// Speeds up the game until the next avaialble stage.
    /// </summary>
    public void JumpToNextStage()
    {
        if (day != 0)
        {
            if (currentProfiles < 3 && !paused && clockedIn && dayGoing)
            {
                if (profileDrop.Count > 0)
                {
                    jumpToNext = true;
                }
            }
        }
    }
    /// <summary>
    /// Chooses the profiles attached to the letters.
    /// </summary>
    public ProfileScript PickProfile()
    {
        currentProfiles += 1;
        ProfileScript temp = availableProfiles[0];
        availableProfiles.Remove(temp);
        return temp;
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
            //Debug function
            if (!paused && !debugPaused)
            {
                time += timeMultiplier * Time.deltaTime;
            }
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
            if (jumpToNext)
            {
                jumpToNext = false;
                totalTime = profileDrop[0];
                minuteSecond = 0;
                hour = 9+Mathf.FloorToInt(totalTime / 60f);
                minute = totalTime - (hour - 9) * 60;
                SetTime(hour, minute);
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
        //Money calculation
        if (validScore > 0)
        {
            int amountGained = Mathf.RoundToInt(validScore*1.3f);
            Debug.Log("Money Gained: " + amountGained);
            money += amountGained;
            Debug.Log("Total money: " + money);
        }
        validScore = 0;
        overallScore += dailyScore;
        profilesMatched += dailyMatch;
        StartCoroutine(fades.FadeOut());
    }
    /// <summary>
    /// Create profiles and setup day characteristics (waves and time)
    /// </summary>
    void FetchDay()
    {
        int previousLink = -1;
        int totalFakeProfiles = 0;
        int totalProfiles = 12;
        switch (day)
        {
            case 0:
                batchSize = 5;
                profileDrop.Add(0);
                profileDrop.Add(240);
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 1),("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 2), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 2), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 3), ("Astrology", 2), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 1), ("Cars", -2), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 2), ("Cars", -2), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 2), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -3), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 1), ("Astrology", 1), ("Cars", -1), ("Video Games", -1), ("Trains", -3) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 3), ("Anime", 2), ("Astrology", 3), ("Cars", -1), ("Video Games", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 2), ("Astrology", 1), ("Cars", -1), ("Video Games", -3), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 1), ("Anime", 2), ("Astrology", 3), ("Cars", -3), ("Video Games", -1), ("Trains", -1) }));
                break;
            case 1:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 5;
                totalFakeProfiles = 0;
                totalProfiles = 15;
                break;
            case 2:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 8;
                totalFakeProfiles = 3;
                totalProfiles = 24;                
                break;
            case 3:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 32;
                totalFakeProfiles = 50;
                totalProfiles = 96;
                break;
            case 4:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 12;
                totalFakeProfiles = 5;
                totalProfiles = 36;
                break;
            case 5:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 25;
                totalFakeProfiles = 15;
                totalProfiles = 75;
                break;
            default:
                profileDrop.Add(0);
                profileDrop.Add(120);
                profileDrop.Add(280);
                batchSize = 5;
                totalProfiles = 15;
                break;
        }
        for (int i = 0; i < totalProfiles; i++)
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
            }
            else
            {
                previousLink = -1;
            }
            ProfileScript newProfile;
            if (totalFakeProfiles > 0)
            {
                totalFakeProfiles--;
                if (Random.Range(0, 1f) > 0.5f)
                {
                    newProfile = new(makeshiftName, chosenPreferences, 6);
                }
                else
                {
                    newProfile = new(makeshiftName, chosenPreferences, Random.Range(1, 6));
                }
                Debug.Log(makeshiftName);
            }
            else
            {
                newProfile = new(makeshiftName, chosenPreferences, 0);
            }

            availableProfiles.Add(newProfile);
        }
        ShuffleList(availableProfiles);
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
    /// <summary>
    /// Debug tool to force spawn 5 profiles
    /// </summary>
    void SummonProfiles()
    {
        int previousLink = -1;
        for (int i = 0; i < 5; i++)
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
            ProfileScript newProfile = new(makeshiftName, chosenPreferences);
            availableProfiles.Add(newProfile);
        }
        Vector3 pos = deskCenter;
        for (int i = 0; i < batchSize; i++)
        {
            Instantiate(profilePrefab, pos, Quaternion.identity);
            pos.x += 0.1f;
        }
    }

    public void ExitToTitleScreen()
    {
        SceneManager.LoadScene(0);
        //Fade out
    }

    /// <summary>
    /// Randomizes the order of the profiles that are currently available for draw.
    /// </summary>
    List<ProfileScript> ShuffleList(List<ProfileScript> current)
    {
        ProfileScript target;
        ProfileScript old;
        int max = current.Count;
        for (int i = 0; i < max; i++)
        {
            int newPos = Random.Range(0, max);
            target = current[newPos];
            old = current[i];
            current[i] = target;
            current[newPos] = old;
        }
        return current;
    }
    public void PauseGame()
    {
        if (dayGoing)
        {
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                if (paused == true)
                {
                    paused = false;
                }
            }
            else
            {
                pauseMenu.SetActive(true);
                if (paused == false)
                {
                    paused = true;
                }
            }
        }
    }
}