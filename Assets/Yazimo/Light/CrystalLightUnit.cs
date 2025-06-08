using UnityEngine;

public class CrystalLightUnit : MonoBehaviour
{
    public Light crystalLight;

    [Range(0f, 10f)]
    public float maxIntensity = 3f;

    public void SetBrightness(float value)
    {
        if (crystalLight != null)
        {
            crystalLight.intensity = Mathf.Clamp01(value) * maxIntensity;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (crystalLight == null)
        {
            Transform light = transform.Find("PointLight") ?? transform.Find("CrystalLight");
            if (light != null)
                crystalLight = light.GetComponent<Light>();
        }
    }
#endif
}
