using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Object class for the matched profiles.
/// </summary>
public class AttachedLetter : MonoBehaviour
{
    public PaperScript prof1;
    public PaperScript prof2;
    public SubmitScript attachedBoard;
    private void Start()
    {
        GameplayScript.instance.dayEnded += EndOfDayCheck;
    }
    private void OnDestroy()
    {
        GameplayScript.instance.dayEnded -= EndOfDayCheck;
        if (prof1 != null)
        {
            Destroy(prof1.gameObject);
        }
        if (prof1 != null)
        {
            Destroy(prof2.gameObject);
        }
        GameplayScript.player.Unselect();
    }
    /// <summary>
    /// Takes both profiles and converts them into the AttachedLetter object.
    /// </summary>
    public void JoinPapers(PaperScript match1, PaperScript match2)
    {
        //Moves them to the center and makes them its children.
        match1.gameObject.transform.position = transform.position;
        match2.gameObject.transform.position = transform.position;
        match1.GetComponent<Collider2D>().enabled = false;
        match2.GetComponent<Collider2D>().enabled = false;
        prof1 = match1;
        prof2 = match2;
        prof1.transform.localScale = new Vector3(1, 1, 1);
        prof2.transform.localScale = new Vector3(1, 1, 1);
        prof1.transform.SetParent(transform);
        prof2.transform.SetParent(transform);
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        ClickMatch();
    }
    /// <summary>
    /// Update the sorting order of the children objects.
    /// </summary>
    void ClickMatch()
    {
        GameplayScript.instance.CallInteraction(prof1.recency);
        GameplayScript.instance.CallInteraction(prof2.recency);
    }
    /// <summary>
    /// Co routine that handles the object interactions with the environment
    /// </summary>
    public IEnumerator HoldObject()
    {
        //Fix for envelopes
        ClickMatch();
        if (attachedBoard != null)
        {
            attachedBoard.isFull = false;
        }
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool selected = true;
        //Makes the object move with the mouse.
        while (selected)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                selected = false;
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
        GameplayScript.player.Unselect();
        //Checks for interactions.
        //Might modify the range of the box cast if not then it will become a raycast
        RaycastHit2D hit = Physics2D.BoxCast(newPosition, new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable"));
        if (hit.collider != null)
        {
            //Handles interaction with the pinboard.
            if (hit.collider.CompareTag("Mailbox"))
            {
                attachedBoard = hit.collider.GetComponent<SubmitScript>();
                if (!attachedBoard.isFull)
                {
                    attachedBoard.available = false;
                    attachedBoard.isFull = true;
                    transform.position = attachedBoard.transform.position;
                }
                else
                {
                    attachedBoard = null;
                    transform.position = GameplayScript.instance.ReturnToDesk(newPosition);
                }
            }
            //Handles the submission of the match.
            else if (hit.collider.CompareTag("Postage"))
            {
                if (attachedBoard != null)
                {
                    attachedBoard.available = true;
                    attachedBoard.isFull = false;
                    attachedBoard = null;
                }
                GameplayScript.mailInstance.Submit(this);
            }
            else
            {
                if (attachedBoard != null)
                {
                    attachedBoard.available = true;
                    attachedBoard.isFull = false;
                    attachedBoard = null;
                }
                transform.position = GameplayScript.instance.ReturnToDesk(newPosition);
            }
        }
        else
        {
            if (attachedBoard != null)
            {
                attachedBoard.available = true;
                attachedBoard.isFull = false;
                attachedBoard = null;
            }
            transform.position = GameplayScript.instance.ReturnToDesk(newPosition);
        }

        yield return null;
    }
    public void EndOfDayCheck()
    {
        GameplayScript.instance.profilesShredded += 2;
        Destroy(gameObject);
    }
}
