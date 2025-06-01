using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BirdBoostParticleController : MonoBehaviour
{
    [Header("金粉粒子控制（此物件應掛粒子系統）")]
    public ParticleSystem boostEffect;

    [Header("觸發條件")]
    public float boostThreshold = 0.1f;

    [Header("Boost 來源")]
    public BirdFlightControllerV2 birdController;

    private ParticleSystem.EmissionModule emission;

    void Start()
    {
        if (boostEffect == null)
            boostEffect = GetComponent<ParticleSystem>();

        if (boostEffect != null)
        {
            // 設定 Emission 模組
            emission = boostEffect.emission;
            emission.rateOverTime = 0f;
            emission.rateOverDistance = 0f;

            // 設定主模組
            var main = boostEffect.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startSpeed = 0f;
        }
        else
        {
            Debug.LogWarning("BoostEffect 粒子系統未設定！");
        }
    }

    void Update()
    {
        if (birdController == null || boostEffect == null)
            return;

        bool boosting = birdController.boostAmount > boostThreshold;

        // 僅控制 rateOverDistance
        emission.rateOverTime = 0f;
        emission.rateOverDistance = boosting ? 10f : 0f;
    }
}
