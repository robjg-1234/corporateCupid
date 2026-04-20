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
    [SerializeField] Slider matchQualityBar;
    [SerializeField] Image resultObject;
    [SerializeField] Image stamp;
    [SerializeField] TMP_Text gainedText;
    [SerializeField] TMP_Text matchesText;
    [SerializeField] TMP_Text shredsText;
    [SerializeField] TMP_Text qualityText;
    [SerializeField] TMP_Text savingText;
    [SerializeField] TMP_Text rentText;
    [SerializeField] TMP_Text foodText;
    [SerializeField] TMP_Text resultsText;
    [SerializeField] TMP_Text feesText;
    [SerializeField] TMP_Text paperDayText;
    [SerializeField] Toggle foodTog;
    [SerializeField] Toggle rentTog;
    [SerializeField] Image secondFade;
    [SerializeField] Image endingScreen;
    int rent = 3;
    int food = 1;
    int ending = 0;
    bool waiting = false;
    private void Start()
    {
        
        instance = GameplayScript.instance;
        rent = 3 + 2 * instance.day;
        food = 1 + 1 * instance.day;
        resultObject.gameObject.SetActive(false);
        StartCoroutine(FadeIn());
    }
    void UpdateText()
    {
        //TO-DO: Separate information for day by day and total
        gainedText.text = "+"+instance.amountGained.ToString("0.00") + "$";
        matchesText.text = instance.dailyMatch + "";
        
        shredsText.text = instance.dailyShred + "";
        if (instance.profilesMatched > 0)
        {
            float val = (instance.dailyScore /( instance.dailyMatch+instance.dailyIncorrectShreds))*100;
            
            qualityText.text = val.ToString("0.00") + "%";
            val /= 100.0f;
            matchQualityBar.value = val;
            if (val> 0.8f)
            {
                stamp.sprite = Resources.Load<Sprite>("endofday/Stamp/A");
            }
            else if (val > 0.65f)
            {
                stamp.sprite = Resources.Load<Sprite>("endofday/Stamp/B");
            }
            else if (val > 0.5f)
            {
                stamp.sprite = Resources.Load<Sprite>("endofday/Stamp/C");
            }
            else
            {
                stamp.sprite = Resources.Load<Sprite>("endofday/Stamp/D");
            }
        }
        else
        {
            matchQualityBar.value = 0;
            stamp.sprite = Resources.Load<Sprite>("endofday/Stamp/D");
            qualityText.text = "0%";
        }
        savingText.text = (instance.money-instance.amountGained + instance.fees).ToString("0.00") +"$";
        resultsText.text = (instance.money).ToString("0.00") + "$";
        rentText.text = "-"+rent.ToString("0.00")+"$";
        foodText.text = "-"+food.ToString("0.00")+ "$";
        feesText.text = "-" + instance.fees.ToString("0.00") + "$";

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
        float fakeFinal = instance.money;
        if (foodTog.isOn)
        {
            if (fakeFinal - food >= 0)
            {
                fakeFinal -= food;
            }
            else
            {
                foodTog.isOn = false;
            }
        }
        if (rentTog.isOn)
        {
            if (fakeFinal - rent >= 0)
            {
                fakeFinal -= rent;
            }
            else
            {
                rentTog.isOn = false;
            }
        }
        resultsText.text = fakeFinal.ToString("0.00") + "$";
    }

    void UpdateMoneyAndFatigue()
    {
        float fakeFinal = instance.money;
        if (foodTog.isOn)
        {
            if (fakeFinal - food >= 0)
            {
                fakeFinal -= food;
                instance.money -= food;
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
            if (fakeFinal - rent >= 0)
            {
                fakeFinal -= rent;
                instance.money -= rent;
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
        secondFade.color = new(secondFade.color.r, secondFade.color.g, secondFade.color.b, 0);
        paperDayText.text = "" + instance.day;
        secondFade.gameObject.SetActive(false);
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
        HandbookScript.instance.CloseBook();
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
        instance.overallScore += instance.dailyScore;
        instance.incorrectShreds += instance.dailyIncorrectShreds;
        instance.profilesMatched += instance.dailyMatch;
        if (instance.validScore > 0 && instance.day>0)
        {
            instance.amountGained += (instance.validScore * 1.3f);
            Debug.Log("Money Gained: " + instance.amountGained);
            instance.money += instance.amountGained;
            Debug.Log("Total money: " + instance.money);
            if (instance.money - instance.fees >= 0)
            {
                instance.money -= instance.fees;
            }
            else
            {
                instance.fees = instance.money;
                instance.money -= instance.fees;
            }
            

        }
        else
        {
            instance.fees = 0;
        }
        instance.validScore = 0;
        UpdateText();
        
        if (instance.day == 0)
        {
            instance.profilesShredded -= instance.dailyShred;
            instance.profilesMatched -= instance.dailyMatch;
            instance.incorrectShreds -= instance.dailyIncorrectShreds;
            instance.overallScore -= instance.dailyScore;
        }
        instance.dailyMatch = 0;
        instance.dailyScore = 0;
        instance.dailyShred = 0;
        instance.dailyIncorrectShreds = 0;
        
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
        secondFade.gameObject.SetActive(true);
        secondFade.color = new(secondFade.color.r, secondFade.color.g, secondFade.color.b, 1);
        resultObject.gameObject.SetActive(true);
        t = 0;
        alpha = 1;
        yield return new WaitForSeconds(0.25f);
        waiting = false;
        while (alpha >0)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(1, 0, t);
            secondFade.color = new(secondFade.color.r, secondFade.color.g, secondFade.color.b, alpha);
            yield return null;
        }
        waiting = true;
        secondFade.color = new(secondFade.color.r, secondFade.color.g, secondFade.color.b, 0);
        secondFade.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        t = 0;
        while (waiting)
        {
            yield return null;
        }
        secondFade.gameObject.SetActive(true);
        while (alpha < 1)
        {
            t += Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            alpha = Mathf.Lerp(0, 1, t);
            secondFade.color = new(secondFade.color.r, secondFade.color.g, secondFade.color.b, alpha);
            yield return null;
        }
        resultObject.gameObject.SetActive(false);
        dayText.text = "Day " + instance.day;
        rent = 3 + 2*instance.day;
        food = 1 + 1*instance.day;
        UpdateMoneyAndFatigue();
        instance.fees = 0;
        instance.amountGained = 0;
        yield return new WaitForSeconds(0.1f);
        if (instance.timeMultiplier >= 2)
        {
            ending = 3;
            StartCoroutine(EndGame());
        }
        else
        {
            if (instance.day == 6)
            {
                float finalScore = instance.overallScore / instance.profilesMatched * 100;
                if (finalScore > 80)
                {
                    ending = 1;
                }
                else if (finalScore > 40)
                {
                    ending = 2;
                }
                else
                {
                    ending = 3;
                }
                StartCoroutine(EndGame());
            }
            else
            {
                StartCoroutine(FadeIn());
            }
        }
    }

    IEnumerator EndGame()
    {
        endingScreen.sprite = Resources.Load<Sprite>("Ending/" + ending);
        float a = 0;
        endingScreen.color = new(1, 1, 1, a);
        endingScreen.gameObject.SetActive(true);
        while (a < 1)
        {
            a += Time.deltaTime * 0.5f;
            if (a > 1)
            {
                a = 1;
            }
            endingScreen.color = new(1, 1, 1, a);
            yield return null;
        }
        yield return new WaitForSeconds(2.5f);
        bool wait = true;
        while (wait)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                wait = false;
            }
            yield return null;
        }
        instance.ExitToTitleScreen();
    }
}
