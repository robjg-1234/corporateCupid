
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    [SerializeField] LayerMask _layers;
    [SerializeField] GameObject envelopePrefab;
    PaperScript selectedObject = null;
    AttachedLetter selectedMatch = null;
    PunchCardScript selectedPunch = null;
    EnvelopeScript selectedEnvelope = null;
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
            if (!instance.paused && !HandbookScript.instance.open)
            {
                //Checks for interactable objects and changes the selected object based on it.
                if (Mouse.current.leftButton.wasPressedThisFrame && selectedObject == null && selectedMatch == null && selectedPunch == null && selectedEnvelope == null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, float.MaxValue, _layers);
                    if (hit.collider != null)
                    {
                        //Checks if the space clicked has a profile so that it gets picked up.
                        if (hit.collider.CompareTag("Cabinet"))
                        {
                            selectedObject = hit.collider.GetComponent<FolderUnit>().PickUp();
                            if (selectedObject != null)
                            {
                                if (instance.day == 0 && instance.stepNumber == 2)
                                {
                                    instance.stepDone = true;
                                    instance.stepNumber++;
                                }
                                instance.CallInteraction(selectedObject.recency);
                                StartCoroutine(selectedObject.HoldObject());
                            }
                        }
                        else if (hit.collider.CompareTag("Envelope"))
                        {
                            if (instance.day!=0 || instance.stepNumber >= 3)
                            {
                                selectedEnvelope = Instantiate(envelopePrefab, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Quaternion.identity).GetComponent<EnvelopeScript>();
                                instance.CallInteraction(selectedEnvelope.recency);
                                StartCoroutine(selectedEnvelope.HoldObject());
                            }
                        }
                        else if (hit.collider.CompareTag("Shredder"))
                        {
                            selectedObject = hit.collider.GetComponent<ShredderScript>().GrabPaper();
                            if (selectedObject != null)
                            {
                                instance.CallInteraction(selectedObject.recency);
                                StartCoroutine(selectedObject.HoldObject());
                            }
                        }
                        else if (hit.collider.CompareTag("button"))
                        {
                            if (instance.day != 0 || instance.stepNumber >= 5)
                            {
                                GameplayScript.shredInstance.ActivateShredder();
                            }
                        }
                        //Checks to see if it is the arrow to open/close cabinet
                        else if (hit.collider.CompareTag("arrow"))
                        {
                            CabinetScript cab = hit.collider.gameObject.GetComponentInParent<CabinetScript>();
                            cab.ToggleFile();
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
                            if (instance.day != 0 || instance.stepNumber >= 4)
                            {
                                selectedMatch = hit.collider.GetComponent<AttachedLetter>();
                                StartCoroutine(selectedMatch.HoldObject());
                            }
                        }
                        //Checks if the object selected is a Profile.
                        else if (hit.collider.CompareTag("Interactable"))
                        {
                            selectedObject = hit.collider.GetComponent<PaperScript>();
                            instance.CallInteraction(selectedObject.recency);
                            StartCoroutine(selectedObject.HoldObject());
                        }
                        else if (hit.collider.CompareTag("handbook"))
                        {
                            HandbookScript.instance.OpenBook();
                        }
                        else if (hit.collider.CompareTag("Clock"))
                        {
                            instance.JumpToNextStage();
                        }
                    }

                }
                //Handles the zoom interactions.
                else if (Mouse.current.rightButton.wasPressedThisFrame && selectedObject == null && selectedMatch == null && selectedPunch == null && selectedEnvelope == null)
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, float.MaxValue, LayerMask.GetMask("Default"));
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
                else if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    instance.PauseGame();

                }
            }
            else
            {
                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                {
                    if (HandbookScript.instance.open)
                    {
                        HandbookScript.instance.CloseBook();
                    }
                    else
                    {
                        instance.PauseGame();
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
        if (selectedEnvelope != null)
        {
            selectedEnvelope = null;
        }
    }
}
