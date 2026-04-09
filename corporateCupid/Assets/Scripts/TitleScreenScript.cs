using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
    [SerializeField] Image fadeScreen;
    bool fadingOut = false;

    void LoadLevel()
    {
        if (!fadingOut)
        {
            fadingOut = true;
            StartCoroutine(FadeOut());
        }
    }
    IEnumerator FadeOut()
    {
        float val = 0;
        while (val < 1)
        {
            val += 4 * Time.deltaTime;
            fadeScreen.color = new Color(1, 1, 1, val);
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
