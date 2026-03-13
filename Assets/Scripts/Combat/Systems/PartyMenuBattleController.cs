using UnityEngine;

public class PartyMenuBattleController : MonoBehaviour
{
    public PartyMenuUI menu;
    public BattleSystem battleSystem;

    void OnEnable()
    {
        menu.OnProfemonSelected += HandleSelection;
    }

    void OnDisable()
    {
        menu.OnProfemonSelected -= HandleSelection;
    }

    public void Open()
    {
        menu.Open(PlayerPartyManager.Instance.party);
    }

    void HandleSelection(ProfemonInstance instance)
    {
        menu.Close();

        battleSystem.PlayerChooseSwitch(instance);
    }
}