using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
    [SerializeField] Image fadeScreen;
    bool fadingOut = false;

    public void LoadLevel()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            fadeScreen.gameObject.SetActive(true);
            StartCoroutine(FadeOut());
        }
    }
    public void ExitGame()
    {
        Application.Quit();
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
