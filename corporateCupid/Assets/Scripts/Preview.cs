using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    [SerializeField] TMP_Text previewName;
    [SerializeField] SpriteRenderer sr;
    public void CopyInformation(string prevName, Sprite profIcon)
    {
        previewName.text = prevName;
        sr.sprite = profIcon;
    }
}
