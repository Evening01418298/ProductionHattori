using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class DashCameraEffect : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public CinemachineVirtualCamera vcam;
    public CinemachineImpulseSource impulseSource;
    public PostProcessVolume volume;

    [Header("FOV")]
    public float baseFOV = 60f;
    public float maxFOVBoost = 12f;
    public float fovLerpSpeed = 6f;

    [Header("Vignette")]
    public float maxVignette = 0.35f;
    public float vignetteLerpSpeed = 6f;

    [Header("Camera Shake")]
    public float maxShake = 0.3f;

    [Header("Dash Vignette")]
    public float dashVignettePeak = 0.55f;   // 一瞬だけ強く
    public float dashVignetteTime = 0.12f;   // 強調時間
    float dashVignetteTimer;
    bool wasDashing;


    float currentFOV;
    Vignette vignette;

    void Start()
    {
        if (vcam != null)
            currentFOV = vcam.m_Lens.FieldOfView;

        if (volume != null)
            volume.profile.TryGetSettings(out vignette);
    }

    void Update()
    {
        if (player == null || vcam == null) return;

        // ===== 速度割合 =====
        float speedRatio = Mathf.Clamp01(player.currentSpeed / player.maxSpeed);

        // ===== ダッシュ入力（Input System）=====
        bool isDash =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        // ===== FOV（速度連動）=====
        float targetFOV = baseFOV + maxFOVBoost * speedRatio;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovLerpSpeed);
        vcam.m_Lens.FieldOfView = currentFOV;

        // ===== Vignette（速度連動）=====
        if (vignette != null)
        {
            float targetVignette = maxVignette * speedRatio;

            // ダッシュ開始時だけ一瞬強め
            if (isDash && speedRatio > 0.9f)
                targetVignette += 0.15f;

            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                targetVignette,
                Time.deltaTime * vignetteLerpSpeed
            );
        }

        // ===== カメラ揺れ（速度連動）=====
        if (impulseSource != null && speedRatio > 0.1f)
        {
            Vector3 impulse = Vector3.up * maxShake * speedRatio;
            impulseSource.GenerateImpulse(impulse);
        }

    }
}
