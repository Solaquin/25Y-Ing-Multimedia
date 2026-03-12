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

    public Button[] moveButtons;
    public TextMeshProUGUI[] moveTexts;


    void OnEnable()
    {
        BattleEvents.OnHPChanged += UpdateHP;
        BattleEvents.OnBattleStarted += InitializeUI;
        BattleEvents.OnActiveUnitChanged += RefreshUI;

    }

    void OnDisable()
    {
        BattleEvents.OnHPChanged -= UpdateHP;
        BattleEvents.OnBattleStarted -= InitializeUI;
        BattleEvents.OnActiveUnitChanged -= RefreshUI;
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
            if (i < moves.Count)
            {
                MoveSO move = moves[i];

                moveTexts[i].text = move.moveName;

                int index = i;

                moveButtons[i].onClick.RemoveAllListeners();
                moveButtons[i].onClick.AddListener(() =>
                {
                    OnMoveSelected(moves[index]);
                });
            }
            else
            {
                moveButtons[i].gameObject.SetActive(i < moves.Count);
            }
        }
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
}