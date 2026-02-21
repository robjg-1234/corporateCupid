using UnityEngine;

public class FolderUnit : MonoBehaviour
{
    [SerializeField] Preview attachedPreview;
    PaperScript savedProfile = null;
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
    public PaperScript PickUp()
    {
        if (savedProfile != null)
        {
            attachedPreview.gameObject.SetActive(false);
            savedProfile.gameObject.SetActive(true);
            StartCoroutine(savedProfile.HoldObject());
            savedProfile = null;
        }
        return savedProfile;
    }
}
