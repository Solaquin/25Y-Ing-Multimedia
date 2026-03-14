using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMenuManager : MonoBehaviour
{
    public Image[] icons;
    public TMP_Text[] names;

    private int selectedIndex = -1;

    void OnEnable()
    {
        RefreshParty();
    }

    public void RefreshParty()
    {
        if (PlayerPartyManager.Instance == null)
        {
        
            return;
        }

        var party = PlayerPartyManager.Instance.party;

        if (party == null)
        {
            Debug.LogError("La lista party es null");
            return;
        }

        for (int i = 0; i < icons.Length; i++)
        {
            if (i < party.Count)
            {
                icons[i].enabled = true;
                icons[i].sprite = party[i].data.image;
                names[i].text = party[i].data.professorName;
            }
            else
            {
                icons[i].enabled = false;
                names[i].text = "-";
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (selectedIndex == -1)
        {
            selectedIndex = index;
            Debug.Log("Slot seleccionado: " + index);
            return;
        }

        Swap(selectedIndex, index);
        selectedIndex = -1;

        RefreshParty();
    }

    void Swap(int a, int b)
    {
        var party = PlayerPartyManager.Instance.party;

        if (a >= party.Count || b >= party.Count)
            return;

        var temp = party[a];
        party[a] = party[b];
        party[b] = temp;

        Debug.Log("Intercambiados " + a + " y " + b);
    }
}