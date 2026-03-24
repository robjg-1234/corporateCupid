using System.Collections;
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
    }

    public void ActivateShredder()
    {
        if (currentPaper != null && !shredding)
        {
            StartCoroutine(ShredPaper());
        }
    }
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
    IEnumerator ShredPaper()
    {
        shredding = true;
        buttonCollider.enabled = false;
        shredderCollider.enabled = false;
        instance.dailyShred++;
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
}
