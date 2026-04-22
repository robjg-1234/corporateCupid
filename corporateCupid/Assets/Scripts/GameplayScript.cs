using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    [SerializeField] TMP_Text tutorial;
    [SerializeField] GameObject submit;
    [SerializeField] GameObject bossChat;
    [SerializeField] GameObject shred;
    [SerializeField] GameObject file;
    [SerializeField] GameObject envelope;
    [SerializeField] GameObject clock;
    [SerializeField] GameObject slot;
    public int currentProfiles = 0;
    string[] preferences = {"Movies","Astrology","Programming","Cars","Video Games","Trains","Winter","Travelling","Reading","Music","Social Events","Animals","Sports","Hiking","Cooking" };
    string[] maleNames = { "Liam","Noah", "Oliver","Elijah","William","James","Benjamin","Lucas","Henry","Alexander","Mason","Michael","Ethan","Daniel","Jacob","Logan","Jackson","Levi","Sebastian","John","Jack","Owen","Theodore","Aiden","Samuel"};
    string[] femaleNames = { "Olivia", "Emma", "Ava", "Charlotte", "Sophia", "Amelia", "Isabella", "Mia","Evelyn","Harper","Camila", "Abigail", "Gianna", "Luna", "Ella", "Elizabeth", "Sofia", "Emily", "Avery", "Mila", "Scarlett", "Eleanor", "Madison", "Layla", "Penelope" };
    string[] surnames = { "Smith", "Johnson", "Williams", "Brown","Jones","Garcia","Miller","Davis","Rodriguez","Martinez","Hernandez","Lopez","Gonzales","Wilson","Anderson","Thomas","Taylor","Moore","Jackson","Martin","Lee","Perez","Thompson","White","Harris" };
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
    public float money = 15;
    public float amountGained = 0;
    public int incorrectShreds = 0;
    public int dailyIncorrectShreds = 0;
    public float fees = 0;
    [NonSerialized] public bool stepDone = false;
    [NonSerialized] public int stepNumber = 0;
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

        int saveCheck = PlayerPrefs.GetInt("Save");
        int tutorialSkipped = PlayerPrefs.GetInt("SkipTutorial");
        if (saveCheck== 0)
        {
            SaveScript.Load();
        }
        if (tutorialSkipped == 1)
        {
            day = tutorialSkipped;
        }
    }
    private void Update()
    {
        //if (dayGoing)
        //{
        //    //if (Keyboard.current.pKey.wasPressedThisFrame)
        //    //{
        //    //    debugPaused = !debugPaused;
        //    //    Debug.Log("Time paused: " + debugPaused);
        //    //}
        //    //else if (Keyboard.current.oKey.wasPressedThisFrame)
        //    //{
        //    //    SummonProfiles();
        //    //}
        //}
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
        if (day == 0)
        {

            bool proceed = false;
            Image sprite = bossChat.GetComponent<Image>();
            yield return new WaitForSeconds(0.5f);
            //Text 1 "Since this is your first day at the office, you need to get up to speed. Drag your punch card onto the clock to start the day."
            tutorial.text = "Since this is your first day at the office, you need to get up to speed. Drag your punch card onto the clock to start the day.";
            bossChat.SetActive(true);
            clock.SetActive(true);
            float t = 0;
            while (!clockedIn)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            sprite.color = new Color(1, 1, 1, 1);
            clock.SetActive(false);
            proceed = false;
            //Click once
            //Text 2 "First, look through the profiles on the desk by right-clicking them. You can drag them around to reorder them as you please."
            tutorial.text = "First, check this profile on the desk by right-clicking them. You can drag it around as you please.";
            Vector3 pos = deskCenter + new Vector2(2.5f, 0); ;
            PaperScript example = null; 
            for (int i = 0; i < 2; i++)
            {
                example = Instantiate(profilePrefab, pos, Quaternion.identity).GetComponent<PaperScript>();
                pos.x += 0.1f;
            }
            yield return new WaitForEndOfFrame();
            cabinetRef.individualUnits[0].SaveProfile(example);
            //Right click on a profile and then click again
            t = 0;
            while (!stepDone)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            sprite.color = new Color(1, 1, 1, 1);
            stepDone = false;
            tutorial.text = "In each profile, you can see important information about our clients. Try to match profiles based on what they agree and disagree on.";
            //Text 3 "In each profile, you can see important information about our clients. Try to match profiles based on what they agree and disagree on."
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            //Click once
            tutorial.text = "The more traits they share, the better the match is. Since this is real life, perfect matches don't exist, so go for the second-best thing. Remember, time is of the essence!";
            //Text 4 "The more traits they share, the better the match is. Since this is real life, perfect matches don't exist, so go for the second-best thing. Remember, time is of the essence!"
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                    stepNumber++;
                }
                yield return null;
            }
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "Click on the file cabinet handle to open it and drag the profile on the first slot out. You can use this place to store up to 5 profiles.";
            //Text 4 "Click on the file cabinet handle to open it, here you can drop profiles for later access. Profiles inside the cabinet are not shredded at the end of the day and can be accessed the next day."
            //Click the handle, close it or click once
            file.SetActive(true);
            slot.SetActive(true);
            t = 0;
            while (!stepDone)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            stepDone = false;
            file.SetActive(false);
            slot.SetActive(false);
            tutorial.text = "To create matches drag the two profiles that you want to match onto the pinboard, and then drag an envelope from the stack and drop on top of them.";
            //Text 5 "To create matches drag the two profiles that you want to match onto the pinboard, and then drag an envelope from the stack and drop on top of them."
            envelope.SetActive(true);
            t = 0;
            while (!stepDone)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            envelope.SetActive(false);
            sprite.color = new Color(1, 1, 1, 1);
            stepDone = false;
            
            //Drop envelope.
            tutorial.text = "This action is irreversible but you can change the profiles before putting them inside the envelope.";
            //Text 6 "This action is irreversible but you can change the profiles before putting them inside the envelope."
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                    stepNumber++;
                }
                yield return null;
            }
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            //Click once
            tutorial.text = "Now you need to drag and drop the newly created match into the submission slot. You cannot make new matches until the match has been submitted.";
            //Text 7 "Now you need to drag and drop the newly created match into the submission slot. You cannot make new matches until the match has been submitted."
            submit.SetActive(true);
            t = 0;
            while (!stepDone)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            submit.SetActive(false);
            stepDone = false;
            //Submit the envelope
            tutorial.text = "The overall quality and quantity of your matches will be reflected on your paycheck at the end of the day, so try to get as much done within the day.";
            //Text 8 "The overall quality and quantity of your matches will be reflected on your paycheck at the end of the day, so try to get as much done within the day."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "Any profile that does not get matched or saved, gets shredded.";
            //Text 9 "There are two outcomes for profiles that are not matched by the end of the day: They get saved or they get shredded."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "Due to the popularity of our company. We receive profiles from lots of sources, including non-humans. However, your objective is to only match humans.";
            //Text 11 "Due to the popularity of our company. We receive profiles from lots of sources, including non-humans. However, your objective is to only match humans."
            //click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "If you look at this profile, you will notice some inconsistencies, which are explained in the employee handbook.";
            Instantiate(profilePrefab, pos, Quaternion.identity);
            //Text 12 "If you look at this profile, you will notice some inconsistencies, which are explained in the employee handbook."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "To solve this problem, grab the profile and drop it in the shredder, then press the button to shred it. Correctly shredding non-human profiles benefits your overall score, so keep an eye out.";
            //Text 13 "To solve this problem, grab the profile and drop it in the shredder, then press the button to shred it. Correctly shredding non-human profiles benefits your overall score, so keep an eye out."
            //Shred a profile
            shred.SetActive(true);
            t = 0;
            while (!stepDone)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            stepDone = false;
            shred.SetActive(false);
            tutorial.text = "If you fail to identify them and match them with a real person, it will affect our company reputation and it will be reflected on your score.";
            //Text 14 "If you fail to identify them and match them with a real person, it will affect our company reputation and it will be reflected on your score."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "Each day there will be 3 batches of profiles, spread out along the day. Each batch will happen only after some time has passed, so you can keep some profiles for the next batch.";
            //Text 15 "Each day there will be 3 batches of profiles, spread out along the day. Each batch will happen only after some time has passed, so you can keep some profiles for the next batch."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "After your day is done, you will be shown the end-of-day report, where you can pay for your mortal needs. Failing to do so may impact your perception of time.";
            //Text 16 "After your day is done, you will be shown the end-of-day report, where you can pay for your mortal needs. Failing to do so may impact your perception of time."
            //Click once
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            tutorial.text = "There will be a final test badge today so that you can put everything into practice, nothing from today will carry over tomorrow. I'll check on your progress with the company in 5 days.";
            //Text 17 "There will be a final test badge today so that you can put everything into practice, nothing from today will carry over tomorrow. I'll check on your progress with the company in 5 days."
            //Click once 5 profiles are dropped
            t = 0;
            while (!proceed)
            {
                t += Time.deltaTime;
                if (t > 5f)
                {
                    sprite.color = new Color(1, 1, 1, 0.5f);
                }
                if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    proceed = true;
                }
                yield return null;
            }
            yield return new WaitForSeconds(0.25f);
            sprite.color = new Color(1, 1, 1, 1);
            proceed = false;
            pos = deskCenter;
            bossChat.SetActive(false);
            for (int i = 0; i < 5; i++)
            {
                Instantiate(profilePrefab, pos, Quaternion.identity);
                pos.x += 0.1f;
            }
            int hour = 15;
            int minute = 0;
            int totalTime = 360;
            int minuteSecond = 0;
            SetTime(hour, minute);
            while (totalTime < 480)
            {
                if (!paused && !debugPaused)
                {
                    float increment = timeMultiplier * Time.deltaTime;
                    if (HandbookScript.instance.open)
                    {
                        increment *= 1 / 3;
                    }
                    time += increment;
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
                yield return null;
            }
            profileDrop.Clear();
            dayGoing = false;
            if (validScore > 0)
            {
                amountGained = 4;
                money += amountGained;
            }
            validScore = 0;
            StartCoroutine(fades.FadeOut());
        }
        else
        {
            while (!clockedIn)
            {
                yield return null;
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
                    float increment = timeMultiplier * Time.deltaTime;
                    if (HandbookScript.instance.open)
                    {
                        increment *= 1 / 3;
                    }
                    time += increment;
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
                    hour = 9 + Mathf.FloorToInt(totalTime / 60f);
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
                    if (day == 3)
                    {
                        batchSize = 40;
                    }
                }
                yield return null;
            }
            profileDrop.Clear();
            dayGoing = false;
            StartCoroutine(fades.FadeOut());
        }
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
                availableProfiles.Add(new ProfileScript("Maria Stuart", new() { ("Movies", 1), ("Anime", 3), ("Astrology", 1),("Hiking", -1), ("Cooking", -1), ("Trains", -1) }));
                availableProfiles.Add(new ProfileScript("Cupid Cupidson", new() { ("Movies", 2), ("Anime", 3), ("Social Events", 1), ("Cars", -1), ("Video Games", -2), ("Trains", -1) }));
                totalProfiles = 6;
                totalFakeProfiles = 1;
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
                batchSize = 18;
                totalFakeProfiles = 50;
                totalProfiles = 98;
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

                if (Random.Range(0, 1f) > 0.5f || day==0)
                {
                    newProfile = new(makeshiftName, chosenPreferences, 8);
                }
                else
                {
                    newProfile = new(makeshiftName, chosenPreferences, Random.Range(1, 8));
                }
                Debug.Log(makeshiftName);
            }
            else
            {
                newProfile = new(makeshiftName, chosenPreferences, 0);
            }

            availableProfiles.Add(newProfile);
        }
        if (day != 0)
        {
            ShuffleList(availableProfiles);
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
        
        SaveScript.Save();
        if (day == 6)
        {
            SaveScript.DeleteSaveFile();
        }
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

    public void Save(ref SaveData data)
    {
        data.currentday = day;
        data.money = money;
        data.score = overallScore;
        data.matches = profilesMatched;
        data.shreds = profilesShredded;
        data.incShreds = incorrectShreds;
        data.mult = timeMultiplier;
    }
    public void Load(SaveData data)
    {
        day = data.currentday;
        money = data.money;
        timeMultiplier = data.mult;
        overallScore = data.score;
        profilesMatched = data.matches;
        profilesShredded = data.shreds;
        incorrectShreds = data.incShreds;
    }
}
[Serializable]
public struct SaveData
{
    public int currentday;
    public float money;
    public int matches;
    public int shreds;
    public int incShreds;
    public float score;
    public float mult;
}