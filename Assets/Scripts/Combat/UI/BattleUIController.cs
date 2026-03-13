using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUIController : MonoBehaviour
{
    public BattleSystem battleSystem;

    public TextMeshProUGUI playerProfemonName;
    public TextMeshProUGUI enemyProfemonName;

    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI enemyHP;

    public GameObject movesPanel;
    public Button[] moveButtons;
    public TextMeshProUGUI[] moveTexts;

    public PartyMenuBattleController partyMenu;


    void OnEnable()
    {
        BattleEvents.OnHPChanged += UpdateHP;
        BattleEvents.OnBattleStarted += InitializeUI;
        BattleEvents.OnActiveUnitChanged += RefreshUI;
        BattleEvents.OnPlayerSwitchRequired += OnSwitchPressed;

    }

    void OnDisable()
    {
        BattleEvents.OnHPChanged -= UpdateHP;
        BattleEvents.OnBattleStarted -= InitializeUI;
        BattleEvents.OnActiveUnitChanged -= RefreshUI;
        BattleEvents.OnPlayerSwitchRequired -= OnSwitchPressed;
    }


    void InitializeUI()
    {
        SetupNames();
        SetupMoves();
        UpdateHP();
    }

    void RefreshUI()
    {
        SetupNames();
        SetupMoves();
        UpdateHP();
        ShowMovesPanel();
    }

    void SetupNames()
    {
        CombatUnit playerUnit = battleSystem.playerUnit;
        CombatUnit enemyUnit = battleSystem.enemyUnit;

        playerProfemonName.text =
            playerUnit.Instance.data.professorName;

        enemyProfemonName.text =
            enemyUnit.Instance.data.professorName;
    }

    void SetupMoves()
    {
        CombatUnit playerUnit = battleSystem.playerUnit;

        List<MoveSO> moves = playerUnit.GetMoves();

        for (int i = 0; i < moveButtons.Length; i++)
        {
            bool hasMove = i < moves.Count;

            moveButtons[i].gameObject.SetActive(hasMove);

            if (!hasMove)
            {
                moveTexts[i].text = "";
                continue;
            }

            MoveSO move = moves[i];

            moveTexts[i].text = move.moveName;

            int index = i;

            moveButtons[i].onClick.RemoveAllListeners();
            moveButtons[i].onClick.AddListener(() =>
            {
                OnMoveSelected(moves[index]);
            });
        }
    }

    void ShowMovesPanel()
    {
        movesPanel.SetActive(true);
    }

    void HideMovesPanel()
    {
        movesPanel.SetActive(false);
    }

    void OnMoveSelected(MoveSO move)
    {
        CombatUnit playerUnit = battleSystem.playerUnit;
        CombatUnit enemyUnit = battleSystem.enemyUnit;

        battleSystem.PlayerChooseMove(playerUnit, enemyUnit, move);
    }

    public void UpdateHP()
    {
        CombatUnit playerUnit = battleSystem.playerUnit;
        CombatUnit enemyUnit = battleSystem.enemyUnit;

        playerHP.text =
            $"HP: {playerUnit.GetCurrentHP()} / {playerUnit.GetMaxHP()}";

        enemyHP.text =
            $"HP: {enemyUnit.GetCurrentHP()} / {enemyUnit.GetMaxHP()}";
    }

    public void OnSwitchPressed()
    {
        HideMovesPanel();

        partyMenu.Open();
    }
}