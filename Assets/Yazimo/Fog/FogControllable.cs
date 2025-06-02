using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FogControllable : MonoBehaviour
{
    public float maxRate = 50f;  // 每個霧可以設定自己最大的 rate
}
