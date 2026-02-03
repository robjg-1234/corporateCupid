using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaperScript : MonoBehaviour
{
    GameplayScript instance;
    ProfileScript identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = GameplayScript.instance;
        identity = instance.PickProfile();
    }
    private void OnDestroy()
    {
        instance = null;
        identity = null;
    }
    private void OnMouseDrag()
    {
        Debug.Log("hi");
    }
    public ProfileScript GetProfile()
    {
        return identity;
    }
}
