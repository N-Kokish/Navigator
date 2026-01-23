using UnityEngine;
using System;

public class ArrowLogic : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private double m_TargetLat = 48.51625;
    [SerializeField] private double m_TargetLon = 32.25832;
    [Header("Visuals")]
    [SerializeField] private Transform m_ArrowObject;
    [SerializeField] private float m_RotationSpeed = 2f;
    private double m_DistanceToTarget;
    private float m_BearingToTarget;
    public double DistanceToTarget
    {
        get { return m_DistanceToTarget; }
    }
    public float BearingToTarget
    {
        get { return m_BearingToTarget; }
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
}