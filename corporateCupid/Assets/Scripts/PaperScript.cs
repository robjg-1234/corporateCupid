using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaperScript : MonoBehaviour
{
    GameplayScript instance;
    ProfileScript identity;
    public int recency = 0;
    [SerializeField] GameObject[] attachedObjects;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = GameplayScript.instance;
        identity = instance.PickProfile();
        recency = instance.currentProfiles;
        instance.objectInteracted += ChangePriority;
        transform.localScale = new Vector3(0.5f, 0.5f);
        
    }
    private void OnDestroy()
    {
        instance.objectInteracted -= ChangePriority;
    }
    public void ChangePriority(int target, int totalProfiles)
    {
        if (target == recency)
        {
            recency = totalProfiles;
        }
        else
        {
            
            if (recency > 0 && target < recency)
            {
                if (recency - totalProfiles > 0)
                {
                    recency -= recency - totalProfiles;
                }
                else
                {
                   recency--;
                }
            }
        }
        GameObject[] temp = new GameObject[6];
        attachedObjects.CopyTo(temp, 0);
        for (int i = 0; i < attachedObjects.Length; i++)
        {
            if (i == 0)
            {
                temp[i].GetComponent<SpriteRenderer>().sortingOrder= 3 * recency;
            }
            else if (i ==1)
            {
                temp[i].GetComponent<SpriteRenderer>().sortingOrder = 1 + 3 * recency;
            }
            else
            {
                temp[i].GetComponent<TextMeshPro>().sortingOrder = 1 + 3 * recency;
            }
        }
    }
    public ProfileScript GetProfile()
    {
        return identity;
    }

    public IEnumerator HoldObject()
    {
        bool selected = true;
        transform.localScale = new Vector3(1f, 1f);
        while (selected)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                selected = false;
            }
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            newPosition.z = 0;
            transform.position = newPosition;
            yield return null;
        }
        GameplayScript.player.Unselect();
        transform.localScale = new Vector3(0.5f, 0.5f);
        yield return null;
    }
}
