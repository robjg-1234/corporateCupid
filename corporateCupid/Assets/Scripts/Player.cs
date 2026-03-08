using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PaperScript selectedObject = null;
    AttachedLetter selectedMatch = null;
    PunchCardScript selectedPunch = null;
    GameplayScript instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayScript.player = this;
        instance = GameplayScript.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (instance.dayGoing)
        {
            //Checks for interactable objects and changes the selected object based on it.
            if (Mouse.current.leftButton.wasPressedThisFrame && selectedObject == null && selectedMatch == null && selectedPunch == null)
            {
                RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), LayerMask.GetMask("Default", "Cabinet"));
                if (hit.collider != null)
                {
                    //Checks if the space clicked has a profile so that it gets picked up.
                    if (hit.collider.CompareTag("Cabinet"))
                    {
                        PaperScript temp = hit.collider.GetComponent<FolderUnit>().PickUp();
                        if (temp != null)
                        {
                            selectedObject = temp;
                            instance.CallInteraction(selectedObject.recency);
                        }
                    }
                    //Checks to see if it is a punch card
                    else if (hit.collider.CompareTag("punch"))
                    {
                        selectedPunch = hit.collider.GetComponent<PunchCardScript>();
                        instance.CallInteraction(selectedPunch.recency);
                        StartCoroutine(selectedPunch.HoldObject());
                    }
                    //Checks if the object is a Match.
                    else if (hit.collider.CompareTag("Match"))
                    {
                        selectedMatch = hit.collider.GetComponent<AttachedLetter>();
                        StartCoroutine(selectedMatch.HoldObject());
                    }
                    //Checks if the object selected is a Profile.
                    else if (hit.collider.CompareTag("Interactable"))
                    {
                        selectedObject = hit.collider.GetComponent<PaperScript>();
                        instance.CallInteraction(selectedObject.recency);
                        StartCoroutine(selectedObject.HoldObject());
                    }
                    
                }

            }
            //Handles the zoom interactions.
            else if (Mouse.current.rightButton.wasPressedThisFrame && selectedObject == null && selectedMatch == null)
            {
                RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f, 0.1f), 0, new Vector2(0, 0), LayerMask.GetMask("Default"));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Interactable"))
                    {
                        selectedObject = hit.collider.GetComponent<PaperScript>();
                        instance.CallInteraction(selectedObject.recency);
                        StartCoroutine(selectedObject.Enhance());
                    }
                }
            }
        }
    }
    /// <summary>
    /// Dereferences the selected object.
    /// </summary>
    public void Unselect()
    {
        if (selectedObject != null)
        {
            selectedObject = null;
        }
        if (selectedMatch != null)
        {
            selectedMatch = null;
        }
        if (selectedPunch != null)
        {
            selectedPunch = null;
        }
    }
}
