using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArrowLogic arrowLogicScript;

    [Header("Settings")]
    [SerializeField] private float showDistance = 100f; // Коли показувати
    [SerializeField] private float hideDistance = 200f; // Коли ховати

    private TextMeshPro m_Text;
    private bool m_IsVisible = false;

    void Start()
    {
        m_Text = GetComponent<TextMeshPro>();
        ToggleVisibility(false);
    }

    void Update()
    {
        if (arrowLogicScript == null) return;

        // Беремо дистанцію
        float distance = (float)arrowLogicScript.DistanceToTarget;

        // 1. Логіка появи/зникнення
        if (!m_IsVisible)
        {
            if (distance < showDistance && GPS.IsSignalGood)
            {
                ToggleVisibility(true);
            }
        }
        else
        {
            if (distance > hideDistance)
            {
                ToggleVisibility(false);
            }
        }

        if (!m_IsVisible) return;

        // 2. Оновлення цифр
        m_Text.text = distance.ToString("F0") + " m";

        // 3. Зміна кольору (Зелений -> Червоний)
        float factor = Mathf.Clamp01(distance / showDistance);
        m_Text.color = Color.Lerp(Color.green, Color.red, factor);
    }

    private void ToggleVisibility(bool state)
    {
        m_IsVisible = state;
        m_Text.enabled = state;
    }
}