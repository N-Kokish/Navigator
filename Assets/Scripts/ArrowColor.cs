using UnityEngine;

public class ArrowColor : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ArrowLogic m_Logic;
    [SerializeField] private Transform m_VisualsParent;
    [Header("Colors")]
    [SerializeField] private Color m_StartColor = Color.red;
    [SerializeField] private Color m_FinishColor = Color.green;
    private Renderer[] m_Renderers;

    void Start()
    {
        if (m_VisualsParent != null)
        {
            m_Renderers = m_VisualsParent.GetComponentsInChildren<Renderer>();
        }
    }

    void Update()
    {
        if (m_Logic == null || m_Renderers == null || m_Renderers.Length == 0) return;
        double currentDist = m_Logic.DistanceToTarget;
        double startDist = m_Logic.StartDistance;
        float t = (float)(currentDist / startDist);
        t = Mathf.Clamp01(t);
        Color targetColor = Color.Lerp(m_FinishColor, m_StartColor, t);
        foreach (Renderer part in m_Renderers)
        {
            if (part != null)
            {
                part.material.color = targetColor;
            }
        }
    }
}