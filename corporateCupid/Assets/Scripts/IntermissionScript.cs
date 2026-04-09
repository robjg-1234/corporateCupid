using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IntermissionScript : MonoBehaviour
{
    GameplayScript instance;
    //Day Screen
    [SerializeField] Image fadeScreen;
    [SerializeField] TMP_Text dayText;
    //Result Screen
    [SerializeField] Image resultObject;
    [SerializeField] TMP_Text reportText;
    [SerializeField] TMP_Text matchesText;
    [SerializeField] TMP_Text shredsText;
    [SerializeField] TMP_Text qualityText;
    [SerializeField] TMP_Text buttonText;
    [SerializeField] Image buttonBack;
    //
    [SerializeField] TMP_Text savingText;
    [SerializeField] TMP_Text rentText;
    [SerializeField] TMP_Text foodText;
    [SerializeField] Image foodToggle;
    [SerializeField] Image rentToggle;
    [SerializeField] Image foodCheck;
    [SerializeField] Image rentCheck;
    [SerializeField] TMP_Text resultsText;
    [SerializeField] Toggle foodTog;
    [SerializeField] Toggle rentTog;
    bool waiting = false;
    private void Start()
    {
        instance = GameplayScript.instance;
        dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, 0);
        resultObject.color = new(resultObject.color.r, resultObject.color.g, resultObject.color.b, 0);
        reportText.color = new(reportText.color.r, reportText.color.g, reportText.color.b, 0);
        matchesText.color = new(matchesText.color.r, matchesText.color.g, matchesText.color.b, 0);
        shredsText.color = new(shredsText.color.r, shredsText.color.g, shredsText.color.b, 0);
        qualityText.color = new(qualityText.color.r, qualityText.color.g, qualityText.color.b, 0);
        buttonText.color = new(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0);
        buttonBack.color = new(buttonBack.color.r, buttonBack.color.g, buttonBack.color.b, 0);

        savingText.color = new(savingText.color.r, savingText.color.g, savingText.color.b, 0);
        rentText.color = new(rentText.color.r, rentText.color.g, rentText.color.b, 0);
        foodText.color = new(foodText.color.r, foodText.color.g, foodText.color.b, 0);
        foodToggle.color = new(foodToggle.color.r, foodToggle.color.g, foodToggle.color.b, 0);
        rentToggle.color = new(rentToggle.color.r, rentToggle.color.g, rentToggle.color.b, 0);
        foodCheck.color = new(foodCheck.color.r, foodCheck.color.g, foodCheck.color.b, 0);
        rentCheck.color = new(rentCheck.color.r, rentCheck.color.g, rentCheck.color.b, 0);
        resultsText.color = new(resultsText.color.r, resultsText.color.g, resultsText.color.b, 0);
        StartCoroutine(FadeIn());
    }
    void UpdateText()
    {
        //TO-DO: Separate information for day by day and total
        matchesText.text = "Profiles Matched: " + instance.dailyMatch + "\r\n";
        shredsText.text = "Profiles Shredded: " + instance.dailyShred + "\r\n";
        if (instance.profilesMatched > 0)
        {
            float val = (instance.dailyScore / instance.dailyMatch)*100;
            qualityText.text = "Matches Quality: " + val.ToString("0.00") + "%\r\n";
        }
        else
        {
            qualityText.text = "Matches Quality: 0%\r\n";
        }
        savingText.text = "Savings: " + instance.money +"$";
        resultsText.text = "Remaining: " + instance.money + "$";
        //TO-DO: Add the total overall score
        
    }
    public void Proceed()
    {
        if (waiting)
        {
            waiting = false;
        }
    }

    public void UpdateCost()
    {
        int fakeFinal = instance.money;
        if (foodTog.isOn)
        {
            if (fakeFinal - 3 >= 0)
            {
                fakeFinal -= 3;
            }
            else
            {
                foodTog.isOn = false;
            }
        }
        if (rentTog.isOn)
        {
            if (fakeFinal - 12 >= 0)
            {
                fakeFinal -= 12;
            }
            else
            {
                rentTog.isOn = false;
            }
        }
        resultsText.text = "Remaining: " + fakeFinal + "$";
    }

    void UpdateMoneyAndFatigue()
    {
        int fakeFinal = instance.money;
        if (foodTog.isOn)
        {
            if (fakeFinal - 3 >= 0)
            {
                fakeFinal -= 3;
                instance.money -= 3;
                instance.timeMultiplier -= 0.1f;
                if (instance.timeMultiplier < 1)
                {
                    instance.timeMultiplier = 1;
                }
            }
        }
        else
        {
            instance.timeMultiplier += 0.1f;
        }
        if (rentTog.isOn)
        {
            if (fakeFinal - 12 >= 0)
            {
                fakeFinal -= 12;
                instance.money -= 12;
                instance.timeMultiplier -= 0.4f;
                if (instance.timeMultiplier < 1)
                {
                    instance.timeMultiplier = 1;
                }
            }
        }
        else
        {
            instance.timeMultiplier += 0.4f;
        }
        foodTog.isOn = false;
        rentTog.isOn = false;
    }
    IEnumerator FadeIn()
    {
        instance.SetTime(9, 0);
        dayText.text = "Day " + instance.day;
        fadeScreen.gameObject.SetActive(true);
        float t = 0;
        float alpha = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            yield return null;
        }
        t = 0;
        yield return new WaitForSeconds(1.5f);
        while (alpha > 0)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);
        alpha = 1;
        t = 0;
        while (alpha > 0)
        {
            t += 2 * Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            fadeScreen.color = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, alpha);
            yield return null;
        }
        fadeScreen.gameObject.SetActive(false);
        StartCoroutine(instance.DayControl());
    }
    public IEnumerator FadeOut()
    {
        fadeScreen.gameObject.SetActive(true);
        float t = 0;
        float alpha = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            fadeScreen.color = new(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, alpha);
            yield return null;
        }
        yield return null;
        instance.clockedIn = false;
        instance.dayEnded?.Invoke();
        instance.profilesShredded += instance.dailyShred;
        UpdateText();
        
        if (instance.day == 0)
        {
            instance.profilesShredded -= instance.dailyShred;
            instance.profilesMatched -= instance.dailyMatch;
            instance.overallScore -= instance.dailyScore;
        }
        instance.dailyMatch = 0;
        instance.dailyScore = 0;
        instance.dailyShred = 0;
        alpha = 0;
        t = 0;
        instance.day++;
        dayText.text = "End of shift";
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        while (alpha > 0)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, alpha);
            yield return null;
        }
        t = 0;
        alpha = 0;
        yield return new WaitForSeconds(0.25f);
        waiting = false;
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            resultObject.color = new(resultObject.color.r, resultObject.color.g, resultObject.color.b, alpha);
            reportText.color = new(reportText.color.r, reportText.color.g, reportText.color.b, alpha);
            matchesText.color = new(matchesText.color.r, matchesText.color.g, matchesText.color.b, alpha);
            shredsText.color = new(shredsText.color.r, shredsText.color.g, shredsText.color.b, alpha);
            qualityText.color = new(qualityText.color.r, qualityText.color.g, qualityText.color.b, alpha);
            buttonText.color = new(buttonText.color.r, buttonText.color.g, buttonText.color.b, alpha);
            buttonBack.color = new(buttonBack.color.r, buttonBack.color.g, buttonBack.color.b, alpha);
            yield return null;
        }
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        t = 0;
        while (waiting)
        {
            yield return null;
        }
        while (alpha > 0)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            matchesText.color = new(matchesText.color.r, matchesText.color.g, matchesText.color.b, alpha);
            shredsText.color = new(shredsText.color.r, shredsText.color.g, shredsText.color.b, alpha);
            qualityText.color = new(qualityText.color.r, qualityText.color.g, qualityText.color.b, alpha);
            yield return null;
        }
        alpha = 0;
        t = 0;
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            savingText.color = new(savingText.color.r, savingText.color.g, savingText.color.b, alpha);
            rentText.color = new(rentText.color.r, rentText.color.g, rentText.color.b, alpha);
            foodText.color = new(foodText.color.r, foodText.color.g, foodText.color.b, alpha);
            foodToggle.color = new(foodToggle.color.r, foodToggle.color.g, foodToggle.color.b, alpha);
            rentToggle.color = new(rentToggle.color.r, rentToggle.color.g, rentToggle.color.b, alpha);
            foodCheck.color = new(foodCheck.color.r, foodCheck.color.g, foodCheck.color.b, alpha);
            rentCheck.color = new(rentCheck.color.r, rentCheck.color.g, rentCheck.color.b, alpha);
            resultsText.color = new(resultsText.color.r, resultsText.color.g, resultsText.color.b, alpha);
            yield return null;
        }
        waiting = true;
        while (waiting)
        {
            yield return null;
        }
        t = 0;
        while (alpha > 0)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            resultObject.color = new(resultObject.color.r, resultObject.color.g, resultObject.color.b, alpha);
            reportText.color = new(reportText.color.r, reportText.color.g, reportText.color.b, alpha);
            savingText.color = new(savingText.color.r, savingText.color.g, savingText.color.b, alpha);
            rentText.color = new(rentText.color.r, rentText.color.g, rentText.color.b, alpha);
            foodText.color = new(foodText.color.r, foodText.color.g, foodText.color.b, alpha);
            foodToggle.color = new(foodToggle.color.r, foodToggle.color.g, foodToggle.color.b, alpha);
            rentToggle.color = new(rentToggle.color.r, rentToggle.color.g, rentToggle.color.b, alpha);
            foodCheck.color = new(foodCheck.color.r, foodCheck.color.g, foodCheck.color.b, alpha);
            rentCheck.color = new(rentCheck.color.r, rentCheck.color.g, rentCheck.color.b, alpha);
            resultsText.color = new(resultsText.color.r, resultsText.color.g, resultsText.color.b, alpha);
            buttonText.color = new(buttonText.color.r, buttonText.color.g, buttonText.color.b, alpha);
            buttonBack.color = new(buttonBack.color.r, buttonBack.color.g, buttonBack.color.b, alpha);
            yield return null;
        }
        t = 0;
        dayText.text = "Day " + instance.day;
        UpdateMoneyAndFatigue();
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(FadeIn());
    }
}
