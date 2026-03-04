using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PunchCardScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Collider2D objectCollider;
    GameplayScript instance;
    Vector3 initialPos;
    public int recency = 0;
    private void Start()
    {
        instance = GameplayScript.instance;
        initialPos = transform.position;
        ChangePriority(recency, instance.currentProfiles);
        instance.objectInteracted += ChangePriority;
        instance.dayEnded += EndOfDayUpdate;
    }
    private void OnDestroy()
    {
        instance.objectInteracted -= ChangePriority;
        instance.dayEnded -= EndOfDayUpdate;
    }

    public void ChangePriority(int target, int totalProfiles)
    {
        int frontVal = 0;
        if (target == recency)
        {
            recency = totalProfiles;
            frontVal = 8;
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
        sr.sortingOrder = 3 * recency + frontVal;
    }

    public IEnumerator HoldObject()
    {
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool selected = true;
        //Keeps the object attached to the mouse position.
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
        sr.sortingOrder = 3 * recency;
        GameplayScript.player.Unselect();
        //Handles any valid interaction.
        RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), float.MaxValue, LayerMask.GetMask("Interactable"));
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Clock"))
            {
                instance.clockedIn = true;
                //when the clock becomes its own object, I'll send the punch card behind it. For now it just gets disabled.
                gameObject.SetActive(false);
            }
            else
            {
                transform.position = instance.ReturnToDesk(newPosition);
            }
        }
        else
        {
            transform.position = instance.ReturnToDesk(newPosition);
        }


    }

    public void EndOfDayUpdate()
    {
        gameObject.transform.position = initialPos;
        ChangePriority(recency, instance.currentProfiles);
        gameObject.SetActive(true);
    }
}
