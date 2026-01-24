using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{

    [SerializeField] private ArrowLogic m_ArrowLogic;
    [Header("TMP")]
    [SerializeField] private TMP_Text m_StatusText;
    [SerializeField] private TMP_Text m_LatText;
    [SerializeField] private TMP_Text m_LonText;
    [SerializeField] private TMP_Text m_AccText;
    [SerializeField] private TMP_Text m_CompassText;
    [SerializeField] private TMP_Text m_DistanceText;
    [SerializeField] private TMP_Text m_TargetAngleText;
    void Update()
    {
        m_StatusText.text = GPS.StatusMessage;
        m_LatText.text = $"Lat: {GPS.Latitude:F6}";
        m_LonText.text = $"Lon: {GPS.Longitude:F6}";
        m_AccText.text = $"Acc: {GPS.Accuracy:F1}m";
        //статус
        if (GPS.IsSignalGood)
        {
            m_StatusText.color = Color.green;
            m_AccText.color = Color.green;
        }
        else
        {
            m_StatusText.color = Color.yellow;
            m_AccText.color = Color.yellow;
        }
        if (GPS.StatusMessage.Contains("Помилка") || GPS.StatusMessage.Contains("Втрачено"))
        {
            m_StatusText.color = Color.red;
        }
        if (m_CompassText != null)
        {
            float az = Compass.Azimuth;
            string dir = GetDirectionName(az);
            m_CompassText.text = $"{dir} : {az:F0}";
        }
        if (m_ArrowLogic != null)
        {
            double dist = m_ArrowLogic.DistanceToTarget;
            string distStr;
            if (dist > 1000)
            {
                distStr = $"{(dist / 1000):F1} km";
            }
            else
            {
                distStr = $"{dist:F0} m";
            }
            if (m_DistanceText != null)
                m_DistanceText.text = $"Dist: {distStr}";
            if (m_TargetAngleText != null)
                m_TargetAngleText.text = $"Target: {m_ArrowLogic.BearingToTarget:F0}";
        }
    }
    private string GetDirectionName(float angle)
    {
        if (angle > 337.5f || angle <= 22.5f) return "N";
        if (angle > 22.5f && angle <= 67.5f) return "NE";
        if (angle > 67.5f && angle <= 112.5f) return "E";
        if (angle > 112.5f && angle <= 157.5f) return "SE";
        if (angle > 157.5f && angle <= 202.5f) return "S";
        if (angle > 202.5f && angle <= 247.5f) return "SW";
        if (angle > 247.5f && angle <= 292.5f) return "W";
        if (angle > 292.5f && angle <= 337.5f) return "NW";
        return "-";
    }
}