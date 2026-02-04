using System.Collections;
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
    public ProfileScript GetProfile()
    {
        return identity;
    }
    public IEnumerator HoldObject()
    {
        while (Mouse.current.leftButton.isPressed)
        {
            Debug.Log("I'm here");
            transform.position = Mouse.current.position.ReadDefaultValue();
            yield return null;
        }
        GameplayScript.player.Unselect();
    }
}
