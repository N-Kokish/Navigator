using UnityEngine;
using System.Collections;
using UnityEngine.Android;

public class GPS : MonoBehaviour
{
    private static float m_Latitude;
    private static float m_Longitude;
    private static float m_Accuracy;
    private static bool m_IsSignalGood = false;
    private static string m_StatusMessage;
    // Налаштування точності
    const float TARGET_ACCURACY = 15f;
    const float UPDATE_DISTANCE = 1f;
    public static float Latitude
    {
        get { return m_Latitude; }
    }
    public static float Longitude
    {
        get { return m_Longitude; }
    }
    public static float Accuracy
    {
        get { return m_Accuracy; }
    }
    public static bool IsSignalGood
    {
        get { return m_IsSignalGood; }
    }
    public static string StatusMessage
    {
        get { return m_StatusMessage; }
    }

    void Start()
    {
        Application.targetFrameRate = 30;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        StartCoroutine(StartLocationService());
    }

    IEnumerator StartLocationService()
    {
        // дозвіл місцезнаходження
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(1.5f);
        }
        Input.location.Start(10f, UPDATE_DISTANCE);
        m_StatusMessage = "Запуск GPS...";
        // Очікування іціалізації і перевірка стану
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        if (maxWait < 1)
        {
            m_StatusMessage = "Тайм-аут GPS";
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            m_StatusMessage = "Помилка: Немає доступу";
            yield break;
        }
        // моніторинг геолокації
        m_StatusMessage = "Пошук супутників...";
        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                m_Latitude = Input.location.lastData.latitude;
                m_Longitude = Input.location.lastData.longitude;
                m_Accuracy = Input.location.lastData.horizontalAccuracy;
                // Логіка якості сигналу
                if (m_Accuracy <= TARGET_ACCURACY)
                {
                    m_StatusMessage = "GPS Активний";
                    m_IsSignalGood = true;
                }
                else
                {
                    m_StatusMessage = $"Слабкий сигнал ({m_Accuracy:F0}м)";
                    m_IsSignalGood = false;
                }
            }
            else
            {
                m_StatusMessage = "Втрачено зв'язок";
                m_IsSignalGood = false;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void OnDestroy()
    {
        if (Input.location.status != LocationServiceStatus.Stopped)
        {
            Input.location.Stop();
        }
    }
}