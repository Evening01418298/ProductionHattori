using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    public float walkSpeed = 5f;
    public float dashSpeed = 9.5f;
    public float maxSpeed = 9.5f;

    [Header("Acceleration")]
    public float acceleration = 12f;
    public float deceleration = 16f;

    [Header("Rotation")]
    public int rotationSpeed = 12;

    [Header("Gravity")]
    public float gravity = -9.81f;

    [Header("Speed Info")]
    public float currentSpeed;

    CharacterController controller;
    Vector3 velocity;
    Vector3 currentMove;
    float currentRotationVelocity;

    Transform cam;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cam = Camera.main.transform;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {

        bool grounded = controller.isGrounded;

        // ===== Input System =====
        float h = (Keyboard.current.dKey.isPressed ? 1 : 0)
                - (Keyboard.current.aKey.isPressed ? 1 : 0);

        float v = (Keyboard.current.wKey.isPressed ? 1 : 0)
                - (Keyboard.current.sKey.isPressed ? 1 : 0);

        bool dash = Keyboard.current.leftShiftKey.isPressed;

        // ===== カメラ基準移動 =====
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 inputDir = (camForward.normalized * v + camRight.normalized * h);
        if (inputDir.magnitude > 1f)
            inputDir.Normalize();

        // ===== 速度決定 =====
        float targetSpeed = walkSpeed;
        if (dash && v > 0 && grounded)
            targetSpeed = dashSpeed;

        Vector3 desiredMove = inputDir * targetSpeed;

        float accel = desiredMove.magnitude > currentMove.magnitude
            ? acceleration
            : deceleration;

        currentMove = Vector3.MoveTowards(
            currentMove,
            desiredMove,
            accel * Time.deltaTime
        );

        // ===== 回転（カメラ向き基準） =====
        if (v > 0.1f)
        {

            Vector3 lookDir = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
            if (lookDir.sqrMagnitude < 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * rotationSpeed
            );
        }


        // ===== 重力 =====
        if (grounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        // ===== 移動（1回だけ）=====
        Vector3 finalMove = currentMove + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);

        // ===== 速度取得 =====
        Vector3 horizontalVelocity = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        currentSpeed = horizontalVelocity.magnitude;
    }
}
