using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Profemon : MonoBehaviour
{
    [Header("Base Data")]
    public ProfemonData data;

    [Header("Wild Settings")]
    public int level = 1;

    [Header("Wild Level Range")]
    public int minLevel = 1;
    public int maxLevel = 5;

    [Header("Despawn Settings")]
    public float despawnDistance = 30f;

    public bool isCaptured = false;

    private ProfemonInstance instance;
    private Transform player;

    [Header("Wander Area")]
    private Vector3 spawnCenter;
    private Vector3 spawnAreaSize;

    [Header("Navigation Settings")]
    public float wanderInterval = 4f;
    public float idleWaitMin = 2f;   // segundos mínimos en idle
    public float idleWaitMax = 5f;   // segundos máximos en idle
    public float rotationSpeed = 8f; // qué tan rápido gira hacia el destino

    private NavMeshAgent agent;
    private Animator animator;

    private float wanderTimer;
    private float idleWaitTime;    // cuánto tiempo esperar antes de moverse
    private bool isWaiting = true; // empieza esperando

    private bool initialized = false;
    private bool isDespawning = false;

    // Nombres de los parámetros Animator Controller
    private static readonly int AnimIsWalking = Animator.StringToHash("isWalking");

    [SerializeField]
    private NotificationSO captureNotification;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    public void Initialize(ProfemonData data)
    {
        if (data == null)
        {
            Debug.LogError("Initialize recibió data null");
            return;
        }

        this.data = data;

        if (maxLevel < minLevel)
            maxLevel = minLevel;

        level = Random.Range(minLevel, maxLevel + 1);
        instance = new ProfemonInstance(data, level);

        // Empieza en idle con un tiempo de espera aleatorio
        idleWaitTime = Random.Range(idleWaitMin, idleWaitMax);
        isWaiting = true;

        initialized = true;

        PlaySpawnAnimation();
    }

    private void Update()
    {
        if (!initialized) return;

        if (player != null && !isCaptured)
        {
            float sqrDistance = (transform.position - player.position).sqrMagnitude;
            if (sqrDistance > despawnDistance * despawnDistance && !isDespawning)
            {
                StartCoroutine(DespawnThenDestroy());
                return;
            }
        }

        HandleWander();
        UpdateAnimator();
    }

    private void HandleWander()
    {
        if (agent == null || spawnAreaSize == Vector3.zero) return;

        if (isWaiting)
        {
            // Está en idle: cuenta el tiempo de espera
            wanderTimer += Time.deltaTime;

            if (wanderTimer >= idleWaitTime)
            {
                // Terminó de esperar: elige un destino y camina
                Vector3 randomPoint = GetRandomPointInSpawnArea();
                agent.SetDestination(randomPoint);
                agent.isStopped = false;

                wanderTimer = 0f;
                isWaiting = false;
            }
        }
        else
        {
            // Está caminando: rota hacia el destino y revisa si llegó
            RotateTowardsDestination();

            bool arrivedAtDestination = !agent.pathPending
                && agent.remainingDistance <= agent.stoppingDistance;

            if (arrivedAtDestination)
            {
                // Llegó: entra en idle con un nuevo tiempo de espera aleatorio
                agent.isStopped = true;
                idleWaitTime = Random.Range(idleWaitMin, idleWaitMax);
                wanderTimer = 0f;
                isWaiting = true;
            }
        }
    }

    private void RotateTowardsDestination()
    {
        // Solo rota si el agente tiene un destino válido y se está moviendo
        if (agent.velocity.sqrMagnitude < 0.01f) return;

        Vector3 direction = agent.velocity.normalized;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        // Camina si el agente se mueve por encima de un umbral pequeño
        bool walking = !isWaiting && agent.velocity.sqrMagnitude > 0.01f;
        animator.SetBool(AnimIsWalking, walking);
    }

    private Vector3 GetRandomPointInSpawnArea()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

        Vector3 target = spawnCenter + new Vector3(randomX, 0, randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    // --- El resto del código no cambia ---

    public ProfemonInstance GetInstance() => instance;

    public void HideProfessor() => gameObject.SetActive(false);
    public void ShowProfessor() => gameObject.SetActive(true);

    public void ConfirmCapture()
    {
        Debug.Log("ConfirmCapture ejecutado");

        if (data == null) { Debug.LogError("DATA ES NULL"); return; }
        if (PlayerPartyManager.Instance == null) { Debug.LogError("PLAYER PARTY MANAGER ES NULL"); return; }
        if (isCaptured) { Debug.Log("Ya estaba capturado."); return; }
        if (instance == null) { Debug.LogError("INSTANCE ES NULL"); return; }

        PlayerPartyManager.Instance.AddToParty(instance);

        StorageMenuManager menu = FindObjectOfType<StorageMenuManager>();

        if (menu != null) menu.Refresh();

        NotificationData notification = new NotificationData(captureNotification);

        notification.customBody = $"{data.professorName} nivel {instance.level} capturado.";

        NotificationManager.Send(notification);

        Debug.Log(data.professorName + " nivel " + instance.level + " capturado.");

        isCaptured = true;
        Destroy(gameObject);
    }

    public void SetSpawnArea(Vector3 center, Vector3 size)
    {
        spawnCenter = center;
        spawnAreaSize = size;
    }

    private void PlaySpawnAnimation()
    {
        StartCoroutine(SpawnAnim());
    }

    private IEnumerator DespawnThenDestroy()
    {
        yield return StartCoroutine(DespawnAnim());
        Destroy(gameObject);
    }

    private IEnumerator SpawnAnim()
    {
        float duration = 0.4f;
        float elapsed = 0f;

        transform.localScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Curva con rebote: overshoot y luego settle en 1
            float scale = SpawnCurve(t);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    // Curva personalizada: sube rápido, hace overshoot, vuelve a 1
    private float SpawnCurve(float t)
    {
        // Elastic-out simplificado
        float overshoot = 1.70158f;
        t -= 1f;
        return t * t * ((overshoot + 1f) * t + overshoot) + 1f;
    }

    public IEnumerator DespawnAnim()
    {
        if (isDespawning) yield break;
        isDespawning = true;

        // Detener movimiento mientras despawnea
        if (agent != null)
            agent.isStopped = true;

        float duration = 0.35f;
        float elapsed = 0f;
        Vector3 originalScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Shrink con ease-in
            float scale = Mathf.Lerp(1f, 0f, t * t);
            transform.localScale = originalScale * scale;

            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}