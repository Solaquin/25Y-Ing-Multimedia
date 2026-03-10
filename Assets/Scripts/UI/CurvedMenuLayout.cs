using UnityEngine;

public class CurvedMenuLayout : MonoBehaviour
{
    public float radius = 0.6f;
    public float angle = 60f;

    void Start()
    {
        int count = transform.childCount;

        for (int i = 0; i < count; i++)
        {
            Transform button = transform.GetChild(i);

            float step = angle / (count - 1);
            float currentAngle = -angle / 2 + step * i;

            float rad = currentAngle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(
                Mathf.Sin(rad) * radius,
                0,
                Mathf.Cos(rad) * radius
            );

            button.localPosition = pos;
            button.LookAt(Camera.main.transform);
        }
    }
}