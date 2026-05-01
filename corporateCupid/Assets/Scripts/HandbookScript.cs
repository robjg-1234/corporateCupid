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
            AudioManager.instance.Playclip("Notebook");
            current--;
            if (current == 0)
            {
                leftButton.SetActive(false);
                leftSideTitle.text = "Lamia";
                leftText.text = "Typically talks about 'midnight cravings' and being a 'night owl', sometimes their eyes will look 'off' or some 'serpent' features might be visible.";
                rightSideTitle.text = "Mormo";
                rightText.text = "Feeds on the fear of people, though has a special interest in the nightmares of children.";
            }
            else if (current == 1)
            {
                leftSideTitle.text = "Gorgon";
                leftText.text = "These sisters are a very dangerous bunch, They can identified from their serpent hair, though can also be noticed, through skin texture and or colour. Viewing an image of the sisters, should not cause petrification.";
                rightSideTitle.text = "Satyr";
                rightText.text = "Sometimes described as a 'Party Animal', with a strong desire for alcohol; can be indistinguisable from the waist up, except for its goat horns; which it struggles to hide.";
            }
            else if (current == 2)
            {
                rightButton.SetActive(true);
                leftSideTitle.text = "Cyclops";
                leftText.text = "Though very human like, the single; usually central, eye can be a big giveaway, if not disguised well.";
                rightSideTitle.text = "Silenus";
                rightText.text = "Often seen as your typical drunkard, with a strong love for wine, though don't let that fool you as they can have more wisdom than the greatest philosophers.";
            }
        }
    }
    public void TurnRight()
    {
        if (current < 3)
        {
            AudioManager.instance.Playclip("Notebook");
            current++;
            if (current == 1)
            {
                leftButton.SetActive(true);
                leftSideTitle.text = "Gorgon";
                leftText.text = "These sisters are a very dangerous bunch, They can identified from their serpent hair, though can also be noticed, through skin texture and or colour. Viewing an image of the sisters, should not cause petrification.";
                rightSideTitle.text = "Satyr";
                rightText.text = "Sometimes described as a 'Party Animal', with a strong desire for alcohol; can be indistinguisable from the waist up, except for its goat horns; which it struggles to hide.";
            }
            else if (current == 2)
            {
                leftSideTitle.text = "Cyclops";
                leftText.text = "Though very human like, the single; usually central, eye can be a big giveaway, if not disguised well.";
                rightSideTitle.text = "Silenus";
                rightText.text = "Often seen as your typical drunkard, with a strong love for wine, though don't let that fool you as they can have more wisdom than the greatest philosophers.";
            }
            else if (current == 3)
            {
                rightButton.SetActive(false);
                leftSideTitle.text = "Siren";
                leftText.text = "Sirens can come in two forms, an Avian-humanoid or a Mermaid, though they will typically look normal in a headshot, you may still be able to notice minor differences. They typically use their voice to lure their victims.";
                rightSideTitle.text = "Bots";
                rightText.text = "Generic, almost rhythmic sentence structure. Weird tone or strange phrasing. Impossible information (Age, Name, or Emote mismatch).";
            }
        }
    }
}
