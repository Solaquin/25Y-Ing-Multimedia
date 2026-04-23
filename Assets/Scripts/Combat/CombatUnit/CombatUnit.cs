using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatUnit : MonoBehaviour
{
    [Header("Setup for editor debug")]
    public ProfemonData data;
    public int level = 1;

    private ProfemonInstance instance;
    public ProfemonInstance Instance => instance;

    Dictionary<StatType, int> statStages = new Dictionary<StatType, int>();

    [SerializeField] bool startOnAwake;
    [SerializeField] int currentHPDebug;

    [Header("Visual")]
    [SerializeField] Transform modelParent;

    GameObject currentModel;
    Animator currentAnimator;

    private void Awake()
    {
        if(startOnAwake)
        {
            instance = new ProfemonInstance(data, level);

            ResetStages();

            currentHPDebug = instance.currentHP;

            PrintStats();
        }

    }

    // ================================
    // INICIALIZAR
    // ================================
    public void InitializeFromInstance(ProfemonInstance instance)
    {
        this.instance = instance;

        ResetStages();

        currentHPDebug = instance.currentHP;

        PrintStats();

        SetupVisual();
    }

    public IEnumerator SwapProfemon(ProfemonInstance newInstance, bool isInitialSpawn = false)
    {
        if (newInstance == null || newInstance.data == null)
        {
            Debug.LogError("SwapProfemon recibió instancia inválida");
            yield break;
        }

        // 1) Salida (solo si ya había algo y no es el spawn inicial)
        if (!isInitialSpawn && currentModel != null)
        {
            yield return StartCoroutine(DespawnAnimation());
        }

        // 2) Actualizar datos (lógica)
        this.instance = newInstance;
        ResetStages();

        // 3) Entrada (siempre)
        yield return StartCoroutine(SpawnAnimation(isInitialSpawn));
    }

    // ================================
    // VIDA
    // ================================

    public void TakeDamage(int amount)
    {
        instance.currentHP -= amount;
        instance.currentHP = Mathf.Clamp(
            instance.currentHP,
            0,
            instance.maxHP
        );

        BattleEvents.OnHPChanged?.Invoke();

        Debug.Log($"{name} recibió {amount} de daño. HP: {instance.currentHP}");
        currentHPDebug = instance.currentHP;
    }

    public void Heal(int amount)
    {
        instance.currentHP += amount;
        instance.currentHP = Mathf.Clamp(
            instance.currentHP,
            0,
            instance.maxHP
        );

        BattleEvents.OnHPChanged?.Invoke();

        Debug.Log($"{name} se curó {amount}. New HP:{instance.currentHP}");
        currentHPDebug = instance.currentHP;
    }

    public bool IsAlive()
    {
        return instance.currentHP > 0;
    }

    public int GetCurrentHP()
    {
        return instance.currentHP;
    }

    public int GetMaxHP()
    {
        return instance.maxHP;
    }

    // ================================
    // STATS
    // ================================

    int GetBaseStat(StatType stat)
    {
        switch (stat)
        {
            case StatType.Attack: return instance.attack;
            case StatType.Defense: return instance.defense;
            case StatType.Speed: return instance.speed;
            case StatType.Accuracy: return instance.accuracy;
            case StatType.Evasion: return instance.evasion;
        }

        return 0;
    }

    public int GetStat(StatType stat)
    {
        int baseValue = GetBaseStat(stat);

        int stage = statStages[stat];

        float multiplier = GetStageMultiplier(stage);

        return Mathf.RoundToInt(baseValue * multiplier);
    }

    public void AddStageModifier(StatType stat, int amount)
    {
        int currentStage = statStages[stat];

        currentStage += amount;

        currentStage = Mathf.Clamp(currentStage, -6, 6);

        statStages[stat] = currentStage;

        Debug.Log($"{name} {stat} stage ahora es {currentStage}");
    }

    public int GetStage(StatType stat)
    {
        return statStages[stat];
    }

    float GetStageMultiplier(int stage)
    {
        if (stage >= 0)
            return (2f + stage) / 2f;

        return 2f / (2f - stage);
    }

    public void ResetStages()
    {
        statStages.Clear();

        foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
        {
            statStages[stat] = 0;
        }
    }

    void PrintStats()
    {
        Debug.Log($"{name} Attack Base: {instance.attack}");
        Debug.Log($"{name} Defense Base: {instance.defense}");
        Debug.Log($"{name} Speed Base: {instance.speed}");
        Debug.Log($"{name} Accuracy Base: {instance.accuracy}");
        Debug.Log($"{name} Evasion Base: {instance.evasion}");
    }

    // ================================
    // ESTADOS
    // ================================
    public void ApplyStatus(StatusEffectSO status, int duration)
    {
        if (instance.activeStatus != null)
        {
            Debug.Log($"{name} ya tiene un estado.");
            return;
        }

        instance.activeStatus = new StatusInstance(status, duration);

        status.OnApply(this);

        Debug.Log($"{name} ahora tiene {status.statusType}");
    }

    public bool TryPreventAction(BattleActionType actionType, out string message)
    {
        message = "";

        if (instance.activeStatus == null)
            return false;

        if (instance.activeStatus.effect.PreventAction(actionType))
        {
            message =
                $"{name} está {instance.activeStatus.effect.statusType} y no puede moverse.";

            return true;
        }

        return false;
    }

    public void TickStatus()
    {
        if (instance.activeStatus == null) return;

        instance.activeStatus.effect.OnTurnEnd(this);

        // -1 = persistente, no cuenta turnos
        if (instance.activeStatus.remainingTurns == -1) return;

        instance.activeStatus.remainingTurns--;

        if (instance.activeStatus.remainingTurns <= 0)
        {
            Debug.Log($"{name} ya no está {instance.activeStatus.effect.statusType}");
            instance.activeStatus = null;
        }
    }

    public void CureStatus()
    {
        if (instance.activeStatus == null) return;

        instance.activeStatus.effect.OnRemove(this);
        Debug.Log($"{name} se curó de {instance.activeStatus.effect.statusType}");
        instance.activeStatus = null;
    }

    // ================================
    // TIPOS
    // ================================

    public float GetTypeMultiplier(TypeSO attackType)
    {
        float multiplier = 1f;

        foreach (var defenseType in instance.types)
        {
            multiplier *= TypeChart.Instance.GetMultiplier(
                attackType,
                defenseType
            );
        }

        return multiplier;
    }

    // ================================
    // MOVIMIENTOS
    // ================================
    public List<MoveSO> GetMoves()
    {
        return instance.currentMoves;
    }

    public MoveSO GetRandomMove()
    {
        var moves = instance.currentMoves;

        if (moves == null || moves.Count == 0)
            return null;

        return moves[Random.Range(0, moves.Count)];
    }

    // ================================
    // VISUAL
    // ================================

    void SetupVisual()
    {
        if (instance == null || instance.data == null)
        {
            Debug.LogError("Instance o data null en CombatUnit");
            return;
        }

        if (currentModel != null)
            Destroy(currentModel);

        GameObject prefab = instance.data.battlePrefab;

        if (prefab == null)
        {
            Debug.LogError("battlePrefab no asignado en " + instance.data.professorName);
            return;
        }

        currentModel = Instantiate(prefab, modelParent);

        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;
        currentModel.transform.localScale = Vector3.one;

        currentAnimator = currentModel.GetComponent<Animator>();
    }

    public void ClearVisual()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
            currentAnimator = null;
        }
    }

    IEnumerator DespawnAnimation()
    {
        if (currentModel == null)
            yield break;

        float t = 0f;
        Vector3 startScale = currentModel.transform.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            currentModel.transform.localScale =
                Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }

        Destroy(currentModel);
        currentModel = null;
        currentAnimator = null;
    }

    IEnumerator SpawnAnimation(bool isInitialSpawn)
    {
        GameObject prefab = instance.data.battlePrefab;

        currentModel = Instantiate(prefab, modelParent);

        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity;

        currentAnimator = currentModel.GetComponent<Animator>();

        float t = 0f;

        if (isInitialSpawn)
        {
            // aparición más “suave”
            currentModel.transform.localScale = Vector3.zero;

            while (t < 1f)
            {
                t += Time.deltaTime * 2f;
                currentModel.transform.localScale =
                    Vector3.Lerp(Vector3.zero, Vector3.one, t);

                yield return null;
            }
        }
        else
        {
            // aparición más rápida tipo cambio
            currentModel.transform.localScale = Vector3.zero;

            while (t < 1f)
            {
                t += Time.deltaTime * 4f;
                currentModel.transform.localScale =
                    Vector3.Lerp(Vector3.zero, Vector3.one, t);

                yield return null;
            }
        }
    }

    // ================================
    // Animations
    // ================================
    public IEnumerator PlayByTag(string tag)
    {
        if (currentAnimator == null) yield break;

        // Dispara el trigger según tag 
        currentAnimator.SetTrigger(tag);

        // Espera a entrar al estado con ese tag
        yield return new WaitUntil(() =>
            currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag(tag)
        );

        // Espera a terminar
        yield return new WaitUntil(() =>
            currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f
        );
    }

    public IEnumerator PlayVisualEvents(List<VisualEvent> events, CombatUnit user, CombatUnit target)
    {
        foreach (var e in events)
        {
            CombatUnit unit = e.onTarget ? target : user;

            // animación
            if (!string.IsNullOrEmpty(e.animTag))
                yield return StartCoroutine(unit.PlayByTag(e.animTag));

            // VFX
            if (e.vfx != null)
                Instantiate(e.vfx, unit.transform.position, Quaternion.identity);

            // sonido
            if (e.sfx != null)
            {
                AudioSource.PlayClipAtPoint(e.sfx, unit.transform.position);
            }
        }
    }
    public IEnumerator PlayVisualPhase(List<VisualEvent> events, VisualPhase phase, CombatUnit user, CombatUnit target)
    {
        foreach (var e in events)
        {
            if (e.phase != phase)
                continue;

            CombatUnit unit = e.onTarget ? target : user;

            // Animación
            if (!string.IsNullOrEmpty(e.animTag))
            {
                yield return StartCoroutine(unit.PlayByTag(e.animTag));
            }

            // VFX
            if (e.vfx != null)
            {
                Instantiate(e.vfx, unit.transform.position, Quaternion.identity);
            }

            // Audio
            if (e.sfx != null)
            {
                AudioSource.PlayClipAtPoint(e.sfx, unit.transform.position);
            }
        }
    }
}