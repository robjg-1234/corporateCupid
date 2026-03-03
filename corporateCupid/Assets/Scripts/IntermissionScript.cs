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

    private void Start()
    {
        instance = GameplayScript.instance;
        dayText.color = new(dayText.color.r, dayText.color.g, dayText.color.b, 0);
        resultObject.color = new(resultObject.color.r, resultObject.color.g, resultObject.color.b, 0);
        reportText.color = new(reportText.color.r, reportText.color.g, reportText.color.b, 0);
        matchesText.color = new(matchesText.color.r, matchesText.color.g, matchesText.color.b, 0);
        shredsText.color = new(shredsText.color.r, shredsText.color.g, shredsText.color.b, 0);
        qualityText.color = new(qualityText.color.r, qualityText.color.g, qualityText.color.b, 0);
        StartCoroutine(FadeIn());
    }
    void UpdateText()
    {
        matchesText.text = "Profiles Matched: " + instance.profilesMatched + "\r\n";
        shredsText.text = "Profiles Shredded: " + instance.profilesShredded + "\r\n";
        qualityText.text = "Matches Quality: " + (instance.overallScore/instance.profilesMatched) + "%\r\n";
    }
    IEnumerator FadeIn()
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
        instance.dayEnded?.Invoke();
        UpdateText();
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
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        bool waiting = true;
        t = 0;
        while (waiting)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                waiting = false;
            }
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
            resultObject.color = new(resultObject.color.r, resultObject.color.g, resultObject.color.b, alpha);
            reportText.color = new(reportText.color.r, reportText.color.g, reportText.color.b, alpha);
            matchesText.color = new(matchesText.color.r, matchesText.color.g, matchesText.color.b, alpha);
            shredsText.color = new(shredsText.color.r, shredsText.color.g, shredsText.color.b, alpha);
            qualityText.color = new(qualityText.color.r, qualityText.color.g, qualityText.color.b, alpha);
            yield return null;
        }
        dayText.text = "Day " + instance.day;
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(FadeIn());
    }
}
