
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Conatins all the information of the profile.
/// </summary>
public class ProfileScript
{
    public int profileType = 0;
    bool visibleRobot = false;
    public string characterName;
    int age;
    string profession;
    string bio;
    List<(string, int)> preferences;
    readonly string[] loveStrings = { "I’m a big lover of X","X is life","If you don’t like X we won’t get along","Can’t live without X"};
    readonly string[] likesStrings = { "Big fan of X","I really like X","I quite enjoy X","First date? X"};
    readonly string[] slightLikeStrings = { "I’m fairly fond of X","Can’t go wrong with X","Will never say no to a day full of X","X isn’t as bad as people make it out to be" };
    readonly string[] hatesStrings = { "Can’t stand X", "The world  would be a better place without X", "Absolutely despise X", "Professional X hater" };
    readonly string[] dislikesStrings = { "X is overrated","Won’t understand if you genuinely like X","Not a fan of X","I don’t enjoy X"};
    readonly string[] slightDisikeStrings = {"Would prefer if you don’t like X","I never fully enjoyed X","Can’t get a taste for X","X is kind of ‘meh’ in my books"};
    readonly string[] bios = {"Looking for a Pam to my Jim.","My mum thinks I’m handsome.","6’3 if that matters.","Just a Ross looking for his Rachel.","Fun fact about me: I’ve never been to IKEA.","Looking for someone who’s the same kind of weird.",
        "Just looking for a good time.","Looking for a mum for my dog.","Fluent in sarcasm.","NO DRAMA!!","Don’t take yourself too seriously.","Unpopular opinion: Pineapple belongs on pizza. Fight me.",
        "Will judge your coffee order.","Will destroy you at Mario Kart.","Star Wars > Star Trek.","Let’s lie and say we met at the grocery store.","Professional overthinker.","Looking for a partner in crime.",
        "Here for a good time, not a long time","I sleep with my socks on. Deal with it.","My therapist thinks I’m a catch.","Toxic in a way you love.","Looking for a small spoon.",
        "Work hard, play hard.","Chivalry is dead.","The quickest way to my heart is through my stomach.","Will always laugh at your jokes.","Looking for my Prince Charming.","Daddy’s girl.","Treat me like a queen and I’ll treat you like a king.",
        "Fries > Guys.","Hufflepuff girl looking for her Gryffindor man."};
    readonly string[] dislikeEmotes = { ". <font=\"CcEmotionSDF\"><color=#FAB525><size=200%>|</size></color></font> " , ". <font=\"CcEmotionSDF\"><color=#FA9125><size=200%>}</size></color></font> ", ". <font=\"CcEmotionSDF\"><color=#D52915><size=200%>{</size></color></font> " };
    readonly string[] likeEmotes = { ". <font=\"CcEmotionSDF\"><color=#a1cc14><size=200%>\\</size></color></font> ", ". <font=\"CcEmotionSDF\"><color=#47910a><size=200%>]</size></color></font> ", ". <font=\"CcEmotionSDF\"><color=#3e6320><size=200%>[</size></color></font> " };
    readonly string[] jobs = { "Doctor", "Teacher", "Engineer", "Nurse", "Architect", "Lawyer", "Chef", "Accountant", "Pharmacist", "Journalist", "Police Officer", "Firefighter", "Software Developer","Electrician","Plumber","Pilot","Graphic Designer","Psychologist", "Scientist","Soical Worker"};
    public ProfileScript(string newName, List<(string, int)> newPreferences, int type =0)
    {
        characterName = newName;
        age = Random.Range(18,35);
        //Pharloom Reference :O
        profileType = type;
        profession = jobs[Random.Range(0,20)];
        preferences = newPreferences;
        profiling(profileType);
        

    }
    void profiling(int type)
    {
        int randVal = Random.Range(0, bios.Length);
        int choice = Random.Range(0, 2);
        bio = bios[randVal];
        switch (type)
        {
            //Lamia
            case 1:
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    choice = Random.Range(0, 2);
                    if (choice == 1)
                    {
                        bio = "I love going for a midnight snack, some say I even crave them!";
                    }
                    else
                    {
                        bio = "Looking for a fellow night-owl, to match my snacking habbits.";
                    }
                }
                break;
            //Mormo
            case 2:
                choice = Random.Range(0, 2);
                if (choice == 1)
                {
                    bio = "I enjoy deep conversations... especially about our deepest fears.";
                }
                else
                {
                    bio = "Lets share our childhood nightmares together.";
                }
                break;
            //Nymph
            case 3:
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    choice = Random.Range(0, 2);
                    if (choice == 1)
                    {
                        bio = "You'll usually find me by the water, its where i feel the most like myself.";
                    }
                    else
                    {
                        bio = "Nature is everything to me, I could literally spend my entire life in it.";
                    }
                }
                break;
            //Satyr
            case 4:
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    choice = Random.Range(0, 2);
                    if (choice == 1)
                    {
                        bio = "If there's a party, i'm already there; with my drink in hand!";
                    }
                    else
                    {
                        bio = "You definetely wont be able to keep up with me, im an animal.";
                    }
                }
                break;
            //Cyclops
            case 5:
                visibleRobot = true;
                break;
            //Silenus
            case 6:
                choice = Random.Range(0, 2);
                if (choice == 1)
                {
                    bio = "Some call me a philosopher but I say its just the drink.";
                }
                else
                {
                    bio = "I ramble allot when im drunk, though sometimes people say I make sense.";
                }
                break;
            //Siren
            case 7:
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    choice = Random.Range(0, 2);
                    if (choice == 1)
                    {
                        bio = "Some say my voice captivates them, I disagree; maybe you can be the deciding factor.";
                    }
                    else
                    {
                        bio = "Follow me like a lost puppy while I sing a melody to our future.";
                    }
                }
                break;
            //Regular Bot
            case 8:
                //Choose 1 of 3 implicit errors (Image, bio, age)
                choice = Random.Range(0, 3);
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    //Create list of robot bios
                    bio = "Beep Beep";
                }
                else if (choice == 2)
                {
                    age = Random.Range(40, 100);
                }
                break;

        }
    }
    /// <summary>
    /// Returns the preferences.
    /// </summary>
    public List<(string, int)> GetPreferences()
    {
        return preferences;
    }
    /// <summary>
    /// Formats the likes into a readable string.
    /// </summary>
    public string GetFormattedLikes()
    {
        //<font="CcEmotionSDF"><color="red><size=200%>/</size></color></font>
        /*
            love =  [
            like =  ]
            slightly like = \
            hate =  {
            dislike =  }
            slightly dislike =  |
            neutral = /
         */
        string[] formattedString = { "", "", "" };
        int[] previousVal = new int[] { -1, -1, -1 };
        for (int i =0; i < 3; i++)
        {
            int randomVal = Random.Range(0, 4);
            foreach (int j in previousVal)
            {
                if (randomVal == j)
                {
                    randomVal++;
                    if (randomVal > 3)
                    {
                        randomVal = 0;
                    }
                }
            }
            if (preferences[i].Item2 == 1)
            {
                if (profileType == 8 && Random.Range(0,1f)>0.5f)
                {
                    formattedString[i] += slightLikeStrings[randomVal] + likeEmotes[Random.Range(0,3)];
                }
                else
                {
                    formattedString[i] += slightLikeStrings[randomVal] + likeEmotes[0];
                }
                    
            }
            else if (preferences[i].Item2 == 2)
            {
                if (profileType == 8 && Random.Range(0, 1f) > 0.5f)
                {
                    formattedString[i] += likesStrings[randomVal] + likeEmotes[Random.Range(0, 3)];
                }
                else
                {
                    formattedString[i] += likesStrings[randomVal] + likeEmotes[1];
                }
            }
            else if (preferences[i].Item2 == 3)
            {
                if (profileType == 8 && Random.Range(0, 1f) > 0.5f)
                {
                    formattedString[i] += loveStrings[randomVal] + likeEmotes[Random.Range(0, 3)];
                }
                else
                {
                    formattedString[i] += loveStrings[randomVal] + likeEmotes[2];
                }
            }
            formattedString[i] = formattedString[i].Replace("X", preferences[i].Item1);
            formattedString[i] = normalizeSentence(formattedString[i]);
        }
        return formattedString[0] + formattedString[1] + formattedString[2];
    }
    /// <summary>
    /// Formats the dislikes into a readable string.
    /// </summary>
    public string GetFormattedDislikes()
    {
        string[] formattedString = {"","",""};
        int[] previousVal = new int[]{ -1, -1, -1 };
        for (int i = 3; i < 6; i++)
        {
            int randomVal = Random.Range(0, 4);
            foreach (int j in previousVal)
            {
                if (randomVal == j)
                {
                    randomVal++;
                    if (randomVal > 3)
                    {
                        randomVal = 0;
                    }
                }
            }
            previousVal[i - 3] = randomVal;
            if (preferences[i].Item2 == -1)
            {
                if (profileType == 8 && Random.Range(0, 1f) > 0.5f)
                {
                    formattedString[i-3] += slightDisikeStrings[randomVal] + dislikeEmotes[Random.Range(0, 3)];
                }
                else
                {
                    formattedString[i-3] += slightDisikeStrings[randomVal] + dislikeEmotes[0];
                }
            }
            else if (preferences[i].Item2 == -2)
            {
                if (profileType == 8 && Random.Range(0, 1f) > 0.5f)
                {
                    formattedString[i - 3] += dislikesStrings[randomVal] + dislikeEmotes[Random.Range(0, 3)];
                }
                else
                {
                    formattedString[i - 3] += dislikesStrings[randomVal] + dislikeEmotes[0];
                }
            }
            else if (preferences[i].Item2 == -3)
            {
                if (profileType == 8 && Random.Range(0, 1f) > 0.5f)
                {
                    formattedString[i - 3] += hatesStrings[randomVal] + dislikeEmotes[Random.Range(0, 3)];
                }
                else
                {
                    formattedString[i - 3] += hatesStrings[randomVal] + dislikeEmotes[0];
                }
            }
            formattedString[i - 3] = formattedString[i - 3].Replace("X", preferences[i].Item1);
            formattedString[i - 3] = normalizeSentence(formattedString[i - 3]);
        }
        return formattedString[0]+formattedString[1]+formattedString[2];
    }
    string normalizeSentence(string oldSentence)
    {
        oldSentence = oldSentence.ToLower();
        return oldSentence.First().ToString().ToUpper() + oldSentence.Substring(1);
    }
    public string GetBio()
    {
        return bio;
    }
    public string GetInfo()
    {
        string formattedString = "Age: " + age + "\n" + profession;
        return formattedString;
    }
}
