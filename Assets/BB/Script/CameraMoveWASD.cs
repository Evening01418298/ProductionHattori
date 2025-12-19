using UnityEngine;

public class CameraMoveWASD : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A,D
        float v = Input.GetAxis("Vertical");   // W,S

        // ƒJƒƒ‰‚ÌŒü‚¢‚Ä‚¢‚é•ûŒüŠî€‚ÅˆÚ“®
        Vector3 move =
            transform.right * h +
            transform.forward * v;

        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
