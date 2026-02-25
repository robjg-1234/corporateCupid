using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CabinetScript : MonoBehaviour
{
    [SerializeField] FolderUnit[] individualUnits;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject drawer;
    [SerializeField] SpriteRenderer triangle;
    [SerializeField] SpriteRenderer cabinetFile;
    bool opening = false;
    bool isOpen = false;
    private void Start()
    {
        GameplayScript.instance.objectInteracted += ReorderLayer;
    }
    private void OnDestroy()
    {
        GameplayScript.instance.objectInteracted -= ReorderLayer;
    }
    void Update()
    {
        //As of right now the way to bring it out is with tab key, will probably get moved to the player so that it can autoamitcally get drawn out while hovering with a profile.
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleFile();
        }
    }
    /// <summary>
    /// Updates the sorting order of all its objects so that they can be on top of the objects in the table. It gets updated when any object gets interacted with.
    /// </summary>
    public void ReorderLayer(int temp, int totalProfiles)
    {
        triangle.sortingOrder = 4 + 3 * totalProfiles;
        cabinetFile.sortingOrder = 4 + 3 * totalProfiles;
        foreach (FolderUnit unit in individualUnits)
        {
            unit.SortLayer(totalProfiles);
        }
    }
    IEnumerator SlideOpen()
    {
        triangle.gameObject.GetComponent<Collider2D>().enabled = false;
        opening = true;
        float t = 0;
        Vector3 finalPos = new(-6.131f, -4.495f, 0);
        Vector3 startingPos = drawer.transform.position;
        Vector3 currentPos = startingPos;
        //Sliding animation of the cabinet opening, using linear interpolation.
        while (currentPos != finalPos)
        {
            t += 4 * Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            currentPos = new Vector3(Mathf.Lerp(startingPos.x, finalPos.x, t), startingPos.y, 0);
            drawer.transform.position = currentPos;
            yield return null;
        }
        //Activates the collider of the drawer spaces so that the player can interact with them.
        foreach (FolderUnit file in individualUnits)
        {
            file.gameObject.GetComponent<Collider2D>().enabled = true;
        }
        opening = false;
        isOpen = true;
    }
    public void ToggleFile()
    {
        if (!opening && !isOpen)
        {
            StartCoroutine(SlideOpen());
        }
        else if (!opening && isOpen)
        {
            StartCoroutine(SlideClose());
        }
    }
    IEnumerator SlideClose()
    {
        opening = true;
        float t = 0;
        Vector3 finalPos = new(-11.67f, -4.495f, 0);
        Vector3 startingPos = drawer.transform.position;
        Vector3 currentPos = startingPos;
        //The opposite of the slide open coroutine.
        foreach (FolderUnit file in individualUnits)
        {
            file.gameObject.GetComponent<Collider2D>().enabled = false;
        }
        while (currentPos != finalPos)
        {
            t += 4 * Time.deltaTime;
            if (t > 1)
            {
                t = 1;
            }
            currentPos = new Vector3(Mathf.Lerp(startingPos.x, finalPos.x, t), startingPos.y, 0);
            drawer.transform.position = currentPos;
            yield return null;
        }
        triangle.gameObject.GetComponent<Collider2D>().enabled = true;
        opening = false;
        isOpen = false;
    }
}
