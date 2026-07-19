using UnityEngine;

public class TutorialTestPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        transform.Translate(
            movement * moveSpeed * Time.deltaTime,
            Space.World
        );
    }
}