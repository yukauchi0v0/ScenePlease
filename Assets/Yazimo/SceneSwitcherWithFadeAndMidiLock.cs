using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class SceneSwitcherStableOptimized : MonoBehaviour
{
    public Transform characterTarget;
    public Transform skyPosition;
    public Transform cityPosition;
    public Transform seaPosition;

    public Image fadeImage;
    public float transitionDuration = 2f;

    private bool isTransitioning = false;
    private bool hasStarted = false;

    private enum Region { Sea, City, Sky }
    private Region currentRegion = Region.City;

    [Header("區間判斷範圍")]
    public float seaMax = 0.3f;
    public float cityMin = 0.35f;
    public float cityMax = 0.65f;
    public float skyMin = 0.7f;

    [Header("Input System Actions")]
    public InputAction regionSlider;
    public InputAction goSeaButton;
    public InputAction goCityButton;
    public InputAction goSkyButton;

    private float lastKnobValue = -1f;
    private float knobThreshold = 0.01f;
    private bool sliderHasMoved = false;

    void OnEnable()
    {
        regionSlider.Enable();
        goSeaButton.Enable();
        goCityButton.Enable();
        goSkyButton.Enable();
    }

    void OnDisable()
    {
        regionSlider.Disable();
        goSeaButton.Disable();
        goCityButton.Disable();
        goSkyButton.Disable();
    }

    void Start()
    {
        // ❌ 不要轉場，不要黑幕，不要移動角色位置
        // ✅ 只設為當前區域（不動角色）
        float knobValue = regionSlider.ReadValue<float>();
        if (knobValue <= seaMax)
            currentRegion = Region.Sea;
        else if (knobValue >= skyMin)
            currentRegion = Region.Sky;
        else if (knobValue >= cityMin && knobValue <= cityMax)
            currentRegion = Region.City;

        lastKnobValue = knobValue;
        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
        hasStarted = true;
    }

    void Update()
    {
        if (!hasStarted || isTransitioning) return;

        // ✅ 按鍵優先
        if (goSeaButton.WasPressedThisFrame())
        {
            TrySwitchRegion(Region.Sea);
            return;
        }
        if (goCityButton.WasPressedThisFrame())
        {
            TrySwitchRegion(Region.City);
            return;
        }
        if (goSkyButton.WasPressedThisFrame())
        {
            TrySwitchRegion(Region.Sky);
            return;
        }

        // ✅ 滑桿只有第一次動才會啟用切換功能
        float knobValue = regionSlider.ReadValue<float>();
        if (!sliderHasMoved && Mathf.Abs(knobValue - lastKnobValue) > knobThreshold)
        {
            sliderHasMoved = true;
        }

        if (sliderHasMoved && Mathf.Abs(knobValue - lastKnobValue) > knobThreshold)
        {
            lastKnobValue = knobValue;

            Region targetRegion = currentRegion;
            if (knobValue <= seaMax)
                targetRegion = Region.Sea;
            else if (knobValue >= skyMin)
                targetRegion = Region.Sky;
            else if (knobValue >= cityMin && knobValue <= cityMax)
                targetRegion = Region.City;

            TrySwitchRegion(targetRegion);
        }
    }

    void TrySwitchRegion(Region targetRegion)
    {
        if (targetRegion != currentRegion)
        {
            currentRegion = targetRegion;
            StartCoroutine(SwitchTo(GetTargetForRegion(currentRegion)));
        }
    }

    Transform GetTargetForRegion(Region region)
    {
        switch (region)
        {
            case Region.Sea: return seaPosition;
            case Region.Sky: return skyPosition;
            default: return cityPosition;
        }
    }

    IEnumerator SwitchTo(Transform target)
    {
        isTransitioning = true;

        // ✅ 漸變黑幕進入
        if (fadeImage != null)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / (transitionDuration / 2f);
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1f);
        }

        yield return new WaitForSeconds(0.05f);

        Vector3 startPos = characterTarget.position;
        Vector3 endPos = target.position;

        float moveT = 0f;
        while (moveT < transitionDuration)
        {
            moveT += Time.deltaTime;
            float lerpT = moveT / transitionDuration;
            characterTarget.position = Vector3.Lerp(startPos, endPos, lerpT);
            yield return null;
        }

        characterTarget.position = endPos;

        // ✅ 漸變回來
        if (fadeImage != null)
        {
            float t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime / (transitionDuration / 2f);
                fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(t));
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0f);
        }

        isTransitioning = false;
    }
}
