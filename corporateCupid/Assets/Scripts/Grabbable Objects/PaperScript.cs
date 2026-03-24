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
    GameObject groundObject;
    GameObject verticalObject;
    public bool saved = false;
    public SpriteRenderer spriteRend;
    [SerializeField] Sprite liftedPaper;
    Sprite groundSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = GameplayScript.instance;
        identity = instance.PickProfile();
        recency = instance.currentProfiles;
        instance.objectInteracted += ChangePriority;
        instance.dayEnded += EndOfDayCheck;
        transform.localScale = new Vector3(0.5f, 0.5f);
        TMP_Text dislikeText = attachedObjects[5].GetComponent<TMP_Text>();
        TMP_Text likedText = attachedObjects[4].GetComponent<TMP_Text>();
        TMP_Text bioText = attachedObjects[3].GetComponent<TMP_Text>();
        TMP_Text nameText = attachedObjects[2].GetComponent<TMP_Text>();
        TMP_Text infoText = attachedObjects[6].GetComponent<TMP_Text>();
        spriteRend = attachedObjects[1].GetComponent<SpriteRenderer>();
        //Whenever profile icons get added, pass the path to the icon
        groundObject = attachedObjects[7];
        verticalObject = attachedObjects[0];
        nameText.text = identity.characterName;
        likedText.text = identity.GetFormattedLikes();
        dislikeText.text = identity.GetFormattedDislikes();
        bioText.text = identity.GetBio();
        infoText.text = identity.GetInfo();
        groundSprite = groundObject.GetComponent<SpriteRenderer>().sprite;
        ChangePriority(recency, instance.currentProfiles);
    }
    private void OnDestroy()
    {
        instance.currentProfiles--;
        instance.objectInteracted -= ChangePriority;
        instance.dayEnded -= EndOfDayCheck;

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
            if (i == 0 ||i ==8)
            {
                temp[i].GetComponent<SpriteRenderer>().sortingOrder = 3 * recency + frontVal;
            }
            else if (i == 1 || i==7)
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
        SpriteRenderer paperType = groundObject.GetComponent<SpriteRenderer>();
        paperType.sprite = liftedPaper;
        attachedObjects[8].transform.localPosition += new Vector3(0,0.271f,0);
        groundObject.SetActive(true);
        verticalObject.SetActive(false);
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
            if (Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                selected = false;
            }
            RaycastHit2D shot = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, float.MaxValue, LayerMask.GetMask("Cabinet"));
            if (shot.collider != null)
            {
                if (shot.collider.CompareTag("arrow"))
                {
                    cab = shot.collider.gameObject.GetComponentInParent<CabinetScript>();
                    if (!cab.isOpen)
                    {
                        cab.ToggleFile();
                    }
                }
            }
            newPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            newPosition.z = 0;
            transform.position = newPosition;
            yield return null;
        }
        while (GameplayScript.instance.paused)
        {
            yield return null;
        }
        GameObject[] momentary = new GameObject[attachedObjects.Length];
        attachedObjects.CopyTo(momentary, 0);
        //Update the sorting order so that it can fall behind the cabinet.
        for (int i = 0; i < attachedObjects.Length; i++)
        {
            if (i == 0 || i == 8)
            {
                momentary[i].GetComponent<SpriteRenderer>().sortingOrder = 3 * recency;
            }
            else if (i == 1 || i == 7)
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
        //Might modify the range of the box cast if not then it will become a raycast
        RaycastHit2D hit = Physics2D.BoxCast(newPosition, new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable", "Cabinet"));
        if (hit.collider != null)
        {
            //Checks if the object can get placed into the cabinet.
            if (hit.collider.CompareTag("Cabinet"))
            {
                FolderUnit temp = hit.collider.GetComponent<FolderUnit>();
                if (!temp.SaveProfile(this))
                {
                    transform.position = instance.ReturnToDesk(newPosition);
                }
            }
            //Checks for interactions with pinboard whether it can be attached or not.
            else if (hit.collider.CompareTag("Mailbox"))
            {
                attachedBoard = hit.collider.GetComponent<SubmitScript>();
                if (!attachedBoard.AddProfile(this))
                {
                    attachedBoard = null;
                    transform.position = instance.ReturnToDesk(newPosition);
                }
                else
                {
                    groundObject.SetActive(false);
                    verticalObject.SetActive(true);
                }
                //To-do
                //Add a way to replace existing paper
            }
            //Checks interactions with shredder.
            else if (hit.collider.CompareTag("Shredder"))
            {
                ShredderScript shred = hit.collider.GetComponent<ShredderScript>();
                if (!shred.AddPaper(this))
                {
                    transform.position = instance.ReturnToDesk(newPosition);
                }
            }
            else
            {
                if (attachedBoard != null)
                {
                    attachedBoard.RemoveProfile(this);
                    attachedBoard = null;
                }
                transform.position = instance.ReturnToDesk(newPosition);
            }
        }
        else
        {
            if (attachedBoard != null)
            {
                attachedBoard.RemoveProfile(this);
                attachedBoard = null;
            }
            transform.position = instance.ReturnToDesk(newPosition);
        }
        paperType.sprite = groundSprite;
        attachedObjects[8].transform.localPosition -= new Vector3(0, 0.271f, 0);

    }
    /// <summary>
    /// Coroutine that zooms into the selected object.
    /// </summary>
    public IEnumerator Enhance()
    {
        groundObject.SetActive(false);
        verticalObject.SetActive(true);
        Vector3 prevPos = transform.position;
        //Scales the object up to 1.45 to get a better view of the text.
        transform.localScale = new Vector3(1.5f, 1.5f);
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
            groundObject.SetActive(true);
            verticalObject.SetActive(false);
            transform.position = instance.ReturnToDesk(prevPos);
        }
        GameplayScript.player.Unselect();
        
    }

    public void EndOfDayCheck()
    {
        if (!saved)
        {
            instance.dailyShred++;
            Destroy(gameObject);
        }
    }
}
