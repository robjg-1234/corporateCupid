using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaperScript : MonoBehaviour
{
    GameplayScript instance;
    ProfileScript identity;
    public int recency = 0;
    SubmitScript attachedBoard;
    [SerializeField] GameObject[] attachedObjects;
    TMP_Text dislikeText;
    TMP_Text likedText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = GameplayScript.instance;
        identity = instance.PickProfile();
        recency = instance.currentProfiles;
        instance.objectInteracted += ChangePriority;
        transform.localScale = new Vector3(0.5f, 0.5f);
        dislikeText = attachedObjects[5].GetComponent<TMP_Text>();
        likedText = attachedObjects[4].GetComponent<TMP_Text>();
        likedText.text = identity.GetFormattedLikes();
        dislikeText.text = identity.GetFormattedDislikes();
    }
    private void OnDestroy()
    {
        instance.currentProfiles--;
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
        GameObject[] temp = new GameObject[attachedObjects.Length];
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
        RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue,LayerMask.GetMask("Interactable"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Mailbox"))
            {
                attachedBoard = hit.collider.GetComponent<SubmitScript>();
                if (!attachedBoard.AddProfile(this))
                {
                    attachedBoard = null;
                }
                //To-do
                //Fix position of the paper
                //Add a way to replace existing paper
            }
        }
        else
        {
            if (attachedBoard != null)
            {
                attachedBoard.RemoveProfile(this);
                attachedBoard = null;
            }
        }
        transform.localScale = new Vector3(0.5f, 0.5f);
        yield return null;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.greenYellow;
    //    Gizmos.DrawWireCube(transform.position, attachedObjects[0].transform.localScale*transform.localScale.x);
    //}
}
