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
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Debug.Log(Mouse.current.position.ReadValue());
        }
        if (Mouse.current.leftButton.wasPressedThisFrame && selectedObject == null)
        {
            RaycastHit2D hit = Physics2D.BoxCast(Mouse.current.position.ReadValue(), new Vector2(1f,1f), 0, new Vector2(0,-1));
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    selectedObject = hit.collider.GetComponent<PaperScript>();
                    StartCoroutine(selectedObject.HoldObject());
                    Debug.Log("Hi");
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(Mouse.current.position.ReadUnprocessedValue(), new Vector3(1f, 1f, 0));
    }
}
