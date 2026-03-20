using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyMenuManager : MonoBehaviour
{
    public Image[] icons;
    public TMP_Text[] names;

    public Image[] hpBars;
    public TMP_Text[] hpTexts;

    public GameObject[] selectedIndicators;

    private int selectedIndex = -1;

    void OnEnable()
    {
        RefreshParty();
    }

    public void RefreshParty()
    {
        if (PlayerPartyManager.Instance == null)
            return;

        var party = PlayerPartyManager.Instance.party;

        for (int i = 0; i < icons.Length; i++)
        {
            if (i < party.Count)
            {
                var p = party[i];

                icons[i].enabled = true;
                icons[i].sprite = p.data.image;

                names[i].text = p.data.professorName;

                float hpPercent = (float)p.currentHP / p.maxHP;

                hpBars[i].fillAmount = hpPercent;

                hpTexts[i].text = p.currentHP + " / " + p.maxHP;
            }
            else
            {
                icons[i].enabled = false;
                names[i].text = "-";
                hpBars[i].fillAmount = 0;
                hpTexts[i].text = "";
            }

            selectedIndicators[i].SetActive(false);
        }

        selectedIndex = -1;
    }

    public void SelectSlot(int index)
    {
        var party = PlayerPartyManager.Instance.party;

        if (index >= party.Count)
            return;

        if (selectedIndex == -1)
        {
            selectedIndex = index;
            selectedIndicators[index].SetActive(true);
        }
        else
        {
            Swap(selectedIndex, index);

            selectedIndicators[selectedIndex].SetActive(false);

            selectedIndex = -1;

            RefreshParty();
        }
    }

    void Swap(int a, int b)
    {
        var party = PlayerPartyManager.Instance.party;

        var temp = party[a];
        party[a] = party[b];
        party[b] = temp;
    }
}