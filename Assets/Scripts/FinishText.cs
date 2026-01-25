using UnityEngine;
using TMPro;

public class FinishText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArrowLogic m_ArrowLogic;
    [Header("Settings")]
    [SerializeField] private float m_ShowDistance = 100f;//відстань до якої появиться текст
    [SerializeField] private float m_HideDistance = 200f;//відстань після якої зникне текст
    private TextMeshPro m_Text;
    private bool m_IsVisible = false;
    void Start()
    {
        m_Text = GetComponent<TextMeshPro>();
        ToggleVisibility(false);
    }

    void Update()
    {
        if (m_ArrowLogic == null) return;
        float distance = (float)m_ArrowLogic.DistanceToTarget;
        if (!m_IsVisible)
        {
            if (distance < m_ShowDistance && GPS.IsSignalGood)
            {
                ToggleVisibility(true);
            }
        }
        else
        {
            if (distance > m_HideDistance)
            {
                ToggleVisibility(false);
            }
        }
        if (!m_IsVisible) return;
        m_Text.text = distance.ToString("F0") + " m";
        float factor = Mathf.Clamp01(distance / m_ShowDistance);
        m_Text.color = Color.Lerp(Color.green, Color.red, factor);
    }

    private void ToggleVisibility(bool state)
    {
        m_IsVisible = state;
        m_Text.enabled = state;
    }
}