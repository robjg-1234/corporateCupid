using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
    [SerializeField] Image fadeScreen;
    [SerializeField] GameObject popUp;
    [SerializeField] Button continueButton;
    [SerializeField] TMP_Text continueText;
    [SerializeField] Image buttonYes;
    [SerializeField] Image buttonNo;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject credits;
    [SerializeField] Toggle windowToggle;
    bool fadingOut = false;
    private void Start()
    {
        PlayerPrefs.SetInt("SkipTutorial", 0);
        if (SaveScript.SaveFileExists())
        {
            PlayerPrefs.SetInt("Save", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Save", 0);
            continueButton.interactable = false;
            continueText.color = new(0.5f, 0.5f, 0.5f, 0.5f);
        }
        if (PlayerPrefs.GetInt("Windowed") == 1)
        {
            windowToggle.isOn = true;
        }
        PlayerPrefs.Save();
    }
    public void PopUp()
    {
        if (!fadingOut)
        {
            popUp.SetActive(true);
        }
    }
    public void closePopUp()
    {
        if (!fadingOut)
        {
            popUp.SetActive(false);
        }
    }
    public void OpenSettings()
    {
        if (!fadingOut)
        {
            settings.SetActive(true);
        }
    }
    public void CloseSettings()
    {
        if (!fadingOut)
        {
            settings.SetActive(false);
        }
    }
    public void OpenCredits()
    {
        if (!fadingOut)
        {
            credits.SetActive(true);
        }
    }
    public void CloseCredits()
    {
        if (!fadingOut)
        {
            credits.SetActive(false);
        }
    }
    /// <summary>
    /// Start new game with tutorial
    /// </summary>
    public void LoadLevel()
    {
        if (!fadingOut)
        {
            buttonYes.color = new(1, 1, 1, 1);
            PlayerPrefs.SetInt("Save", 0);
            PlayerPrefs.Save();
            fadingOut = true;
            fadeScreen.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }
    /// <summary>
    /// Continue from save file.
    /// </summary>
    public void ContinueLevel()
    {
        if (!fadingOut)
        {
            PlayerPrefs.SetInt("Save", 1);
            PlayerPrefs.Save();
            fadingOut = true;
            fadeScreen.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }
    /// <summary>
    /// Start game by skipping tutorial.
    /// </summary>
    public void SkipLoadLevel()
    {
        if (!fadingOut)
        {
            buttonNo.color = new(1, 1, 1, 1);
            PlayerPrefs.SetInt("SkipTutorial", 1);
            PlayerPrefs.SetInt("Save", 0);
            PlayerPrefs.Save();
            fadingOut = true;
            fadeScreen.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Resize(bool windowed)
    {
        if (windowed)
        {
            Screen.SetResolution(960, 540, false);
            PlayerPrefs.SetInt("Windowed", 1);
        }
        else
        {
            Screen.SetResolution(1920, 1080, true);
            PlayerPrefs.SetInt("Windowed", 0);
        }
        PlayerPrefs.Save();
    }
    IEnumerator FadeOut()
    {
        float val = 0;
        while (val < 1)
        {
            val += 2 * Time.deltaTime;
            fadeScreen.color = new Color(0, 0, 0, val);
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
