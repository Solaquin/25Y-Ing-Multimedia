using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageSlot : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    ProfemonInstance instance;

    public void Setup(ProfemonInstance p)
    {
        instance = p;

        icon.sprite = p.data.image;
        nameText.text = p.data.professorName;
    }
}