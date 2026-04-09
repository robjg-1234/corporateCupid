
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Conatins all the information of the profile.
/// </summary>
public class ProfileScript
{
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
    public ProfileScript(string newName, List<(string, int)> newPreferences)
    {
        characterName = newName;
        age = Random.Range(18,35);
        //Pharloom Reference :O

        profession = "Bell Ringer";
        preferences = newPreferences;
        int randVal = Random.Range(0, bios.Length);
        bio = bios[randVal];
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
        string formattedString = "";
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
                formattedString += slightLikeStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#a1cc14><size=200%>\\</size></color></font> ";
            }
            else if (preferences[i].Item2 == 2)
            {
                formattedString += likesStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#47910a><size=200%>]</size></color></font> ";
            }
            else if (preferences[i].Item2 == 3)
            {
                formattedString += loveStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#3e6320><size=200%>[</size></color></font> ";
            }
            formattedString = formattedString.Replace("X", preferences[i].Item1);
        }
        
        return formattedString;
    }
    /// <summary>
    /// Formats the dislikes into a readable string.
    /// </summary>
    public string GetFormattedDislikes()
    {
        string formattedString = "";
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
                formattedString += slightDisikeStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#FAB525><size=200%>|</size></color></font> ";
            }
            else if (preferences[i].Item2 == -2)
            {
                formattedString += dislikesStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#FA9125><size=200%>}</size></color></font> ";
            }
            else if (preferences[i].Item2 == -3)
            {
                formattedString += hatesStrings[randomVal] + ". <font=\"CcEmotionSDF\"><color=#D52915><size=200%>{</size></color></font> ";
            }
            formattedString = formattedString.Replace("X", preferences[i].Item1);
        }
        return formattedString;
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
