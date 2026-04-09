using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CabinetScript : MonoBehaviour
{
    [SerializeField] public FolderUnit[] individualUnits;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject drawer;
    [SerializeField] GameObject shredder;
    [SerializeField] SpriteRenderer[] attachedSprites;
    [SerializeField] SpriteRenderer cabinetRenderer;
    bool opening = false;
    public bool isOpen = false;
    private void Start()
    {
        GameplayScript.instance.objectInteracted += ReorderLayer;
        GameplayScript.instance.dayEnded += EmptyCabinet;
    }
    private void OnDestroy()
    {
        GameplayScript.instance.objectInteracted -= ReorderLayer;
        GameplayScript.instance.dayEnded -= EmptyCabinet;
    }
    void Update()
    {
        //As of right now the way to bring it out is with tab key, will probably get moved to the player so that it can automatically get drawn out while hovering with a profile.
        if (Keyboard.current.tabKey.wasPressedThisFrame && GameplayScript.instance.dayGoing)
        {
            ToggleFile();
        }
    }
    /// <summary>
    /// Updates the sorting order of all its objects so that they can be on top of the objects in the table. It gets updated when any object gets interacted with.
    /// </summary>
    public void ReorderLayer(int temp, int totalProfiles)
    {
        cabinetRenderer.sortingOrder = 4 + 3 * totalProfiles;
        foreach(SpriteRenderer sr in attachedSprites)
        {
            sr.sortingOrder = 5 + 3 * totalProfiles;
        }
        foreach (FolderUnit unit in individualUnits)
        {
            unit.SortLayer(totalProfiles);
        }
    }
    IEnumerator SlideOpen()
    {
        ReorderLayer(0, GameplayScript.instance.currentProfiles+3);
        arrow.gameObject.GetComponent<Collider2D>().enabled = false;
        shredder.GetComponent<Collider2D>().enabled = false;
        opening = true;
        float t = 0;
        Vector3 finalPos = new(-5.92000008f, -4.49499989f, 0);
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
        arrow.gameObject.GetComponent<Collider2D>().enabled = true;
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
        arrow.gameObject.GetComponent<Collider2D>().enabled = false;
        opening = true;
        float t = 0;
        Vector3 finalPos = new Vector3(-11.6700001f, -4.49499989f, 0);
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
        arrow.gameObject.GetComponent<Collider2D>().enabled = true;
        shredder.GetComponent<Collider2D>().enabled = true;
        opening = false;
        isOpen = false;
    }

    public void EmptyCabinet()
    {
        if (GameplayScript.instance.day == 0)
        {
            foreach (FolderUnit unit in individualUnits)
            {
                unit.ClearInventory();
            }
        }
    }
}
