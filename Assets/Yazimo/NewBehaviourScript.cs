using UnityEngine;

public class FullscreenToggle : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }
}
