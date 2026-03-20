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

    public void OnClick()
    {
        PlayerPartyManager manager = PlayerPartyManager.Instance;

        if (manager.party.Contains(instance))
        {
            // PARTY → STORAGE
            manager.party.Remove(instance);
            manager.storage.Add(instance);

            Debug.Log(instance.data.professorName + " movido a Storage");
        }
        else
        {
            // STORAGE → PARTY

            // 🔒 LIMITADOR DE SEGURIDAD
            if (manager.party.Count >= 6)
            {
                Debug.Log("No puedes tener más de 6 Profemon en la Party");
                return;
            }

            manager.storage.Remove(instance);
            manager.party.Add(instance);

            Debug.Log(instance.data.professorName + " movido a Party");
        }

        StorageMenuManager menu = FindObjectOfType<StorageMenuManager>();

        if (menu != null)
            menu.Refresh();
    }
}