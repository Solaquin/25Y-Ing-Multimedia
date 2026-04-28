using UnityEngine;

public class BeltFollowTarget : MonoBehaviour
{
    [Header("Referencias")]
    public Transform bodyTarget;  // CharacterController o XROrigin
    public Transform headTarget;  // Main Camera

    [Header("Posición")]
    public Vector3 offset = new Vector3(0, -0.3f, 0);

    [Header("Rotación")]
    public float threshold = 45f;
    public float rotationSpeed = 5f;

    private float currentBodyYaw;

    void Start()
    {
        if (headTarget != null)
            currentBodyYaw = headTarget.eulerAngles.y;

        // Posición y rotación inicial inmediata
        SnapToBelt();
    }

    void LateUpdate()
    {
        if (bodyTarget == null || headTarget == null) return;

        // Posición sigue al cuerpo
        transform.position = bodyTarget.position + offset;

        // Rotación lazy basada en la cabeza
        float headYaw = headTarget.eulerAngles.y;
        float delta = Mathf.DeltaAngle(currentBodyYaw, headYaw);

        if (Mathf.Abs(delta) > threshold)
        {
            float targetYaw = headYaw - (Mathf.Sign(delta) * threshold);
            currentBodyYaw = Mathf.LerpAngle(currentBodyYaw, targetYaw, Time.deltaTime * rotationSpeed);
        }

        transform.rotation = Quaternion.Euler(0, currentBodyYaw, 0);
    }

    void SnapToBelt()
    {
        if (bodyTarget == null || headTarget == null) return;
        transform.position = bodyTarget.position + offset;
        currentBodyYaw = headTarget.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, currentBodyYaw, 0);
    }
}