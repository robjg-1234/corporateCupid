using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttachedLetter : MonoBehaviour
{
    public PaperScript prof1;
    public PaperScript prof2;
    public SubmitScript attachedBoard;
    private void OnDestroy()
    {
        Destroy(prof1.gameObject);
        Destroy(prof2.gameObject);
        GameplayScript.player.Unselect();
    }
    public void JoinPapers(PaperScript match1, PaperScript match2)
    {
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
    void ClickMatch()
    {
        GameplayScript.instance.CallInteraction(prof1.recency);
        GameplayScript.instance.CallInteraction(prof2.recency);
    }
    public IEnumerator HoldObject()
    {
        ClickMatch();
        if (attachedBoard != null)
        {
            attachedBoard.isFull = false;
        }
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool selected = true;
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
        RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Mailbox"))
            {
                attachedBoard = hit.collider.GetComponent<SubmitScript>();
                if (!attachedBoard.isFull)
                {
                    attachedBoard.available = false;
                    attachedBoard.isFull = true;
                    transform.position = attachedBoard.transform.position;
                }
            }
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
                returnToDesk(newPosition);
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
            returnToDesk(newPosition);
        }

        yield return null;
    }
    void returnToDesk(Vector3 prevPos)
    {
        float top = -1.2f;
        float bottom = -2.8f;
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
}
