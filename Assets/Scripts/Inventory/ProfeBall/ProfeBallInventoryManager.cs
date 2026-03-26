using UnityEngine;

public class ProfeBallInventoryManager : MonoBehaviour
{
    public static ProfeBallInventoryManager Instance;

    [SerializeField] private ProfeBallInventory inventory = new();
    public ProfeBallInventory Inventory => inventory;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
