using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Por si acaso

public class ProfeBallBeltSlots : MonoBehaviour
{
    [Header("VR References")]
    public Transform xrOrigin;
    public Transform head;

    [Header("Spawn (REAL PROFEBALL)")]
    public GameObject realPrefab;
    public Transform hand;

    [Header("Slot Visual Prefab")]
    public GameObject profeBallPrefab;
    public int maxSlots = 8;
    public float radius = 0.18f;
    public float heightOffset = -0.25f;
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
            BeltSlotItem slot = obj.GetComponent<BeltSlotItem>();
            slot.Init(this);
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
        List<ProfeBallSO> flatList = new List<ProfeBallSO>();
        foreach (var (item, count) in balls)
        {
            for (int i = 0; i < count; i++)
                flatList.Add(item);
        }

        int countToShow = Mathf.Min(flatList.Count, maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = slots[i];
            if (i >= countToShow)
            {
                obj.SetActive(false);
                continue;
            }
            obj.SetActive(true);
            float t = (maxSlots == 1) ? 0.5f : (float)i / (maxSlots - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            float rad = angle * Mathf.Deg2Rad;
            Vector3 center = xrOrigin.position + Vector3.up * heightOffset;
            Vector3 offset = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)) * radius;
            obj.transform.position = center + offset;
            obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - head.position);

            var slotItem = obj.GetComponent<BeltSlotItem>();
            slotItem.SetBall(flatList[i]);
        }
    }

    // 🔥 MODIFICADO: ahora devuelve la bola real para poder transferir el grab
    public GameObject SpawnRealBall(Vector3 position, Quaternion rotation, Transform attachTransform)
    {
        GameObject real = Instantiate(realPrefab, position, rotation);

        Rigidbody rb = real.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        var grab = real.GetComponent<XRGrabInteractable>();
        if (grab != null && attachTransform != null)
        {
            grab.attachTransform = attachTransform;
            grab.throwOnDetach = true;
        }

        return real; // ← Nuevo retorno
    }
}