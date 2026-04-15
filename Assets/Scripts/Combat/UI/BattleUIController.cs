using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIController : MonoBehaviour
{
    public BattleSystem battleSystem;

    [Header("General")]
    public TextMeshProUGUI playerProfemonName;
    public TextMeshProUGUI enemyProfemonName;
    public TextMeshProUGUI playerHP;
    public TextMeshProUGUI enemyHP;

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject movesPanel;
    public GameObject itemsPanel;
    public GameObject partyPanel;

    [Header("Movements")]
    public Button[] moveButtons;
    public TextMeshProUGUI[] moveTexts;

    [Header("Items")]
    public Transform itemButtonContainer;   // ScrollView Content donde se instancian los botones
    public GameObject itemButtonPrefab;     // Prefab: Button + TextMeshProUGUI para nombre + TextMeshProUGUI para cantidad
    public TextMeshProUGUI itemDescriptionText; // (opcional) descripción del item seleccionado

    [Header("Party")]
    public PartyMenuBattleController partyMenu;

    private BattleUIState currentState;
    private List<GameObject> spawnedItemButtons = new List<GameObject>();

    // item seleccionado, esperando que el jugador elija target en partyMenu
    private BattleItemSO pendingItem;

    public enum BattleUIState
    {
        Main,
        Moves,
        Items,
        Party
    }

    // ---------------------------
    // EVENTOS
    // ---------------------------
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

    // ---------------------------
    // INICIALIZACIÓN
    // ---------------------------
    void InitializeUI()
    {
        SetupNames();
        SetupMoves();
        UpdateHP();
        ShowPanel(BattleUIState.Main);
    }

    void RefreshUI()
    {
        SetupNames();
        SetupMoves();
        UpdateHP();
        ShowPanel(BattleUIState.Main);
    }

    // ---------------------------
    // CONTROL DE PANELES
    // ---------------------------
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
        pendingItem = null;
        ShowPanel(BattleUIState.Main);
    }

    // ---------------------------
    // BOTONES PRINCIPALES
    // ---------------------------
    public void OnAttackPressed()
    {
        SetupMoves();
        ShowPanel(BattleUIState.Moves);
    }

    public void OnItemPressed()
    {
        SetupItems();
        ShowPanel(BattleUIState.Items);
    }

    public void OnSwitchPressed()
    {
        pendingItem = null;
        ShowPanel(BattleUIState.Party);
        partyMenu.Open();
    }

    // ---------------------------
    // NOMBRES
    // ---------------------------
    void SetupNames()
    {
        playerProfemonName.text = battleSystem.playerUnit.Instance.data.professorName;
        enemyProfemonName.text = battleSystem.enemyUnit.Instance.data.professorName;
    }

    // ---------------------------
    // MOVIMIENTOS
    // ---------------------------
    void SetupMoves()
    {
        CombatUnit playerUnit = battleSystem.playerUnit;
        List<MoveSO> moves = playerUnit.GetMoves();

        for (int i = 0; i < moveButtons.Length; i++)
        {
            bool hasMove = i < moves.Count;
            moveButtons[i].gameObject.SetActive(hasMove);

            if (!hasMove) { moveTexts[i].text = ""; continue; }

            MoveSO move = moves[i];
            moveTexts[i].text = move.moveName;

            int index = i;
            moveButtons[i].onClick.RemoveAllListeners();
            moveButtons[i].onClick.AddListener(() => OnMoveSelected(moves[index]));
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

    // ---------------------------
    // ITEMS
    // ---------------------------
    void SetupItems()
    {
        // Limpiar botones anteriores
        foreach (var go in spawnedItemButtons)
            Destroy(go);

        spawnedItemButtons.Clear();

        if (itemDescriptionText != null)
            itemDescriptionText.text = "";

        var items = ItemInventory.Instance.GetBattleItems();

        if (items.Count == 0)
        {
            // Mostrar mensaje vacío si no hay items
            if (itemDescriptionText != null)
                itemDescriptionText.text = "No tienes objetos.";
            return;
        }

        foreach (var (item, count) in items)
        {
            GameObject btn = Instantiate(itemButtonPrefab, itemButtonContainer);
            spawnedItemButtons.Add(btn);

            // Buscar los textos del prefab por nombre o índice
            // Asume que el prefab tiene al menos un TMP_Text para el nombre
            TextMeshProUGUI[] texts = btn.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 1)
                texts[0].text = item.displayName;

            if (texts.Length >= 2)
                texts[1].text = $"x{count}";

            // Deshabilitar si no se puede usar en ningún miembro del equipo vivo
            bool anyUsable = IsItemUsableOnAnyAlly(item);
            btn.GetComponent<Button>().interactable = anyUsable;

            // Capturar para el closure
            BattleItemSO captured = item;

            btn.GetComponent<Button>().onClick.AddListener(() =>
                OnItemSelected(captured)
            );
        }
    }

    /// <summary>
    /// Devuelve true si el item puede usarse en al menos un Profemon vivo del equipo.
    /// </summary>
    bool IsItemUsableOnAnyAlly(BattleItemSO item)
    {
        var party = PlayerPartyManager.Instance.GetParty();

        foreach (var member in party)
        {
            if (item.CanUseOn(member))
                return true;
        }

        return false;
    }

    void OnItemSelected(BattleItemSO item)
    {
        pendingItem = item;

        if (itemDescriptionText != null)
            itemDescriptionText.text = item.description;

        // Abrir el menú de equipo para elegir sobre quién usar el item
        ShowPanel(BattleUIState.Party);
        partyMenu.OpenForItemTarget(item, OnItemTargetSelected);
    }

    /// <summary>
    /// Callback que recibe partyMenu cuando el jugador elige un Profemon como target del item.
    /// Si el item no aplica sobre ese target, vuelve al panel de items sin consumir el turno.
    /// </summary>
    void OnItemTargetSelected(ProfemonInstance target)
    {
        if (pendingItem == null) return;

        if (!pendingItem.CanUseOn(target))
        {
            // Mostrar mensaje en el textbox y volver al panel de items sin consumir el turno
            StartCoroutine(ShowNoEffectMessage(target));
            return;
        }

        battleSystem.PlayerChooseItem(pendingItem, target);

        pendingItem = null;

        ShowPanel(BattleUIState.Main);
    }

    IEnumerator ShowNoEffectMessage(ProfemonInstance target)
    {
        yield return StartCoroutine(
            BattleMessenger.Show($"No tendrá efecto en {target.data.professorName}.")
        );

        SetupItems();
        ShowPanel(BattleUIState.Items);
    }

    // ---------------------------
    // HP
    // ---------------------------
    public void UpdateHP()
    {
        playerHP.text =
            $"HP: {battleSystem.playerUnit.GetCurrentHP()} / {battleSystem.playerUnit.GetMaxHP()}";

        enemyHP.text =
            $"HP: {battleSystem.enemyUnit.GetCurrentHP()} / {battleSystem.enemyUnit.GetMaxHP()}";
    }
}