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
    TMP_Text nameText;
    public SpriteRenderer spriteRend;


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
        nameText = attachedObjects[2].GetComponent<TMP_Text>();
        spriteRend = attachedObjects[1].GetComponent<SpriteRenderer>();
        //Whenever profile icons get added, pass the path to the icon
        nameText.text = identity.characterName;
        likedText.text = identity.GetFormattedLikes();
        dislikeText.text = identity.GetFormattedDislikes();
        ChangePriority(recency, instance.currentProfiles);
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
        int frontVal = 0;
        if (target == recency)
        {
            recency = totalProfiles;
            frontVal= 8;
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
                temp[i].GetComponent<SpriteRenderer>().sortingOrder = 3 * recency + frontVal;
            }
            else if (i == 1)
            {
                temp[i].GetComponent<SpriteRenderer>().sortingOrder = 1 + 3 * recency + frontVal;
            }
            else
            {
                temp[i].GetComponent<TextMeshPro>().sortingOrder = 1 + 3 * recency + frontVal;
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
        if (attachedBoard != null)
        {
            attachedBoard.RemoveProfile(this);
            attachedBoard = null;
        }
        Vector3 newPosition = new Vector3(0, 0, 0);
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
        GameObject[] momentary = new GameObject[attachedObjects.Length];
        attachedObjects.CopyTo(momentary, 0);
        for (int i = 0; i < attachedObjects.Length; i++)
        {
            if (i == 0)
            {
                momentary[i].GetComponent<SpriteRenderer>().sortingOrder = 3 * recency;
            }
            else if (i == 1)
            {
                momentary[i].GetComponent<SpriteRenderer>().sortingOrder = 1 + 3 * recency;
            }
            else
            {
                momentary[i].GetComponent<TextMeshPro>().sortingOrder = 1 + 3 * recency;
            }
        }
        GameplayScript.player.Unselect();
        RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable", "Cabinet"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Cabinet"))
            {
                FolderUnit temp = hit.collider.GetComponent<FolderUnit>();
                if (!temp.SaveProfile(this))
                {
                    returnToDesk(newPosition);
                }
            }
            else if (hit.collider.CompareTag("Mailbox"))
            {
                attachedBoard = hit.collider.GetComponent<SubmitScript>();
                if (!attachedBoard.AddProfile(this))
                {
                    attachedBoard = null;
                    returnToDesk(newPosition);
                }
                //To-do
                //Add a way to replace existing paper
            }
            else if (hit.collider.CompareTag("Shredder"))
            {
                Destroy(this.gameObject);
                Debug.Log("BYE");
                //Fully implement the shredder which is going to have two stages the place and the click to shred, I don't know how this affects the other part of the game
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
        }
        else
        {
            returnToDesk(newPosition);
        }


    }

    public IEnumerator Enhance()
    {

        Vector3 prevPos = transform.position;
        transform.localScale = new Vector3(1.45f, 1.45f);
        transform.position = new Vector3(0, 0, 0);
        bool enhanced = true;
        yield return new WaitForSeconds(0.01f);
        while (enhanced)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                enhanced = false;
            }
            yield return null;
        }
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        if (attachedBoard != null)
        {
            transform.position = prevPos;
        }
        else
        {
            returnToDesk(prevPos);
        }
        GameplayScript.player.Unselect();
    }

    void returnToDesk(Vector3 prevPos)
    {
        float top = -1.2f;
        float bottom = -2.8f;
        float left = -6f;
        float right = 6f;
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
