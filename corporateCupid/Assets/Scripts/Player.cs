using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    PaperScript selectedObject = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameplayScript.player = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedObject == null)
        {
            RaycastHit2D hit = Physics2D.BoxCast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue())+new Vector3(0.25f,0.25f, 0), new Vector2(0.5f,0.5f), 0, new Vector2(0,-1));
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    selectedObject = hit.collider.GetComponent<PaperScript>();
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
