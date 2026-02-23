using System.Collections.Generic;
using UnityEngine;

public class SubmitScript : MonoBehaviour
{
    //Keep a reference of the selected letters.
    [SerializeField] GameObject matchObject;
    GameplayScript instance;
    PaperScript submissionOne;
    PaperScript submissionTwo;
    public bool isFull = false;
    public bool available = true;
    int id = 1;
    float totalScore = 0;
    Dictionary<int, float> relationshipMatches = new Dictionary<int, float>();

    private void Start()
    {
        instance = GameplayScript.instance;
        GameplayScript.mailInstance = this;
    }

    /// <summary>
    /// Adds a profile to the submission.
    /// </summary>
    public bool AddProfile(PaperScript selection)
    {
        bool admitted = false;
        if (available)
        {
            //Checks for empty spaces in the pinboard.
            if (submissionOne == null && submissionTwo != selection)
            {
                if (submissionTwo != null)
                {
                    submissionOne = submissionTwo;
                    submissionTwo = selection;
                    submissionOne.transform.position = transform.position - new Vector3(0.3f, 0, 0);
                    submissionTwo.transform.position = transform.position;
                }
                else
                {
                    submissionOne = selection;
                    submissionOne.transform.position = transform.position;
                }
                admitted = true;
            }
            else if (submissionTwo == null && submissionOne != selection)
            {
                submissionTwo = selection;
                submissionOne.transform.position = transform.position - new Vector3(0.3f, 0, 0);
                submissionTwo.transform.position = transform.position;
                admitted = true;
            }
            else
            {
                //T0-DO
                //Handle when the submission is full.
                //Possible replacing mechanic
                Debug.Log("Envelope is full.");
            }
            if (submissionOne != null || submissionTwo != null)
            {
                isFull = true;
            }
        }
        return admitted;
    }

    /// <summary>
    /// Checks if there are two profiles submitted and calculates their compatibility.
    /// </summary>
    public void Submit(AttachedLetter selection)
    {
        //To-do
        //Add confirmation
        totalScore = CalculateScores(selection.prof1.GetProfile().GetPreferences(), selection.prof2.GetProfile().GetPreferences());
        relationshipMatches.Add(id, totalScore);
        id++;
        Destroy(selection);
        instance.CallInteraction(instance.currentProfiles);
        Debug.Log(totalScore);
        instance.profilesMatched++;
        instance.overallScore += totalScore;
        
    }
    /// <summary>
    /// Handles the creation of Matches after clicking a button.
    /// </summary>
    public void CreateMatch()
    {
        if (submissionOne != null && submissionTwo != null)
        {
            AttachedLetter newMatch = Instantiate(matchObject, transform.position, Quaternion.identity).GetComponent<AttachedLetter>();
            newMatch.JoinPapers(submissionOne, submissionTwo);
            submissionTwo = null;
            submissionOne = null;
            newMatch.attachedBoard = this;
            isFull = true;
            available = false;
        }
    }

    /// <summary>
    /// Compares the preferences of both of the submitted profiles and returns the compatibility score between them.
    /// </summary>
    /// <param name="profileOne"></param>
    /// <param name="profileTwo"></param>
    /// <returns></returns>
    public static float CalculateScores(List<(string, int)> profileOne, List<(string, int)> profileTwo)
    {
        //strongly likes + strongly likes + 5
        /*
         * likes + likes  +3
         * 
         * prefer + prefer + 2
         * 
         * if both like but some people more than others get the average
         * 
         * same rules for dislike
         * 
         * if they  have differing likes ( 3 -  -1) go for the lowest
         * 
         * neutral + 1
         * 
         */
        float score = 0;
        int bestScore = 0;
        int[] potentialValues = new int[] { 2, 3, 5 };
        for (int i = 0; i < profileOne.Count; i++)
        {
            bestScore += potentialValues[Mathf.Abs(profileOne[i].Item2)-1];
            bool found = false;
            int value = -1;
            for (int j = 0; j < profileTwo.Count; j++)
            {
                if (profileOne[i].Item1.Equals(profileTwo[j].Item1))
                {
                    found = true;
                    value = j;
                    break;
                }
            }
            if (found)
            {
                int result = 0;
                if ((profileOne[i].Item2 > 0 && profileTwo[i].Item2 > 0) || (profileOne[i].Item2 < 0 && profileTwo[i].Item2 < 0))
                {
                    result = Mathf.Min(Mathf.Abs(profileTwo[value].Item2), Mathf.Abs(profileOne[i].Item2));
                    if (result == 3)
                    {
                        score += potentialValues[result - 1];
                    }
                    else 
                    {
                        score += (potentialValues[result]+ potentialValues[result - 1])/2;
                    }
                }
                else
                {
                    result = Mathf.Min(profileTwo[value].Item2, profileOne[i].Item2);
                    score -= potentialValues[(result * -1) - 1];
                }
            }
            else
            {
                score += 1;
            }
        }
        if (score < 0)
        {
            score = 0;
        }
        score /= bestScore;
        Debug.Log(bestScore);

        return score;
    }
    /// <summary>
    /// Handles the removal of profiles from the Pinboard.
    /// </summary>
    public void RemoveProfile(PaperScript profile)
    {
        if (submissionOne == profile)
        {
            submissionOne = null;
        }
        else if (submissionTwo == profile)
        {
            submissionTwo = null;
            if (submissionOne != null)
            {
                submissionOne.transform.position = transform.position;
            }
        }
        if (submissionTwo == null && submissionOne == null)
        {
            isFull = false;
        }
    }
}
