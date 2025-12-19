using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessDebug : MonoBehaviour
{
    public PostProcessVolume volume;

    Vignette vignette;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("PostProcessVolume が設定されていません");
            return;
        }

        if (volume.profile.TryGetSettings(out vignette))
        {
            Debug.Log("Vignette 取得成功");
        }
        else
        {
            Debug.LogError("Vignette が Profile に存在しません");
        }
    }

    void Update()
    {
        if (vignette == null) return;

        // 0〜1 を往復させる
        float value = Mathf.PingPong(Time.time, 1f);
        vignette.intensity.value = value;

        Debug.Log($"Vignette Intensity = {value:F2}");
    }
}
