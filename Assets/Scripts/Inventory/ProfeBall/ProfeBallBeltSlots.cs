using System.Collections.Generic;
using UnityEngine;

public class ProfeBallBeltSlots : MonoBehaviour
{
    [Header("VR References")]
    public Transform xrOrigin;
    public Transform head;

    [Header("Prefab")]
    public GameObject profeBallPrefab;

    [Header("Belt Settings")]
    public int maxSlots = 8;
    public float radius = 0.18f;
    public float heightOffset = -0.25f;

    [Header("Arc")]
    public float startAngle = -120f;
    public float endAngle = 120f;

    private List<GameObject> slots = new List<GameObject>();

    void Start()
    {
        if (head == null)
            head = Camera.main.transform;

        CreateSlots();
    }

    void CreateSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(profeBallPrefab, transform);
            obj.SetActive(false);
            slots.Add(obj);
        }
    }

    void LateUpdate()
    {
        UpdateSlots();
    }

    void UpdateSlots()
    {
        var balls = ItemInventory.Instance.GetProfeBalls();

        int index = 0;

        foreach (var (item, count) in balls)
        {
            for (int i = 0; i < count; i++)
            {
                if (index >= maxSlots) return;

                GameObject obj = slots[index];
                obj.SetActive(true);

                // 🔥 desactivar física en cinturón
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                float t = (float)index / Mathf.Max(1, maxSlots - 1);
                float angle = Mathf.Lerp(startAngle, endAngle, t);
                float rad = angle * Mathf.Deg2Rad;

                Vector3 center = xrOrigin.position + Vector3.up * heightOffset;

                Vector3 offset = new Vector3(
                    Mathf.Sin(rad),
                    0,
                    Mathf.Cos(rad)
                ) * radius;

                obj.transform.position = center + offset;

                obj.transform.rotation =
                    Quaternion.LookRotation(obj.transform.position - head.position);

                index++;
            }
        }

        for (int i = index; i < maxSlots; i++)
            slots[i].SetActive(false);
    }
}