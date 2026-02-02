using Unity.VisualScripting;
using UnityEngine;

public class PaperScript : MonoBehaviour
{
    ProfileScript identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public ProfileScript OnDrop()
    {

        return identity;
    }
}
