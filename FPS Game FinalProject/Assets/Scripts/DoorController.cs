using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    [Header("Door References")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Door Settings")]
    public float openAngle = 45f;
    public float openSpeed = 2f;

    private bool playerNearby = false;
    private bool doorOpened = false;

    private PlayerKey playerKey;

    private Quaternion leftClosedRotation;
    private Quaternion rightClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightOpenRotation;

    private void Start()
    {
        leftClosedRotation = leftDoor.localRotation;
        rightClosedRotation = rightDoor.localRotation;

        leftOpenRotation = leftClosedRotation * Quaternion.Euler(0, -openAngle, 0);
        rightOpenRotation = rightClosedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    private void Update()
    {
        if (!playerNearby || doorOpened)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerKey != null && playerKey.hasKey)
            {
                doorOpened = true;
                StartCoroutine(OpenDoor());
            }
            else
            {
                Debug.Log("You need to collect the key first!");
            }
        }
    }

    IEnumerator OpenDoor()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;

            leftDoor.localRotation = Quaternion.Slerp(leftClosedRotation, leftOpenRotation, t);
            rightDoor.localRotation = Quaternion.Slerp(rightClosedRotation, rightOpenRotation, t);

            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            playerNearby = true;
            playerKey = player.GetComponent<PlayerKey>();

            Debug.Log("Player entered gate trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerNearby = false;
            playerKey = null;
        }
    }
}