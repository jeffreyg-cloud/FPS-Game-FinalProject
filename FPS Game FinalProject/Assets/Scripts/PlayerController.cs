using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public float moveSpeed;
    public float gravityModifier;
    public float jumpingPower;
    public float runSpeed;

    public CharacterController charCon;
    private Vector3 moveInput;

    public Transform camTrans;
    public Animator anim;

    private int jumpAgain;

    public GameObject bullet;
    public Transform firePoint;
    public WandManager wandManager;

    public float mouseSensitivity;

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        // Read keyboard movement using the new Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                vertical += 1f;

            if (Keyboard.current.sKey.isPressed)
                vertical -= 1f;

            if (Keyboard.current.dKey.isPressed)
                horizontal += 1f;

            if (Keyboard.current.aKey.isPressed)
                horizontal -= 1f;
        }

        float yStore = moveInput.y;

        // Move based on the player's facing direction
        Vector3 vertMove = transform.forward * vertical;
        Vector3 horiMove = transform.right * horizontal;

        moveInput = vertMove + horiMove;
        moveInput.Normalize();

        // Running
        if (Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed)
        {
            moveInput *= runSpeed;
        }
        else
        {
            moveInput *= moveSpeed;
        }

        moveInput.y = yStore;

        // Gravity
        moveInput.y += Physics.gravity.y
                       * gravityModifier
                       * Time.deltaTime;

        if (charCon.isGrounded)
        {
            moveInput.y = -1f;

            if (Keyboard.current != null &&
                Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                moveInput.y = jumpingPower;
                jumpAgain = 2;
            }
        }

        // Double jump
        if (jumpAgain > 0 &&
            Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            moveInput.y = jumpingPower;
            jumpAgain--;
        }

        charCon.Move(moveInput * Time.deltaTime);

        float horizontalSpeed = new Vector3(
            charCon.velocity.x,
            0f,
            charCon.velocity.z
        ).magnitude;

        if (anim != null)
        {
            anim.SetFloat("moveSpeed", horizontalSpeed);
        }

        // Mouse camera movement
        Vector2 mouseInput = Vector2.zero;

        if (Mouse.current != null)
        {
            mouseInput = Mouse.current.delta.ReadValue()
                         * mouseSensitivity
                         * 0.01f;
        }

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + mouseInput.x,
            transform.rotation.eulerAngles.z
        );

        if (camTrans != null)
        {
            camTrans.rotation = Quaternion.Euler(
                camTrans.rotation.eulerAngles
                + new Vector3(-mouseInput.y, 0f, 0f)
            );
        }

        // Shooting
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Left Click!");

            if (wandManager == null)
            {
                Debug.LogError("WandManager is NOT assigned!");
                return;
            }

            WandStats wand = wandManager.ActiveWand;

            if (wand == null)
            {
                Debug.LogError("Active wand is NULL — check WandManager's Wands array!");
                return;
            }

            if (wand.effectPrefab == null)
            {
                Debug.LogError("Active wand's Effect Prefab is NOT assigned!");
                return;
            }

            if (wand.firePoint == null)
            {
                Debug.LogError("Active wand's Fire Point is NOT assigned!");
                return;
            }

            if (camTrans == null)
            {
                Debug.LogError("Camera Transform is NOT assigned!");
                return;
            }

            if (!wandManager.TrySpendMana(wand.manaCost))
            {
                Debug.Log("Not enough mana!");
                return;
            }

            RaycastHit hit;

            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit))
            {
                wand.firePoint.LookAt(hit.point);
            }
            else
            {
                wand.firePoint.LookAt(camTrans.position + camTrans.forward * 30f);
            }

            GameObject newBullet = Instantiate(
                wand.effectPrefab,
                wand.firePoint.position,
                wand.firePoint.rotation
            );

            if (wand.wandAnimator != null)
            {
                wand.wandAnimator.SetTrigger("Attack");
            }

            Debug.Log("Bullet spawned at: " + newBullet.transform.position);
        }

    }

}