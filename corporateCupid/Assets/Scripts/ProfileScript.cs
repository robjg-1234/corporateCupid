
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
    public ProfileScript(string newName, List<(string, int)> newPreferences, int type =0)
    {
        characterName = newName;
        age = Random.Range(18,35);
        //Pharloom Reference :O
        profileType = type;
        profession = "Bell Ringer";
        preferences = newPreferences;
        profiling(profileType);
        

    }
    void profiling(int type)
    {
        int randVal = Random.Range(0, bios.Length);
        switch (type)
        {
            //Lamia
            case 1:
                bio = "Hi, I am a Lamia.";
                break;
            //Mormo
            case 2:
                bio = "Hi, I am a Mormo.";
                break;
            //Nymph
            case 3:
                age = Random.Range(18, 20);
                bio = "Hi, I am a Nymph.";
                break;
            //Satyr
            case 4:
                bio = "Hi, I am a Satyr.";
                break;
            //Cyclops
            case 5:
                bio = "Hi, I am a Cyclops.";
                break;
            //Regular Bot
            case 6:
                //Choose 1 of 3 implicit errors (Image, bio, age)
                int choice = Random.Range(0, 3);
                bio = bios[randVal];
                if (choice == 0)
                {
                    visibleRobot = true;
                }
                else if (choice == 1)
                {
                    //Create list of robot bios
                    bio = "I am robot";
                }
                else if (choice == 2)
                {
                    age = Random.Range(18, 100);
                }
                break;
            //normal
            default:
                bio = bios[randVal];
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
                if (profileType == 6 && Random.Range(0,1f)>0.5f)
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
                if (profileType == 6 && Random.Range(0, 1f) > 0.5f)
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
                if (profileType == 6 && Random.Range(0, 1f) > 0.5f)
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
                if (profileType == 6 && Random.Range(0, 1f) > 0.5f)
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
                if (profileType == 6 && Random.Range(0, 1f) > 0.5f)
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
                if (profileType == 6 && Random.Range(0, 1f) > 0.5f)
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
