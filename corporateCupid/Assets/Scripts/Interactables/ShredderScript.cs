using System.Collections;
using System.Security.Principal;
using UnityEngine;

public class ShredderScript : MonoBehaviour
{
    [SerializeField] Collider2D shredderCollider;
    [SerializeField] Collider2D buttonCollider;
    [SerializeField] GameObject fakeProfile;
    GameplayScript instance;
    PaperScript currentPaper;
    bool shredding = false;
    Vector3 originalPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPos = fakeProfile.transform.localPosition;
        instance = GameplayScript.instance;
        GameplayScript.shredInstance = this;
        instance.dayEnded += EndOfDay;
    }
    private void OnDestroy()
    {
        instance.dayEnded -= EndOfDay;
    }
    /// <summary>
    /// Inititate Paper Shredding
    /// </summary>
    public void ActivateShredder()
    {
        if (currentPaper != null && !shredding)
        {
            StartCoroutine(ShredPaper());
        }
    }
    /// <summary>
    /// Activates the fake profile and saves a reference to the selected paper.
    /// </summary>
    public bool AddPaper(PaperScript profile)
    {
        if (currentPaper == null)
        {
            currentPaper = profile;
            profile.gameObject.SetActive(false);
            buttonCollider.enabled = true;
            fakeProfile.SetActive(true);
            return true;
        }
        return false;
    }
    /// <summary>
    /// Returns the existing paper. 
    /// </summary>
    public PaperScript GrabPaper()
    {
        PaperScript temp = null;
        if (currentPaper != null)
        {
            currentPaper.gameObject.SetActive(true);
            temp = currentPaper;
            currentPaper = null;
            fakeProfile.SetActive(false);
        }
        return temp;
    }
    /// <summary>
    /// Coroutine that processes the animation for shredding the paper.
    /// </summary>
    IEnumerator ShredPaper()
    {
        if (instance.day ==0 && instance.stepNumber == 5)
        {
            instance.stepNumber++;
            instance.stepDone = true;
        }
        shredding = true;
        buttonCollider.enabled = false;
        shredderCollider.enabled = false;
        instance.dailyShred++;
        if (currentPaper.GetProfile().profileType > 0)
        {
            instance.dailyScore += 0.25f;
        }
        else
        {
            instance.dailyScore -= 0.25f;
        }
        if (instance.dailyScore < 0)
        {
            instance.dailyScore = 0;
        }
        Destroy(currentPaper.gameObject);
        currentPaper = null;
        Vector3 finalPos = new Vector3(0.0410000011f, -0.899999976f, 0);
        float t = 0;
        Vector3 startingPos = originalPos;
        Vector3 currentPos = startingPos;
        //Shredding Animation
        while (currentPos.y != finalPos.y)
        {
            t += 4 * Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            currentPos = new Vector3(startingPos.x, Mathf.Lerp(startingPos.y, finalPos.y, t), 0);
            fakeProfile.transform.localPosition = currentPos;
            yield return null;
        }
        shredderCollider.enabled = true;
        shredding = false;
        fakeProfile.SetActive(false);
        fakeProfile.transform.localPosition = originalPos;
    }
    /// <summary>
    /// Function attahced to the end of day action that handles any remaining object.
    /// </summary>
    public void EndOfDay()
    {
        if (currentPaper != null)
        {
            instance.dailyShred++;
            if (currentPaper.GetProfile().profileType > 0 && instance.day != 3)
            {
                instance.dailyScore += 0.25f;
            }
            else
            {
                if (instance.day != 3)
                {
                    instance.dailyScore -= 0.25f;
                }
            }
            if (instance.dailyScore < 0)
            {
                instance.dailyScore = 0;
            }
            Destroy(currentPaper.gameObject);
            currentPaper = null;
            fakeProfile.SetActive(false);
        }
    }
}
