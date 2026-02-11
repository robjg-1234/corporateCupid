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
    /// <summary>
    /// Compares the priority of thepaper that was interacted with, and updates its order inside the layer.
    /// </summary>
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

    /// <summary>
    /// Coroutine to handle paper movement and other interactions (pin to pinboard, shred).
    /// </summary>
    public IEnumerator HoldObject()
    {
        Vector3 newPosition = new Vector3(0,0,0);
        bool selected = true;
        //transform.localScale = new Vector3(1.45f, 1.45f);
        while (selected)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                selected = false;
            }
            newPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
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
            returnToDesk(newPosition);
        }
        yield return null;
    }
    
    public IEnumerator Enhance()
    {
        transform.localScale = new Vector3(1.45f, 1.45f);
        transform.position = new Vector3(0, 0, 0);
        while (!Mouse.current.leftButton.wasPressedThisFrame || !Mouse.current.rightButton.wasPressedThisFrame || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            yield return null;
        }
    }

    void returnToDesk(Vector3 prevPos)
    {
        float top = -1f;
        float bottom = -3f;
        float left = -2.25f;
        float right = 6.25f;
        if (prevPos.x < left)
        {
            prevPos.x = left;
        }
        if (prevPos.x > right)
        {
            prevPos.x = right;
        }
        if (prevPos.y < bottom)
        {
            prevPos.y = bottom;
        }
        if (prevPos.y > top)
        {
            prevPos.y = top;
        }
        transform.position = prevPos;
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.greenYellow;
    //    Gizmos.DrawWireCube(transform.position, attachedObjects[0].transform.localScale*transform.localScale.x);
    //}
}
