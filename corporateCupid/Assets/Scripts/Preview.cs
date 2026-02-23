using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    [SerializeField] TMP_Text previewName;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer backgroundPaper;
    /// <summary>
    /// Copies information of the profile into the preview.
    /// </summary>
    public void CopyInformation(string prevName, Sprite profIcon)
    {
        previewName.text = prevName;
        sr.sprite = profIcon;
    }
    /// <summary>
    /// Updates the sorting order of all the objects contained in the game object.
    /// </summary>
    public void UpdateSort(int curProfiles)
    {
        previewName.GetComponent<TextMeshPro>().sortingOrder = 7 + 3 * curProfiles;
        sr.sortingOrder = 7 + 3 * curProfiles;
        backgroundPaper.sortingOrder = 6 + 3 * curProfiles;
    }
}
