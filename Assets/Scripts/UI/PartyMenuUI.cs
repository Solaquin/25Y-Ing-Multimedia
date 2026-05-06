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

    [Header("HP Bars")]
    public Image[] hpBars;

    // NUEVO: iconos de los profemon
    public Image[] profemonIcons;

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

                // (lo dejas por si lo quieres usar después)
                //hpTexts[i].text = $"{p.currentHP}/{p.maxHP}";

                // 🔥 BARRA DE VIDA
                if (hpBars != null && i < hpBars.Length)
                {
                    float hpPercent = (float)p.currentHP / p.maxHP;
                    hpBars[i].fillAmount = hpPercent;

                    // Color dinámico
                    if (hpPercent > 0.5f)
                        hpBars[i].color = Color.green;
                    else if (hpPercent > 0.25f)
                        hpBars[i].color = Color.yellow;
                    else
                        hpBars[i].color = Color.red;
                }

                // 🔥 ICONO
                if (profemonIcons != null && i < profemonIcons.Length)
                {
                    if (p.data != null && p.data.image != null)
                    {
                        profemonIcons[i].sprite = p.data.image;
                        profemonIcons[i].color = Color.white;
                    }
                }

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
                // Ocultar botón
                partyButtons[i].gameObject.SetActive(false);

                // LIMPIAR TEXTO
                if (i < nameTexts.Length)
                    nameTexts[i].text = "";

                if (i < hpTexts.Length)
                    hpTexts[i].text = "";

                // LIMPIAR BARRA
                if (hpBars != null && i < hpBars.Length)
                {
                    hpBars[i].fillAmount = 0f;
                }

                // LIMPIAR IMAGEN
                if (profemonIcons != null && i < profemonIcons.Length)
                {
                    profemonIcons[i].sprite = null;
                    profemonIcons[i].color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    void SelectProfemon(ProfemonInstance instance)
    {
        OnProfemonSelected?.Invoke(instance);
    }
}