using System.Collections.Generic;
using UnityEngine;

public class ProfeBallBeltDisplay : MonoBehaviour
{
    [Header("Referencias VR")]
    public Transform hips; // centro del cuerpo (XR Origin / Camera rig)
    public Transform head;

    [Header("Belt settings")]
    public GameObject profeBallPrefab;
    public float radius = 0.35f;          // distancia del cinturón
    public float heightOffset = -0.4f;    // altura respecto a la cabeza/rig
    public float maxArcDegrees = 120f;    // arco total del cinturón

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

        int totalCount = 0;
        foreach (var b in balls)
            totalCount += b.count;

        if (totalCount == 0) return;

        int index = 0;

        foreach (var (item, count) in balls)
        {
            for (int i = 0; i < count; i++)
            {
                float t = (totalCount == 1) ? 0.5f : (float)index / (totalCount - 1);

                float angle = Mathf.Lerp(-maxArcDegrees / 2, maxArcDegrees / 2, t);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 center = hips.position + Vector3.up * heightOffset;

                Vector3 offset = new Vector3(
                    Mathf.Sin(rad),
                    0,
                    Mathf.Cos(rad)
                ) * radius;

                Vector3 spawnPos = center + offset;

                GameObject obj = Instantiate(profeBallPrefab, spawnPos, Quaternion.identity);

                obj.transform.LookAt(center + Vector3.up * 0.2f);

                spawned.Add(obj);
                index++;
            }
        }
    }

    void Clear()
    {
        foreach (var go in spawned)
            if (go != null) Destroy(go);

        spawned.Clear();
    }
}