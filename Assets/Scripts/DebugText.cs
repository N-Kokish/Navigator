using UnityEngine;
using TMPro;

public class DebugText : MonoBehaviour
{
    [Header("TMP")]
    [SerializeField] private TMP_Text m_StatusText;
    [SerializeField] private TMP_Text m_LatText;
    [SerializeField] private TMP_Text m_LonText;
    [SerializeField] private TMP_Text m_AccText;

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
    }
}