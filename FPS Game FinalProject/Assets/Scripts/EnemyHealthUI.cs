using TMPro;
using UnityEngine;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Animation")]
    [SerializeField] private float changeSpeed = 5f;

    private float targetScale = 1f;

    private void Start()
    {
        if (healthFill != null)
        {
            healthFill.localScale = new Vector3(
                1f,
                healthFill.localScale.y,
                healthFill.localScale.z
            );
        }
    }

    private void Update()
    {
        if (healthFill == null)
        {
            return;
        }

        Vector3 currentScale = healthFill.localScale;

        float newScaleX = Mathf.MoveTowards(
            currentScale.x,
            targetScale,
            changeSpeed * Time.deltaTime
        );

        healthFill.localScale = new Vector3(
            newScaleX,
            currentScale.y,
            currentScale.z
        );
    }

    public void SetHealth(float current, float maximum)
    {
        maximum = Mathf.Max(1f, maximum);

        current = Mathf.Clamp(
            current,
            0f,
            maximum
        );

        targetScale = current / maximum;

        if (healthText != null)
        {
            healthText.text =
                Mathf.CeilToInt(current)
                + " / "
                + Mathf.CeilToInt(maximum);
        }
    }
}