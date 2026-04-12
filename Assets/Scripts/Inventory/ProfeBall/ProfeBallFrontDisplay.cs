using System.Collections.Generic;
using UnityEngine;

public class ProfeBallFrontDisplay : MonoBehaviour
{
    [Header("Referencia VR (cámara del jugador)")]
    public Transform head; // cámara VR

    [Header("Spawn settings")]
    public GameObject profeBallPrefab;
    public float distanceInFront = 1.5f;
    public float spacing = 0.25f;

    private List<GameObject> spawned = new List<GameObject>();

    void Start()
    {
        if (head == null)
            head = Camera.main.transform;

        ItemInventory.Instance.OnInventoryChanged += Refresh;
        Refresh("");
    }

    void OnDestroy()
    {
        if (ItemInventory.Instance != null)
            ItemInventory.Instance.OnInventoryChanged -= Refresh;
    }

    void Refresh(string _)
    {
        Clear();

        var balls = ItemInventory.Instance.GetProfeBalls();
        if (balls == null) return;

        int index = 0;

        foreach (var (item, count) in balls)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 basePos =
                    head.position + head.forward * distanceInFront;

                Vector3 offset =
                    head.right * (index * spacing - (count * spacing) * 0.5f);

                Vector3 spawnPos = basePos + offset;

                GameObject obj = Instantiate(profeBallPrefab, spawnPos, Quaternion.identity);

                // que mire al jugador
                obj.transform.LookAt(head);

                spawned.Add(obj);
                index++;
            }
        }
    }

    void Clear()
    {
        foreach (var go in spawned)
        {
            if (go != null) Destroy(go);
        }
        spawned.Clear();
    }
}