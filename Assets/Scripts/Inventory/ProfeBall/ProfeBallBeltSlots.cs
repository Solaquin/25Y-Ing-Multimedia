using System.Collections.Generic;
using UnityEngine;

public class ProfeBallBeltSlots : MonoBehaviour
{
    [Header("VR References")]
    public Transform hips;   // centro del cuerpo (XR Origin o Camera Offset)
    public Transform head;

    [Header("Belt Settings")]
    public GameObject profeBallPrefab;
    public int maxSlots = 8;
    public float radius = 0.35f;
    public float heightOffset = -0.4f;

    [Header("Slot arc")]
    public float startAngle = -120f;
    public float endAngle = 120f;

    private List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        if (head == null)
            head = Camera.main.transform;

        CreateSlots();

        ItemInventory.Instance.OnInventoryChanged += Refresh;
        Refresh("");
    }

    void OnDestroy()
    {
        if (ItemInventory.Instance != null)
            ItemInventory.Instance.OnInventoryChanged -= Refresh;
    }

    void CreateSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(profeBallPrefab, transform);
            obj.SetActive(false); // vacío al inicio
            slots.Add(obj);
        }
    }

    void Refresh(string _)
    {
        var balls = ItemInventory.Instance.GetProfeBalls();

        int total = 0;
        foreach (var b in balls)
            total += b.count;

        // llenar slots
        int index = 0;

        foreach (var (item, count) in balls)
        {
            for (int i = 0; i < count; i++)
            {
                if (index >= maxSlots)
                    return;

                GameObject slotObj = slots[index];
                slotObj.SetActive(true);

                float t = (maxSlots == 1) ? 0.5f : (float)index / (maxSlots - 1);
                float angle = Mathf.Lerp(startAngle, endAngle, t);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 center = hips.position + Vector3.up * heightOffset;

                Vector3 offset = new Vector3(
                    Mathf.Sin(rad),
                    0,
                    Mathf.Cos(rad)
                ) * radius;

                Vector3 pos = center + offset;

                slotObj.transform.position = pos;
                slotObj.transform.rotation = Quaternion.LookRotation(slotObj.transform.position - head.position);

                index++;
            }
        }

        // desactivar slots sobrantes
        for (int i = index; i < maxSlots; i++)
        {
            slots[i].SetActive(false);
        }
    }
}