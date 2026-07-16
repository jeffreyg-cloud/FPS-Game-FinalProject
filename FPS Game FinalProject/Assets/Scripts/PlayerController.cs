using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public float moveSpeed = 8f;
    public float gravityModifier = 4f;
    public float jumpingPower = 9f;
    public float runSpeed = 16f;

    public CharacterController charCon;
    private Vector3 moveInput;

    public Transform camTrans;
    public Animator anim;

    int jumpAgain;

    public GameObject bullet;
    public Transform firePoint;

    public float mouseSensitivity = 3f;

    void Start()
    {
        if (charCon == null)
            charCon = GetComponent<CharacterController>();

        Debug.Log("PlayerController Started");

        if (charCon == null)
            Debug.LogError("CharacterController is NULL!");

        if (camTrans == null)
            Debug.LogError("Camera Transform is NULL!");

        if (anim == null)
            Debug.LogError("Animator is NULL!");
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Debug.Log($"Horizontal = {h}  Vertical = {v}");

        float yStore = moveInput.y;

        Vector3 vertMove = transform.forward * v;
        Vector3 horiMove = transform.right * h;

        moveInput = vertMove + horiMove;
        moveInput.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
            moveInput *= runSpeed;
        else
            moveInput *= moveSpeed;

        moveInput.y = yStore;

        if (charCon.isGrounded)
        {
            moveInput.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveInput.y = jumpingPower;
                jumpAgain = 2;
            }
        }
        else
        {
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        if (jumpAgain > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpingPower;
            jumpAgain--;
        }

        charCon.Move(moveInput * Time.deltaTime);

        float horizontalSpeed = new Vector3(charCon.velocity.x, 0, charCon.velocity.z).magnitude;

        if (anim != null)
            anim.SetFloat("moveSpeed", horizontalSpeed);

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);

        if (camTrans != null)
        {
            camTrans.Rotate(Vector3.left * mouseY * mouseSensitivity);
        }

        if (bullet != null && firePoint != null && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit))
            {
                firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(camTrans.position + camTrans.forward * 30f);
            }

            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }
    }
}