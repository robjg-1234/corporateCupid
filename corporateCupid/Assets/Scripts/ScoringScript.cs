using System.Collections.Generic;
using UnityEngine;

public class ScoringScript
{
    List<(int, float)> relationshipMatches;

    public static float CalculateScores(List<(string, int)> profileOne, List<(string, int)> profileTwo)
    {
        //strongly likes + strongly likes + 5
        /*
         * likes + likes  +3
         * 
         * prefer + prefer + 2
         * 
         * if both like but some people more than others round down
         * 
         * same rules for dislike
         * 
         * if they  have differing likes ( 3 -  -1) go for the lowest
         * 
         * neutral + 1
         * 
         */
        float score = 0;
        int[] potentialValues = new int[] {2,3,5 };
        
        for (int i = 0; i < profileOne.Count; i++)
        {
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
                if ((profileOne[i].Item2 > 0 && profileTwo[i].Item2>0) || (profileOne[i].Item2 < 0 && profileTwo[i].Item2 < 0))
                {
                    result = Mathf.Min(Mathf.Abs(profileTwo[value].Item2), Mathf.Abs(profileOne[i].Item2));
                    Debug.Log(potentialValues[result-1]);
                    score += potentialValues[result-1];
                }
                else
                {
                    result = Mathf.Min(profileTwo[value].Item2, profileOne[i].Item2);
                    Debug.Log(potentialValues[(result * -1) - 1]);
                    score -= potentialValues[(result*-1)-1];
                }
            }
            else
            {
                Debug.Log(1);
                score += 1;
            }
        }
        if (score < 0)
        {
            score = 0;
        }
        Debug.Log(score);
        score /= 30;
       
        return score;



    }
}
