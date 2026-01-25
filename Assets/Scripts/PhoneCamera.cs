using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PhoneCamera : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RawImage m_BackImage;
    [SerializeField] private AspectRatioFitter m_Fitter;
    private WebCamTexture m_Cam;
    private bool m_IsInitialized = false;
    public WebCamTexture CurrentCam
    {
        get { return m_Cam; }
    }
    public RawImage BackgroundImage
    {
        get { return m_BackImage; }
    }
    public bool IsInitialized
    {
        get { return m_IsInitialized; }
    }

    IEnumerator Start()
    {
        // Запит дозволу
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.LogError("Error: User denied camera access.");
            yield break;
        }
        // пошук камер
        int maxWait = 20;
        while (WebCamTexture.devices.Length == 0 && maxWait > 0)
        {
            yield return new WaitForSeconds(0.2f);
            maxWait--;
        }
        if (WebCamTexture.devices.Length == 0)
        {
            Debug.LogError("Error: No camera device found.");
            yield break;
        }
        // вибір камери(!фронтальна)
        string backCamName = "";
        foreach (var dev in WebCamTexture.devices)
        {
            if (!dev.isFrontFacing)
            {
                backCamName = dev.name;
                break;
            }
        }
        // налаштування
        if (string.IsNullOrEmpty(backCamName))
        {
            m_Cam = new WebCamTexture(3264, 2448, 30);
        }
        else
        {
            m_Cam = new WebCamTexture(backCamName, 3264, 2448, 30);
        }
        m_BackImage.texture = m_Cam;
        m_Cam.Play();
        // очікування фактичного старту
        while (m_Cam.width < 100)
        {
            yield return null;
        }
        SetupCameraOrientation();
        m_IsInitialized = true;
    }

    void Update()
    {
        if (!m_IsInitialized || m_Cam == null) return;
        // Оновлення повороту (якщо користувач крутить телефон)
        float angle = -m_Cam.videoRotationAngle;
        m_BackImage.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    void SetupCameraOrientation()
    {
        float videoRatio = (float)m_Cam.width / (float)m_Cam.height;
        m_Fitter.aspectRatio = videoRatio;
        float scaleY;
        if (m_Cam.videoVerticallyMirrored)// перевернута камера?
        {
            scaleY = -1f; 
        }
        else
        {
            scaleY = 1f;  
        }
        m_BackImage.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        Debug.Log("Camera initialized: " + m_Cam.width + "x" + m_Cam.height);
    }
}