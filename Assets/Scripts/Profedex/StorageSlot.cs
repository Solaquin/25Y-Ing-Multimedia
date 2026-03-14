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
            // Mover de PARTY → STORAGE
            manager.party.Remove(instance);
            manager.storage.Add(instance);

            Debug.Log(instance.data.professorName + " movido a Storage");
        }
        else
        {
            // Mover de STORAGE → PARTY
            if (manager.party.Count >= manager.maxPartySize)
            {
                Debug.Log("Party llena");
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