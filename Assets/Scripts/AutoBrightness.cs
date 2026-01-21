using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoBrightness : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RawImage m_CamDisplay;
    [Header("Settings")]
    [SerializeField] private float m_TargetGray = 0.5f;
    [SerializeField] private float m_CheckInterval = 0.5f;
    [Header("Smoothness")]
    [SerializeField] private float m_SmoothSpeed = 2.0f;
    [SerializeField] private float m_MinLight = 1.0f;
    [SerializeField] private float m_MaxLight = 6.0f;
    private WebCamTexture m_Cam;
    private Material m_Mat;
    private float m_CurrentLightVal = 1.0f;
    private float m_TargetLightVal = 1.0f;
    private int m_LightShaderID; 
    //для GPU розрахунків
    private RenderTexture m_TinyTexture;
    private Texture2D m_OnePixel;
    private Rect m_Rect1x1;

    void Start()
    {
        if (m_CamDisplay != null)
        {
            m_Mat = m_CamDisplay.material;
        }
        else
        {
            Debug.LogError("AutoBrightness: RawImage==null");
            enabled = false;
            return;
        }
        // ID
        m_LightShaderID = Shader.PropertyToID("_Light");
        //все в 1 піксель
        m_TinyTexture = new RenderTexture(1, 1, 0);
        m_OnePixel = new Texture2D(1, 1, TextureFormat.RGB24, false);
        m_Rect1x1 = new Rect(0, 0, 1, 1);
        StartCoroutine(CheckBrightnessRoutine());
    }

    void Update()
    {
        if (m_Mat == null) return;
        m_CurrentLightVal = Mathf.Lerp(m_CurrentLightVal, m_TargetLightVal, Time.deltaTime * m_SmoothSpeed);// плавна зміна яскравості
        m_Mat.SetFloat(m_LightShaderID, m_CurrentLightVal);
    }

    IEnumerator CheckBrightnessRoutine()
    {
        WebCamTexture tempCam = null;
        // Очікування включення камери
        while (tempCam == null || !tempCam.isPlaying)
        {
            tempCam = m_CamDisplay.texture as WebCamTexture;
            yield return null;
        }
        m_Cam = (WebCamTexture)m_CamDisplay.texture;
        // Аналіз яскравості
        while (true)
        {
            CalculateOnGPU();
            yield return new WaitForSeconds(m_CheckInterval);
        }
    }

    void CalculateOnGPU()
    {
        if (m_Cam == null || m_Cam.width < 16) return;
        Graphics.Blit(m_Cam, m_TinyTexture);// в 1 піксель
        RenderTexture.active = m_TinyTexture;
        m_OnePixel.ReadPixels(m_Rect1x1, 0, 0);//копіювання з gpu в сpu
        m_OnePixel.Apply();
        RenderTexture.active = null;
        Color avgColor = m_OnePixel.GetPixel(0, 0);//Отримання кольору
        // Яскравість Люма
        float avgLum = (avgColor.r * 0.2126f) + (avgColor.g * 0.7152f) + (avgColor.b * 0.0722f);
        // Захист від ділення на нуль якщо темрява
        if (avgLum < 0.05f) avgLum = 0.05f;
        float neededMult = m_TargetGray / avgLum;
        m_TargetLightVal = Mathf.Clamp(neededMult, m_MinLight, m_MaxLight);
    }

    void OnDestroy()
    {
        if (m_TinyTexture != null) m_TinyTexture.Release();
    }
}