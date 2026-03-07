using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BattleUIController : MonoBehaviour
{
    public BattleSystem battleSystem;

    public CombatUnit player;
    public CombatUnit enemy;

    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI enemyHP;

    public Button[] moveButtons;
    public TextMeshProUGUI[] moveTexts;


    void OnEnable()
    {
        BattleEvents.OnHPChanged += UpdateHP;
    }

    void OnDisable()
    {
        BattleEvents.OnHPChanged -= UpdateHP;
    }


    void Start()
    {
        SetupMoves();
        UpdateHP();
    }

    void SetupMoves()
    {
        List<MoveSO> moves = player.GetMoves();

        for (int i = 0; i < moveButtons.Length; i++)
        {
            if (i < moves.Count)
            {
                MoveSO move = moves[i];

                moveTexts[i].text = move.moveName;

                int index = i;

                moveButtons[i].onClick.AddListener(() =>
                {
                    OnMoveSelected(moves[index]);
                });
            }
            else
            {
                moveButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void OnMoveSelected(MoveSO move)
    {
        battleSystem.PlayerChooseMove(player, enemy, move);

        UpdateHP();
    }

    public void UpdateHP()
    {
        playerHP.text =
            $"HP: {player.GetCurrentHP()} / {player.GetMaxHP()}";

        enemyHP.text =
            $"HP: {enemy.GetCurrentHP()} / {enemy.GetMaxHP()}";
    }
}