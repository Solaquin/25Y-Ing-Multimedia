using UnityEngine;

public class ProfeBallState : MonoBehaviour
{
    public enum State
    {
        InBelt,
        InHand,
        InWorld
    }

    public State currentState;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetInBelt()
    {
        currentState = State.InBelt;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void SetInHand()
    {
        currentState = State.InHand;

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void SetInWorld()
    {
        currentState = State.InWorld;

        rb.isKinematic = false;
        rb.useGravity = true;
    }
    public void Throw(Vector3 velocity)
    {
        SetInWorld();

        rb.linearVelocity = velocity;
    }
}