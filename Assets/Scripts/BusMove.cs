using UnityEngine;
using UnityEngine.InputSystem;

public class BusMove : MonoBehaviour
{
    public Vector2 InputVec;
    public float ForwardSpeed;
    public float BackwardSpeed;
    public float MexSpeed;
    public float TurnSpeed;

    Rigidbody2D Rigidbody;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float ForwardInput = InputVec.y;
        float TurnInput = InputVec.x * -1;

        if (ForwardInput > 0 && Rigidbody.velocity.sqrMagnitude < MexSpeed) // ������ �ӵ� �ٸ���
            Rigidbody.AddForce(transform.up * ForwardInput * ForwardSpeed);
        else
            Rigidbody.AddForce(transform.up * ForwardInput * BackwardSpeed);

        if (ForwardInput != 0) // ��ü�� �����϶��� ������ȯ
            Rigidbody.MoveRotation(Rigidbody.rotation + TurnInput * TurnSpeed * Time.fixedDeltaTime);
     }

    void OnMove(InputValue input)
    {
        InputVec = input.Get<Vector2>();
    }
}
