using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    [SerializeField] TMP_Text previewName;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] SpriteRenderer backgroundPaper;
    public void CopyInformation(string prevName, Sprite profIcon)
    {
        previewName.text = prevName;
        sr.sprite = profIcon;
    }
    public void UpdateSort(int curProfiles)
    {
        previewName.GetComponent<TextMeshPro>().sortingOrder = 7 + 3 * curProfiles;
        sr.sortingOrder = 7 + 3 * curProfiles;
        backgroundPaper.sortingOrder = 6 + 3 * curProfiles;
    }
}
