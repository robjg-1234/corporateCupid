using UnityEngine;

public class FolderUnit : MonoBehaviour
{
    [SerializeField] SpriteRenderer squareRend;
    PaperScript savedProfile = null;
    [SerializeField] Sprite fullSprite;
    [SerializeField] Sprite emptySprite;
    /// <summary>
    /// Updates its sorting order and calls a similar function in its children.
    /// </summary>
    public void SortLayer(int curProfiles)
    {
        //attachedPreview.UpdateSort(curProfiles);
        squareRend.sortingOrder = 5 + 3 * curProfiles;
    }
    /// <summary>
    /// Handles the addition of the profiles into the cabinet, and activates a preview version of the profile. Returns true if the profile is accepted.
    /// </summary>
    public bool SaveProfile(PaperScript newProf)
    {
        if (savedProfile == null)
        {
            savedProfile = newProf;
            newProf.saved = true;
            squareRend.sprite = fullSprite;
            newProf.gameObject.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Returns the profile saved in this object.
    /// </summary>
    public PaperScript PickUp()
    {
        PaperScript sending = null;
        if (savedProfile != null)
        {
            squareRend.sprite = emptySprite;
            savedProfile.gameObject.SetActive(true);
            savedProfile.saved = false;
            sending = savedProfile;
            savedProfile = null;
        }
        return sending;
    }
    /// <summary>
    /// Empty file spot;
    /// </summary>
    public void ClearInventory()
    {
        if (savedProfile != null)
        {
            squareRend.sprite = emptySprite;
            Destroy(savedProfile.gameObject);
            savedProfile = null;
        }
    }
}
