using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfedexSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    public void Setup(ProfemonData data, bool discovered, Sprite unknown)
    {
        if (discovered)
        {
            icon.sprite = data.image;
            nameText.text = data.professorName;
        }
        else
        {
            icon.sprite = unknown;
            nameText.text = "????";
        }
    }
}