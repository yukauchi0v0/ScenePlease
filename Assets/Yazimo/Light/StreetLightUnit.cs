using UnityEngine;

public class StreetLightUnit : MonoBehaviour
{
    public SpriteRenderer lampGlowRenderer;
    public Light pointLight;

    [Range(0f, 1f)]
    public float brightness = 0f;

    public float maxLightIntensity = 1f;
    public float minGlowAlpha = 0f;
    public float maxGlowAlpha = 1f;

    void Awake()
    {
        // 如果沒指定，就自動找子物件
        if (lampGlowRenderer == null)
        {
            Transform glow = transform.Find("LampGlow");
            if (glow != null) lampGlowRenderer = glow.GetComponent<SpriteRenderer>();
        }

        if (pointLight == null)
        {
            Transform light = transform.Find("SpotLight") ?? transform.Find("PointLight");
            if (light != null) pointLight = light.GetComponent<Light>();
        }
    }

    public void SetBrightness(float value)
    {
        brightness = Mathf.Clamp01(value);

        // 控制圖片透明度
        if (lampGlowRenderer != null)
        {
            Color color = lampGlowRenderer.color;
            color.a = Mathf.Lerp(minGlowAlpha, maxGlowAlpha, brightness);
            lampGlowRenderer.color = color;
        }

        // 控制 Light 強度
        if (pointLight != null)
        {
            pointLight.intensity = brightness * maxLightIntensity;
        }
    }
}
