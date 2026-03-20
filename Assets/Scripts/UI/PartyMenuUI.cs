using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class PartyMenuUI : MonoBehaviour
{
    public GameObject panel;

    public Button[] partyButtons;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] hpTexts;

    List<ProfemonInstance> currentParty;

    public Action<ProfemonInstance> OnProfemonSelected;

    public void Open(List<ProfemonInstance> party)
    {
        panel.SetActive(true);

        currentParty = party;

        SetupParty();
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    void SetupParty()
    {
        for (int i = 0; i < partyButtons.Length; i++)
        {
            if (i < currentParty.Count)
            {
                ProfemonInstance p = currentParty[i];

                nameTexts[i].text = p.data.professorName;
                //hpTexts[i].text = $"{p.currentHP}/{p.maxHP}";

                int index = i;

                partyButtons[i].onClick.RemoveAllListeners();
                partyButtons[i].onClick.AddListener(() =>
                {
                    SelectProfemon(currentParty[index]);
                });

                partyButtons[i].gameObject.SetActive(true);

                partyButtons[i].interactable = p.currentHP > 0;
            }
            else
            {
                partyButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SelectProfemon(ProfemonInstance instance)
    {
        OnProfemonSelected?.Invoke(instance);
    }
}