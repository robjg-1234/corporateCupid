using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CabinetScript : MonoBehaviour
{
    [SerializeField] FolderUnit[] individualUnits;
    [SerializeField] GameObject drawer;
    [SerializeField] SpriteRenderer triangle;
    [SerializeField] SpriteRenderer cabinetFile;
    bool opening = false;
    bool isOpen = false;
    // Update is called once per frame
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
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleFile();
        }
    }
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
        opening = true;
        float t = 0;
        Vector3 finalPos = new(-6.131f, -4.495f, 0);
        Vector3 startingPos = drawer.transform.position;
        Vector3 currentPos = startingPos;
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
        opening = false;
        isOpen = false;
    }
}
