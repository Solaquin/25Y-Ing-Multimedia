using UnityEngine;

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

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError("ProfemonData no asignado en " + gameObject.name);
            return;
        }

        // 🔥 Seguridad por si alguien pone mal los valores
        if (maxLevel < minLevel)
            maxLevel = minLevel;

        // 🔥 Nivel aleatorio dentro del rango
        level = Random.Range(minLevel, maxLevel + 1);

        instance = new ProfemonInstance(data, level);

        // 🔥 Buscar jugador por Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        Debug.Log(data.professorName + " salvaje generado nivel " + level);
    }

    private void Update()
    {
        if (player == null)
            return;

        if (isCaptured)
            return;

        float sqrDistance = (transform.position - player.position).sqrMagnitude;

        if (sqrDistance > despawnDistance * despawnDistance)
        {
            Destroy(gameObject);
        }
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
}