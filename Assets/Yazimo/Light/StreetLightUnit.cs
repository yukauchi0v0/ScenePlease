using UnityEngine;

public class StreetLightUnit : MonoBehaviour
{
    [Header("掛上這張亮光圖的 SpriteRenderer")]
    public SpriteRenderer lampGlowRenderer;

    [Header("燈光元件（Point Light / Spot Light）")]
    public Light pointLight;

    [Header("目前亮度 (自動控制)")]
    [Range(0f, 1f)]
    public float brightness = 0f;

    [Header("可調參數")]
    public float maxLightIntensity = 1f;
    public float minGlowAlpha = 0f;
    public float maxGlowAlpha = 1f;

    // --------- 自動找子物件 ----------
    public void InitIfNeeded()
    {
        if (lampGlowRenderer == null)
        {
            Transform glow = transform.Find("LampGlow");
            if (glow != null)
                lampGlowRenderer = glow.GetComponent<SpriteRenderer>();
        }

        if (pointLight == null)
        {
            Transform light = transform.Find("SpotLight") ?? transform.Find("PointLight");
            if (light != null)
                pointLight = light.GetComponent<Light>();
        }
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp01(value);

        InitIfNeeded(); // 保險起見，每次設定前都確保欄位有抓到

        if (lampGlowRenderer != null)
        {
            Color color = lampGlowRenderer.color;
            color.a = Mathf.Lerp(minGlowAlpha, maxGlowAlpha, brightness);
            lampGlowRenderer.color = color;
        }

        if (pointLight != null)
        {
            pointLight.intensity = brightness * maxLightIntensity;
        }
    }

#if UNITY_EDITOR
    // 這段會在你每次改 Prefab、拖進場景、或有任何變更時自動填欄位
    void OnValidate()
    {
        InitIfNeeded();
    }
#endif
}
