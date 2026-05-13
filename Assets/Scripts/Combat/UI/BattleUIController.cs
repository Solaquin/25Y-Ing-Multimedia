using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleUnitSide
{
    Player,
    Enemy
}

public class BattleUIController : MonoBehaviour
{
    public BattleSystem battleSystem;

    // NUEVO
    [Header("Audio UI")]
    public AudioInteractivo audioUI;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject movesPanel;
    public GameObject itemsPanel;
    public GameObject partyPanel;

    [Header("Movements")]
    public Button[] moveButtons;
    public TextMeshProUGUI[] moveTexts;

    [Header("Items")]
    public Transform itemButtonContainer;
    public GameObject itemButtonPrefab;
    public TextMeshProUGUI itemDescriptionText;

    [Header("Party")]
    public PartyMenuBattleController partyMenu;

    [Header("Icons")]
    public Image playerProfemonIcon;

    [Header("World HP Bars")]
    public GameObject hpBarPrefab;

    private HPBarUI playerBar;
    private HPBarUI enemyBar;

    // 🔥 NUEVO: iconos por tipo de movimiento
    [System.Serializable]
    public class TipoMovimientoIcono
    {
        public TypeSO tipo;
        public Sprite icono;
    }

    [Header("Iconos movimientos")]
    public List<TipoMovimientoIcono> iconosMovimiento;

    private BattleUIState currentState;
    private List<GameObject> spawnedItemButtons = new List<GameObject>();
    private BattleItemSO pendingItem;

    public enum BattleUIState
    {
        Main,
        Moves,
        Items,
        Party
    }

    // 🔊 NUEVO
    void PlayClick()
    {
        if (audioUI != null)
            audioUI.ActivarAudio();
    }

    void OnEnable()
    {
        BattleEvents.OnHPChanged += UpdateHP;
        BattleEvents.OnBattleStarted += InitializeUI;
        BattleEvents.OnActiveUnitChanged += RefreshUI;
        BattleEvents.OnPlayerSwitchRequired += OnSwitchPressed;
        BattleEvents.OnBattleEnded += ClearWorldBars;
    }

    void OnDisable()
    {
        BattleEvents.OnHPChanged -= UpdateHP;
        BattleEvents.OnBattleStarted -= InitializeUI;
        BattleEvents.OnActiveUnitChanged -= RefreshUI;
        BattleEvents.OnPlayerSwitchRequired -= OnSwitchPressed;
        BattleEvents.OnBattleEnded -= ClearWorldBars;
    }

    void InitializeUI()
    {
        SetupMoves();
        UpdateProfemonIcon();

        CrearBarrasWorld();
        UpdateHP();

        ShowPanel(BattleUIState.Main);
    }

    void RefreshUI(BattleUnitSide unitSide)
    {
        switch (unitSide)
        {
            case BattleUnitSide.Player:
                RebuildPlayerBar();
                SetupMoves();
                UpdateProfemonIcon();
                break;

            case BattleUnitSide.Enemy:
                RebuildEnemyBar();
                break;
        }

        UpdateHP();
        ShowPanel(BattleUIState.Main);
    }

    void ShowPanel(BattleUIState state)
    {
        currentState = state;

        mainPanel.SetActive(state == BattleUIState.Main);
        movesPanel.SetActive(state == BattleUIState.Moves);
        itemsPanel.SetActive(state == BattleUIState.Items);
        partyPanel.SetActive(state == BattleUIState.Party);
    }

    public void OnBackPressed()
    {
        PlayClick();
        pendingItem = null;
        ShowPanel(BattleUIState.Main);
    }

    public void OnAttackPressed()
    {
        PlayClick();
        SetupMoves();
        ShowPanel(BattleUIState.Moves);
    }

    public void OnItemPressed()
    {
        PlayClick();
        SetupItems();
        ShowPanel(BattleUIState.Items);
    }

    public void OnSwitchPressed()
    {
        PlayClick();
        pendingItem = null;
        ShowPanel(BattleUIState.Party);
        partyMenu.Open();
    }

    // 🔥 MÉTODO PARA OBTENER ICONO SEGÚN TIPO
    Sprite GetIconoMovimiento(TypeSO tipo)
    {
        foreach (var t in iconosMovimiento)
        {
            if (t.tipo == tipo)
                return t.icono;
        }
        return null;
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
                
            // 🔥 NUEVO: cambiar imagen del botón según tipo
            Image img = moveButtons[i].GetComponent<Image>();
            if (img != null)
            {
                Sprite icono = GetIconoMovimiento(move.moveType);
                if (icono != null)
                    img.sprite = icono;
            }

            int index = i;
            moveButtons[i].onClick.RemoveAllListeners();
            moveButtons[i].onClick.AddListener(() =>
            {
                PlayClick();
                OnMoveSelected(moves[index]);
            });
        }
    }

    void OnMoveSelected(MoveSO move)
    {
        battleSystem.PlayerChooseMove(
            battleSystem.playerUnit,
            battleSystem.enemyUnit,
            move
        );

        ShowPanel(BattleUIState.Main);
    }

    void SetupItems()
    {
        foreach (var go in spawnedItemButtons)
            Destroy(go);

        spawnedItemButtons.Clear();

        if (itemDescriptionText != null)
            itemDescriptionText.text = "";

        var items = ItemInventory.Instance.GetBattleItems();

        foreach (var (item, count) in items)
        {
            GameObject btn = Instantiate(itemButtonPrefab, itemButtonContainer);
            spawnedItemButtons.Add(btn);

            TextMeshProUGUI[] texts = btn.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 1)
                texts[0].text = item.displayName;

            if (texts.Length >= 2)
                texts[1].text = $"x{count}";

            BattleItemSO captured = item;

            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayClick();
                OnItemSelected(captured);
            });
        }
    }

    void OnItemSelected(BattleItemSO item)
    {
        pendingItem = item;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        ShowPanel(BattleUIState.Party);
        partyMenu.OpenForItemTarget(item, OnItemTargetSelected);
    }

    void OnItemTargetSelected(ProfemonInstance target)
    {
        PlayClick();

        if (pendingItem == null) return;

        battleSystem.PlayerChooseItem(pendingItem, target);
        pendingItem = null;

        ShowPanel(BattleUIState.Main);
    }

    public void UpdateHP()
    {
        int currentPlayer = battleSystem.playerUnit.GetCurrentHP();
        int maxPlayer = battleSystem.playerUnit.GetMaxHP();

        int currentEnemy = battleSystem.enemyUnit.GetCurrentHP();
        int maxEnemy = battleSystem.enemyUnit.GetMaxHP();

        //WORLD UI
        if (playerBar != null)
            playerBar.UpdateHP(currentPlayer, maxPlayer);
       
        if (enemyBar != null)
            enemyBar.UpdateHP(currentEnemy, maxEnemy);
    }
    IEnumerator RetryIcon()
    {
        yield return new WaitForSeconds(0.1f);
        UpdateProfemonIcon();
    }

    private bool intentandoCargarIcono = false;

    void UpdateProfemonIcon()
    {
        if (battleSystem.playerUnit == null ||
            battleSystem.playerUnit.Instance == null ||
            battleSystem.playerUnit.Instance.data == null)
        {
            if (!intentandoCargarIcono)
            {
                intentandoCargarIcono = true;
                StartCoroutine(RetryIcon());
            }
            return;
        }

        intentandoCargarIcono = false;

        if (playerProfemonIcon != null)
            playerProfemonIcon.sprite = battleSystem.playerUnit.Instance.data.image;
    }

    void CrearBarrasWorld()
    {
        if (playerBar != null)
            Destroy(playerBar.gameObject);

        if (enemyBar != null)
            Destroy(enemyBar.gameObject);

        // PLAYER

        Transform pCombatUnitTransform = battleSystem.playerUnit.transform;

        GameObject pBar = Instantiate(
            hpBarPrefab,
            pCombatUnitTransform
        );

        WorldHPBar pFollow = pBar.GetComponent<WorldHPBar>();
        pFollow.target = pCombatUnitTransform;

        playerBar = pBar.GetComponent<HPBarUI>();

        playerBar.Setup(
            battleSystem.playerUnit.Instance.data.professorName,
            battleSystem.playerUnit.GetCurrentHP(),
            battleSystem.playerUnit.GetMaxHP()
        );

        // ENEMY

        Transform eCombatUnitTransform = battleSystem.enemyUnit.transform;

        GameObject eBar = Instantiate(
            hpBarPrefab,
            eCombatUnitTransform
        );

        WorldHPBar eFollow = eBar.GetComponent<WorldHPBar>();
        eFollow.target = eCombatUnitTransform;

        enemyBar = eBar.GetComponent<HPBarUI>();

        enemyBar.Setup(
            battleSystem.enemyUnit.Instance.data.professorName,
            battleSystem.enemyUnit.GetCurrentHP(),
            battleSystem.enemyUnit.GetMaxHP()
        );
    }

    void RebuildPlayerBar()
    {
        if (playerBar != null)
            Destroy(playerBar.gameObject);

        Transform pCombatUnitTransform = battleSystem.playerUnit.transform;

        GameObject pBar = Instantiate(
            hpBarPrefab,
            pCombatUnitTransform
        );

        WorldHPBar pFollow = pBar.GetComponent<WorldHPBar>();
        pFollow.target = battleSystem.playerUnit.transform;

        playerBar = pBar.GetComponent<HPBarUI>();

        playerBar.Setup(
            battleSystem.playerUnit.Instance.data.professorName,
            battleSystem.playerUnit.GetCurrentHP(),
            battleSystem.playerUnit.GetMaxHP()
        );
    }

    void RebuildEnemyBar()
    {
        if (enemyBar != null)
            Destroy(enemyBar.gameObject);

        Transform eCombatUnitTransform = battleSystem.enemyUnit.transform;

        GameObject eBar = Instantiate(
            hpBarPrefab,
            eCombatUnitTransform
        );

        WorldHPBar eFollow = eBar.GetComponent<WorldHPBar>();
        eFollow.target = battleSystem.enemyUnit.transform;

        enemyBar = eBar.GetComponent<HPBarUI>();

        enemyBar.Setup(
            battleSystem.enemyUnit.Instance.data.professorName,
            battleSystem.enemyUnit.GetCurrentHP(),
            battleSystem.enemyUnit.GetMaxHP()
        );
    }

    void ClearWorldBars()
    {
        if (playerBar != null)
        {
            Destroy(playerBar.gameObject);
            playerBar = null;
        }

        if (enemyBar != null)
        {
            Destroy(enemyBar.gameObject);
            enemyBar = null;
        }
    }
}