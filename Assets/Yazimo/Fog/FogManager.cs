using UnityEngine;
using UnityEngine.InputSystem;

public class FogManager : MonoBehaviour
{
    public InputAction fogKnob;
    public float smoothSpeed = 3f;

    private FogControllable[] fogs;
    private float lastValue = 0f;

    void Start()
    {
        fogKnob.Enable();
        fogs = FindObjectsOfType<FogControllable>();
    }

    void Update()
    {
        float knobValue = fogKnob.ReadValue<float>();
        lastValue = Mathf.Lerp(lastValue, knobValue, Time.deltaTime * smoothSpeed); // 平滑

        float alpha = Mathf.Lerp(0f, 0.4f, lastValue); // 控制透明度

        foreach (var fog in fogs)
        {
            if (fog == null) continue;
            var ps = fog.GetComponent<ParticleSystem>();

            // Emission rate 過渡
            var emission = ps.emission;
            emission.rateOverTime = lastValue * fog.maxRate;

            // Alpha 過渡
            var colorOverLifetime = ps.colorOverLifetime;
            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(alpha, 0.3f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            colorOverLifetime.color = grad;
        }
    }
}
