using System;
using UnityEngine;

public class PartyMenuBattleController : MonoBehaviour
{
    public PartyMenuUI menu;
    public BattleSystem battleSystem;

    private bool isItemTargetMode;
    private Action<ProfemonInstance> onItemTargetSelected;

    void OnEnable()
    {
        menu.OnProfemonSelected += HandleSelection;
    }

    void OnDisable()
    {
        menu.OnProfemonSelected -= HandleSelection;
    }

    // Modo switch — igual que antes
    public void Open()
    {
        isItemTargetMode = false;
        onItemTargetSelected = null;
        menu.Open(PlayerPartyManager.Instance.party);
    }

    // Modo item target — abre el mismo menú pero con otro callback
    public void OpenForItemTarget(BattleItemSO item, Action<ProfemonInstance> callback)
    {
        isItemTargetMode = true;
        onItemTargetSelected = callback;
        menu.Open(PlayerPartyManager.Instance.party);
    }

    void HandleSelection(ProfemonInstance instance)
    {
        menu.Close();

        if (isItemTargetMode)
        {
            onItemTargetSelected?.Invoke(instance);
            isItemTargetMode = false;
            onItemTargetSelected = null;
        }
        else
        {
            battleSystem.PlayerChooseSwitch(instance);
        }
    }
}