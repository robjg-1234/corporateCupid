using UnityEngine;

public class FolderUnit : MonoBehaviour
{
    [SerializeField] Preview attachedPreview;
    [SerializeField] SpriteRenderer squareRend;
    PaperScript savedProfile = null;
    /// <summary>
    /// Updates its sorting order and calls a similar function in its children.
    /// </summary>
    public void SortLayer(int curProfiles)
    {
        attachedPreview.UpdateSort(curProfiles);
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
            attachedPreview.CopyInformation(savedProfile.GetProfile().characterName, savedProfile.spriteRend.sprite);
            attachedPreview.gameObject.SetActive(true);
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
        if (savedProfile != null)
        {
            //Deactivates the preview.
            attachedPreview.gameObject.SetActive(false);
            savedProfile.gameObject.SetActive(true);
            StartCoroutine(savedProfile.HoldObject());
            savedProfile = null;
        }
        return savedProfile;
    }
}
