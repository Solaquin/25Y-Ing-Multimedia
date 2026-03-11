using UnityEngine;

public class FloatingMenu3D : MonoBehaviour
{
    public Transform playerCamera;

    [Header("Distance From Player")]
    public float distance = 1.5f;

    [Header("Vertical Offset")]
    public float heightOffset = -0.2f;

    [Header("Horizontal Offset")]
    public float horizontalOffset = 0.3f;

    void Update()
    {
        if (playerCamera == null) return;

        Vector3 forward = playerCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerCamera.right;
        right.y = 0;
        right.Normalize();

        Vector3 targetPosition =
            playerCamera.position
            + forward * distance
            + right * horizontalOffset
            + Vector3.up * heightOffset;

        transform.position = targetPosition;

        transform.LookAt(playerCamera);
        transform.Rotate(0, 180, 0);
    }
}