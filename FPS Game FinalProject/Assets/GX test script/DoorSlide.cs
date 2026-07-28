using UnityEngine;
using System.Collections;

public class DoorSlide : MonoBehaviour
{
    [Header("Door Settings")]
    public float slideDistance = 3f;
    public float slideSpeed = 2f;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    private bool playerNear = false;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        closedPosition = transform.position;

        // Slide along the BLUE arrow (Local Z axis)
        openPosition = closedPosition + transform.forward * slideDistance;
    }

    void Update()
    {
        if (playerNear && !isOpen && !isMoving && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        isMoving = true;

        while (Vector3.Distance(transform.position, openPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                openPosition,
                slideSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = openPosition;
        isOpen = true;
        isMoving = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
}