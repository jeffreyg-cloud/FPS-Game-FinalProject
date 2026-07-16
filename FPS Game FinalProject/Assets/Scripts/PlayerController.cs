using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public float moveSpeed, gravityModifier, jumpingPower, runSpeed;
    public CharacterController charCon;
    private Vector3 moveInput;
    public Transform camTrans;
    public Animator anim;
    int jumpAgain;

    public GameObject bullet;
    public Transform firePoint;

    public float mouseSensitivity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        float yStore = moveInput.y;

        //move based on player facing direction
        Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");//forward is Z or blue axis
        Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");//right is X or red axis

        moveInput = vertMove + horiMove;
        moveInput.Normalize();//to normalize the speed of diagonal movement, such as A&W

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput = moveInput * runSpeed;
        }
        else
        {
            moveInput = moveInput * moveSpeed;
        }

        moveInput.y = yStore;

        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (charCon.isGrounded)//detect the ground
        {
            moveInput.y = -1f;
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                moveInput.y = jumpingPower;
                jumpAgain = 2;
            }

        }

        if (jumpAgain > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpingPower;
            jumpAgain--;
        }



        charCon.Move(moveInput * Time.deltaTime);

        Debug.Log(moveInput.magnitude);
        float horizontalSpeed = new Vector3(charCon.velocity.x, 0, charCon.velocity.z).magnitude;
        anim.SetFloat("moveSpeed", horizontalSpeed);


        //Player looking rotation (right and left)
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
        camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

        Debug.Log(horizontalSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit))
            {
                firePoint.LookAt(hit.point);
            }
            else
            {
                firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
                //looking at the centre of the screen
            }

            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }
    }
}
