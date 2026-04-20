using System.Collections;
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
        if (instance.day == 0 && instance.stepNumber == 3)
        {
            instance.stepNumber++;
            instance.stepDone = true;
        }
        totalScore = CalculateScores(selection.prof1.GetProfile().GetPreferences(), selection.prof2.GetProfile().GetPreferences());
        if (!(selection.prof1.GetProfile().profileType > 0 || selection.prof2.GetProfile().profileType > 0) && instance.day != 3)
        {
            totalScore = 0;
            instance.fees += 0.1f;
        }
        relationshipMatches.Add(id, totalScore);
        id++;
        Destroy(selection.gameObject);
        instance.CallInteraction(instance.currentProfiles);
        if (totalScore >= 0.45)
        {
            instance.validScore += totalScore;
        }
        instance.dailyMatch++;
        Debug.Log(totalScore);
        instance.dailyScore += totalScore;
    }
    /// <summary>
    /// Handles the creation of Matches after clicking a button.
    /// </summary>
    public bool CreateMatch(EnvelopeScript envelope)
    {
        if (submissionOne != null && submissionTwo != null && instance.clockedIn)
        {
            if (instance.day==0 && instance.stepNumber == 2)
            {
                instance.stepNumber++;
                instance.stepDone = true;
            }
            AttachedLetter newMatch = Instantiate(matchObject, transform.position, Quaternion.identity).GetComponent<AttachedLetter>();
            newMatch.JoinPapers(submissionOne, submissionTwo, envelope);
            submissionTwo = null;
            submissionOne = null;
            newMatch.attachedBoard = this;
            isFull = true;
            available = false;
            Debug.Log("true");
            return true;
        }
        Debug.Log("false");
        return false;
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
                if ((profileOne[i].Item2 > 0 && profileTwo[value].Item2 > 0) || (profileOne[i].Item2 < 0 && profileTwo[value].Item2 < 0))
                {
                    score++;
                    int tempVal = potentialValues[Mathf.Abs(profileTwo[value].Item2) - 1] - potentialValues[Mathf.Abs(profileOne[i].Item2) - 1];
                    if (tempVal > 0)
                    {
                        bestScore += tempVal;
                    }
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
                    result = Mathf.Min(Mathf.Abs(profileTwo[value].Item2), Mathf.Abs(profileOne[i].Item2));
                    score -= potentialValues[result - 1];
                }
            }
            else
            {
                score += 1.5f;
            }
            
        }
        if (score < 0)
        {
            score = 0;
        }
        else if (score > bestScore)
        {
            score = bestScore;
        }
        score /= bestScore;
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
