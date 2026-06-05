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

    private bool hasBeenKO = false;

    [Header("Visual")]
    [SerializeField] Transform modelParent;

    GameObject currentModel;
    Animator currentAnimator;

    private void Awake()
    {
        if (startOnAwake)
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
        ResetKOFlag();

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

    public bool HasBeenKO => hasBeenKO;
    public void MarkAsKO()
    {
        hasBeenKO = true;
    }

    public void ResetKOFlag()
    {
        hasBeenKO = false;
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

    public bool AddStageModifier(StatType stat, int amount)
    {
        int oldStage = statStages[stat];

        int newStage = Mathf.Clamp(
            oldStage + amount,
            -6,
            6
        );

        statStages[stat] = newStage;

        return oldStage != newStage;
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
    public bool ApplyStatus(StatusEffectSO status, int duration)
    {
        if (status == null)
            return false;

        instance.ValidateStatus();

        if (instance.ActiveStatus != null)
            return false;

        instance.TrySetStatus(status, duration);

        status.OnApply(this);

        return true;
    }

    public bool TryPreventAction(BattleActionType actionType, out string message)
    {
        message = "";

        instance.ValidateStatus();

        if (instance.ActiveStatus == null)
            return false;

        if (instance.ActiveStatus.effect.PreventAction(actionType))
        {
            message = instance.ActiveStatus.effect.GetPreventActionMessage(this);

            return true;
        }

        return false;
    }

    public List<string> TickStatus()
    {
        List<string> messages = new();

        instance.ValidateStatus();

        if (instance.ActiveStatus == null)
            return messages;

        int hpBefore = GetCurrentHP();

        instance.ActiveStatus.effect.OnTurnEnd(this);

        int hpAfter = GetCurrentHP();

        if (hpAfter < hpBefore)
        {
            messages.Add(
                $"{Instance.data.professorName} sufrió daño por {instance.ActiveStatus.effect.statusType}."
            );
        }

        // -1 = persistente
        if (instance.ActiveStatus.remainingTurns == -1)
            return messages;

        instance.ActiveStatus.remainingTurns--;

        if (instance.ActiveStatus.remainingTurns <= 0)
        {
            messages.AddRange(CureStatus());
        }

        return messages;
    }

    public List<string> CureStatus()
    {
        List<string> messages = new();

        instance.ValidateStatus();

        if (instance.ActiveStatus == null)
            return messages;

        string statusName =
            instance.ActiveStatus.effect.statusType.ToString();

        if (instance.ActiveStatus.effect != null)
            instance.ActiveStatus.effect.OnRemove(this);

        messages.Add(
            $"{Instance.data.professorName} ya no está {statusName}."
        );

        instance.CureStatusCondition();

        return messages;
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

    public IEnumerator PlayFaint()
    {
        if (currentAnimator == null) yield break;

        currentAnimator.SetTrigger(BattleAnimKeys.Faint);

        yield return new WaitUntil(() =>
            currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Faint")
        );

        yield return new WaitUntil(() =>
            currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        yield return new WaitForSeconds(0.4f);

        // Desaparece
        yield return StartCoroutine(DespawnAnimation());
    }

    public IEnumerator PlayByTag(string tag, float exitTime = 0.8f, bool waitForExit = true)
    {
        if (currentAnimator == null) yield break;

        currentAnimator.SetTrigger(tag);

        // Esperar a entrar al estado
        yield return new WaitUntil(() =>
            currentAnimator.GetCurrentAnimatorStateInfo(0).IsTag(tag)
        );

        if (waitForExit)
        {
            // Esperar hasta cierto punto
            yield return new WaitUntil(() =>
                currentAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= exitTime
            );
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
            if (e.vfx != null && e.vfx.Length > 0)
            {
                GameObject prefab = e.vfx[Random.Range(0, e.vfx.Length)];

                Instantiate(
                    prefab,
                    transform.position,
                    Quaternion.identity
                );
            }

            // AUDIO CORREGIDO
            if (e.sfx != null && e.sfx.Length > 0)
            {
                AudioClip clip =
                    e.sfx[Random.Range(0, e.sfx.Length)];

                AudioSource.PlayClipAtPoint(
                    clip,
                    transform.position
                );
            }
        }
    }
}
