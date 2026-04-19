using TMPro;
using UnityEngine;

public class HandbookScript : MonoBehaviour
{
    [SerializeField] GameObject bookObject;
    [SerializeField] GameObject leftButton;
    [SerializeField] GameObject rightButton;
    [SerializeField] TMP_Text leftSideTitle;
    [SerializeField] TMP_Text rightSideTitle;
    [SerializeField] TMP_Text leftText;
    [SerializeField] TMP_Text rightText;
    public static HandbookScript instance;
    int current = 0;
    public bool open = false;
    private void Start()
    {
        instance = this;
    }
    public void OpenBook()
    {
        bookObject.SetActive(true);
        open = true;
    }
    public void CloseBook()
    {
        bookObject.SetActive(false);
        open = false;
    }

    public void TurnLeft()
    {
        if (current > 0)
        {
            current--;
            if (current == 0)
            {
                leftButton.SetActive(false);
                leftSideTitle.text = "Lamia";
                leftText.text = "Human features, feeds on young men and loves to scare and or eat kids.";
                rightSideTitle.text = "Mormo";
                rightText.text = "Does not like Nurses and Mothers, who use her name to scare children into behaving; lost their child and will seek a replacement.";
            }
            else if (current == 1)
            {
                rightButton.SetActive(true);
                leftSideTitle.text = "Nymph";
                leftText.text = "Will appear young and have a strong love for nature, though only in a very specific way, depending on their variance (Naids, Dyads, Oreads and Oceanids); their profile image may also show ‘growths’ aligned with their variant.";
                rightSideTitle.text = "Satyr";
                rightText.text = "Satyrs love partying; they may be able to blend in with waist up shots, however they have no fool proof way of hiding their goat horns.";
            }
        }
    }
    public void TurnRight()
    {
        if (current < 2)
        {
            current++;
            if (current == 1)
            {
                leftButton.SetActive(true);
                leftSideTitle.text = "Nymph";
                leftText.text = "Will appear young and have a strong love for nature, though only in a very specific way, depending on their variance (Naids, Dyads, Oreads and Oceanids); their profile image may also show ‘growths’ aligned with their variant.";
                rightSideTitle.text = "Satyr";
                rightText.text = "Satyrs love partying; they may be able to blend in with waist up shots, however they have no fool proof way of hiding their goat horns.";
            }
            else if (current == 2)
            {
                rightButton.SetActive(false);
                leftSideTitle.text = "Cyclops";
                leftText.text = "Will attempt to hide their, single eye using a multitude of ways; glasses, eye patches, paint or fake eyes.";
                rightSideTitle.text = "Bots";
                rightText.text = "Generic, almost rhythmic sentence structure. Weird tone or strange phrasing. Impossible information (Age, Name, or Emote mismatch).";
            }
        }
    }
}
