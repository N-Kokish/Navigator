using UnityEngine;
using UnityEngine.InputSystem;

public class Compass : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float m_Smoothing = 5f;
    [SerializeField] private float m_HeadingOffset = 180f;
    private float m_CurrentHeading;
    private static float m_Azimuth;
    public static float Azimuth
    {
        get { return m_Azimuth; }
    }
    void Start()
    {
        // Перевірка датчиків
        if (Accelerometer.current != null)
            InputSystem.EnableDevice(Accelerometer.current);
        if (MagneticFieldSensor.current != null)
            InputSystem.EnableDevice(MagneticFieldSensor.current);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Accelerometer.current == null || MagneticFieldSensor.current == null) return;
        // Де низ? Де північ?
        Vector3 gravity = Accelerometer.current.acceleration.ReadValue();
        Vector3 magnet = MagneticFieldSensor.current.magneticField.ReadValue();
        // Де схід?
        Vector3 rawEast = Vector3.Cross(magnet.normalized, gravity.normalized);
        if (rawEast.magnitude < 0.1f) return;
        Vector3 g = gravity.normalized;
        Vector3 east = rawEast.normalized;
        Vector3 north = Vector3.Cross(g, east).normalized;
        float heading = -(Mathf.Atan2(east.z, north.z) * Mathf.Rad2Deg);// перетворення вектора в градуси
        // Калібрування
        heading += m_HeadingOffset;
        heading = Mathf.Repeat(heading, 360f);
        m_CurrentHeading = Mathf.LerpAngle(m_CurrentHeading, heading, Time.deltaTime * m_Smoothing);
        m_Azimuth = Mathf.Repeat(m_CurrentHeading, 360f);
    }
}