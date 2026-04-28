using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnvelopeScript : MonoBehaviour
{
    [SerializeField] SpriteRenderer back;
    [SerializeField] SpriteRenderer front;
    GameplayScript instance;
    public int recency = 0;
    private void Start()
    {
        instance = GameplayScript.instance;
        instance.currentProfiles++;
        recency = instance.currentProfiles;
        ChangePriority(recency, instance.currentProfiles);
        instance.objectInteracted += ChangePriority;
    }
    private void OnDestroy()
    {
        GameplayScript.player.Unselect();
        StopAllCoroutines();
        instance.currentProfiles--;
        instance.objectInteracted -= ChangePriority;
    }
    public void TransitionOrder(int lowestPriority)
    {
        back.sortingOrder = 3*lowestPriority-1;
        front.sortingOrder = 3*lowestPriority + 13;
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
                recency--;
            }
        }
        back.sortingOrder = 3 * recency + frontVal;
        front.sortingOrder = 4 * recency + frontVal;
    }

    public IEnumerator HoldObject()
    {
        Vector3 newPosition = new Vector3(0, 0, 0);
        bool selected = true;
        //Keeps the object attached to the mouse position.
        while (selected)
        {
            if (this != null)
            {
                if (Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    selected = false;
                }
                newPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                newPosition.z = 0;
                transform.position = newPosition;
            }
            yield return null;
        }
        while (instance.paused)
        {
            yield return null;
        }
        GameplayScript.player.Unselect();
        //Handles any valid interaction.
        RaycastHit2D hit = Physics2D.Raycast(newPosition, Vector2.zero, float.MaxValue, LayerMask.GetMask("Interactable"));
        if (hit.collider != null)
        {
            Debug.Log("Hi");
            if (hit.collider.CompareTag("Mailbox"))
            {
                if (!GameplayScript.mailInstance.CreateMatch(this))
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
