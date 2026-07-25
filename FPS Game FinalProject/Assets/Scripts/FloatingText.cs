using TMPro;
using UnityEngine;

// Attach to a UI prefab (Image/empty RectTransform with a TMP_Text child).
// Must be instantiated as a child of a Screen Space - Overlay Canvas.
// Floats upward in screen pixels and fades out, then destroys itself.
[RequireComponent(typeof(RectTransform))]
public class FloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float floatSpeed = 60f; // pixels per second
    [SerializeField] private float lifetime = 1f;

    private RectTransform rect;
    private float timer;
    private Color baseColor;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void Setup(string message, Color color)
    {
        if (text == null) text = GetComponentInChildren<TMP_Text>();
        if (text == null) return;

        text.text = message;
        text.color = color;
        baseColor = color;
    }

    private void Update()
    {
        rect.anchoredPosition += Vector2.up * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        if (text != null)
        {
            Color c = baseColor;
            c.a = alpha;
            text.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}