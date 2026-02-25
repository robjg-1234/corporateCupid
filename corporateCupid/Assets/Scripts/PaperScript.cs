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
        CabinetScript cab = null;
        if (attachedBoard != null)
        {
            attachedBoard.RemoveProfile(this);
            attachedBoard = null;
        }
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool selected = true;
        //transform.localScale = new Vector3(1.45f, 1.45f);
        //Keeps the object attached to the mouse position.
        while (selected)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                selected = false;
            }
            RaycastHit2D shot = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Cabinet"));
            if (shot.collider != null)
            {
                if (shot.collider.CompareTag("arrow"))
                {
                    cab = shot.collider.gameObject.GetComponentInParent<CabinetScript>();
                    cab.ToggleFile();
                }
            }
            newPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            newPosition.z = 0;
            transform.position = newPosition;
            yield return null;
        }
        GameObject[] momentary = new GameObject[attachedObjects.Length];
        attachedObjects.CopyTo(momentary, 0);
        //Update the sorting order so that it can fall behind the cabinet.
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
        //Handles any valid interaction.
        RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable", "Cabinet"));
        if (hit.collider != null)
        {
            //Checks if the object can get placed into the cabinet.
            if (hit.collider.CompareTag("Cabinet"))
            {
                FolderUnit temp = hit.collider.GetComponent<FolderUnit>();
                if (!temp.SaveProfile(this))
                {
                    returnToDesk(newPosition);
                }
            }
            //Checks for interactions with pinboard whether it can be attached or not.
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
            //Checks interactions with shredder.
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
    /// <summary>
    /// Coroutine that zooms into the selected object.
    /// </summary>
    public IEnumerator Enhance()
    {

        Vector3 prevPos = transform.position;
        //Scales the object up to 1.45 to get a better view of the text.
        transform.localScale = new Vector3(1.45f, 1.45f);
        transform.position = new Vector3(0, 0, 0);
        bool enhanced = true;
        yield return new WaitForSeconds(0.01f);
        //Waits for any of the valid keys to be pressed to exit the animation.
        while (enhanced)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                enhanced = false;
            }
            yield return null;
        }
        //Returns it to the correct location, to the pinboard or the desk.
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
    /// <summary>
    /// Ensures the object is within the boundaries of a valid place.
    /// </summary>
    void returnToDesk(Vector3 prevPos)
    {
        float top = instance.deskCenter.y + instance.size.y / 2f;
        float bottom = instance.deskCenter.y - instance.size.y / 2f;
        float left = instance.deskCenter.x - instance.size.x / 2f;
        float right = instance.deskCenter.x + instance.size.x / 2f;
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
}
