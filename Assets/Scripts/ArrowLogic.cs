using UnityEngine;
using System;
using TMPro;
using System.Globalization;
using System.Collections;

public class ArrowLogic : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private double m_TargetLat = 48.51625;
    [SerializeField] private double m_TargetLon = 32.25832;
    [Header("UI Input Fields")]
    [SerializeField] private TMP_InputField m_LatInput;
    [SerializeField] private TMP_InputField m_LonInput;
    [Header("Visuals")]
    [SerializeField] private Transform m_ArrowObject;
    [SerializeField] private float m_RotationSpeed = 2f;
    private Coroutine m_SearchCoroutine;
    private double m_DistanceToTarget;
    private float m_BearingToTarget;
    private double m_StartDistance;
    public double DistanceToTarget
    {
        get { return m_DistanceToTarget; }
    }
    public float BearingToTarget
    {
        get { return m_BearingToTarget; }
    }
    public double StartDistance
    {
        get { return m_StartDistance; }
    }

    void Start()
    {
        // Для ArrowColor
        if (GPS.IsSignalGood)
        {
            m_StartDistance = CalculateDistance(GPS.Latitude, GPS.Longitude, m_TargetLat, m_TargetLon);
            if (m_StartDistance < 1) m_StartDistance = 1;
        }
        else
        {
            m_StartDistance = 0.1f; // Щоб стрілка була червоною якщо сигнал поганий
            m_SearchCoroutine = StartCoroutine(WaitForGoodSignal());
        }
        // Для логіки
        if (m_LatInput != null)
        {
            m_LatInput.text = m_TargetLat.ToString(CultureInfo.InvariantCulture);
            m_LatInput.onValueChanged.AddListener(delegate { UpdateCoordinates(); });
        }
        if (m_LonInput != null)
        {
            m_LonInput.text = m_TargetLon.ToString(CultureInfo.InvariantCulture);
            m_LonInput.onValueChanged.AddListener(delegate { UpdateCoordinates(); });
        }
    }

    void Update()
    {
        double currentLat = GPS.Latitude;
        double currentLon = GPS.Longitude;
        float phoneAzimuth = Compass.Azimuth;
        if (currentLat == 0 && currentLon == 0) return;
        m_BearingToTarget = (float)CalculateBearing(currentLat, currentLon, m_TargetLat, m_TargetLon);
        m_DistanceToTarget = CalculateDistance(currentLat, currentLon, m_TargetLat, m_TargetLon);
        float finalAngle = m_BearingToTarget - phoneAzimuth;
        //Поворот стрілки
        if (m_ArrowObject != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, finalAngle, 0);
            m_ArrowObject.localRotation = Quaternion.Slerp(m_ArrowObject.localRotation, targetRotation, Time.deltaTime * m_RotationSpeed);
        }
    }
    // Очікування гарного сигналу для заданя початкової відстані
    IEnumerator WaitForGoodSignal()
    {
        while (!GPS.IsSignalGood)
        {
            yield return new WaitForSeconds(1f);
        }
        m_StartDistance = CalculateDistance(GPS.Latitude, GPS.Longitude, m_TargetLat, m_TargetLon);
        if (m_StartDistance < 1) m_StartDistance = 1;
    }
    // Визначення кута до цілі
    private double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        double dLon = ToRad(lon2 - lon1);// різниця радіанів по довготі
        // dPhi = Log(tan(lat2 / 2 + PI / 4) / tan(lat1 / 2 + PI / 4)) (різниця радіанів по широті (проєкція Меркатора))
        double dPhi = Math.Log(Math.Tan(ToRad(lat2) / 2 + Math.PI / 4) / Math.Tan(ToRad(lat1) / 2 + Math.PI / 4));
        if (Math.Abs(dLon) > Math.PI)
        {
            if (dLon > 0)
            {
                dLon = -(2 * Math.PI - dLon);
            }
            else
            {
                dLon = (2 * Math.PI + dLon);
            }
        }
        return (ToDeg(Math.Atan2(dLon, dPhi)) + 360) % 360;
    }
    // Визначення відстані до цілі
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371000; // Радіус Землі
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        // a = sin^2(dlat/2) + cos(lat1) * cos(lat2) * sin^2(dlon/2) (формула Haversine)
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    }
    private double ToRad(double deg)
    {
        return deg * (Math.PI / 180);
    }
    private double ToDeg(double rad)
    {
        return rad * (180 / Math.PI);
    }
    public void UpdateCoordinates()
    {
        bool changed = false;
        if (m_LatInput != null && !string.IsNullOrEmpty(m_LatInput.text))
        {
            string latText = m_LatInput.text.Replace(",", ".");
            if (double.TryParse(latText, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultLat))
            {
                m_TargetLat = resultLat;
                changed = true;
            }
        }
        if (m_LonInput != null && !string.IsNullOrEmpty(m_LonInput.text))
        {
            string lonText = m_LonInput.text.Replace(",", ".");
            if (double.TryParse(lonText, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultLon))
            {
                m_TargetLon = resultLon;
                changed = true;
            }
        }
        // Для ArrowColor
        if (changed)
        {
            if (m_SearchCoroutine != null) StopCoroutine(m_SearchCoroutine);
            m_StartDistance = CalculateDistance(GPS.Latitude, GPS.Longitude, m_TargetLat, m_TargetLon);
            if (m_StartDistance < 1) m_StartDistance = 1;
        }
    }
}