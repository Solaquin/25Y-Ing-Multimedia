using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfedexSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    private ProfemonData data;
    private bool discovered;

    public Sprite unknown;

    public void Setup(ProfemonData data, bool discovered, Sprite unknown)
    {
        this.data = data;
        this.discovered = discovered;

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

    public void OnClick()
    {
        if (!discovered) return;

        FindObjectOfType<VRMenuManager>().OpenProfesorDetalle(data);
    }
}