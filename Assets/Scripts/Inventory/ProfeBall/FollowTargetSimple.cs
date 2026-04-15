using UnityEngine;

public class FollowTargetSimple : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    public float smooth = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + target.TransformDirection(offset);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * smooth
        );
    }
}