using UnityEditor.Experimental.GraphView;
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

    private NavMeshAgent agent;
    private float wanderTimer;

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError("ProfemonData no asignado en " + gameObject.name);
            return;
        }

        // Seguridad por si alguien pone mal los valores
        if (maxLevel < minLevel)
            maxLevel = minLevel;

        //  Nivel aleatorio dentro del rango
        level = Random.Range(minLevel, maxLevel + 1);

        instance = new ProfemonInstance(data, level);

        // Buscar jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();

        Debug.Log(data.professorName + " salvaje generado nivel " + level);
    }

    private void Update()
    {
        if (player != null && !isCaptured)
        {
            float sqrDistance = (transform.position - player.position).sqrMagnitude;

            if (sqrDistance > despawnDistance * despawnDistance)
            {
                Destroy(gameObject);
                return;
            }
        }

        HandleWander();
    }

    private void HandleWander()
    {
        if (agent == null)
            return;

        if (spawnAreaSize == Vector3.zero)
            return;

        wanderTimer += Time.deltaTime;

        if (wanderTimer >= wanderInterval)
        {
            Vector3 randomPoint = GetRandomPointInSpawnArea();
            agent.SetDestination(randomPoint);
            wanderTimer = 0f;
        }
    }

    private Vector3 GetRandomPointInSpawnArea()
    {
        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

        Vector3 target = spawnCenter + new Vector3(randomX, 0, randomZ);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    public ProfemonInstance GetInstance()
    {
        return instance;
    }

    public void HideProfessor()
    {
        gameObject.SetActive(false);
    }

    public void ShowProfessor()
    {
        gameObject.SetActive(true);
    }

    public void ConfirmCapture()
    {
        Debug.Log("ConfirmCapture ejecutado");

        if (data == null)
        {
            Debug.LogError("DATA ES NULL");
            return;
        }

        if (PlayerPartyManager.Instance == null)
        {
            Debug.LogError("PLAYER PARTY MANAGER ES NULL");
            return;
        }

        if (isCaptured)
        {
            Debug.Log("Ya estaba capturado.");
            return;
        }

        if (instance == null)
        {
            Debug.LogError("INSTANCE ES NULL");
            return;
        }
        // Verificar espacio en party
        if (PlayerPartyManager.Instance.HasSpaceInParty())
        {
            PlayerPartyManager.Instance.AddToParty(instance);

            Debug.Log(data.professorName +
                " nivel " + instance.level +
                " añadido a la Party.");
        }
        else
        {
            Debug.Log("Party llena. Registrado en Profedex pero liberado.");

            if (ProfedexManager.Instance != null)
                ProfedexManager.Instance.RegisterProfessor(data);
        }

        isCaptured = true;
        Destroy(gameObject);
    }

    public void SetSpawnArea(Vector3 center, Vector3 size)
    {
        spawnCenter = center;
        spawnAreaSize = size;
    }
}