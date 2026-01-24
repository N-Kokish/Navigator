using UnityEngine;

public class DebugController : MonoBehaviour
{
    [Header("Toggle Settings")]
    [SerializeField] private GameObject m_DebugPanel;
    // ¬кл/викл дебаг
    public void TogglePanel()
    {
        if (m_DebugPanel != null)
        {
            bool isActive = m_DebugPanel.activeSelf;
            m_DebugPanel.SetActive(!isActive);
        }
    }
}