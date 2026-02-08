using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PaperScript selectedObject = null;
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
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedObject == null)
        {
            RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), new Vector2(0.1f,0.1f), 0, new Vector2(0,0));
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    selectedObject = hit.collider.GetComponent<PaperScript>();
                    instance.CallInteraction(selectedObject.recency);
                    StartCoroutine(selectedObject.HoldObject());
                }
            }
        }
    }

    public void Unselect()
    {
        if (selectedObject != null)
        {
            selectedObject = null;
        }
    }
}
