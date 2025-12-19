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
    public float maxFOVBoost = 14f;
    public float fovLerpSpeed = 4f;

    [Header("Dash FOV Boost")]
    public float dashFOVBoost = 6f;
    public float dashFOVTime = 0.12f;

    [Header("Vignette (Speed Based)")]
    public float maxVignette = 0.35f;
    public float vignetteLerpSpeed = 3f;

    [Header("Dash Vignette")]
    public float dashVignettePeak = 0.55f;
    public float dashVignetteTime = 0.12f;

    [Header("Turn Skew (Dutch)")]
    public float maxDutch = 12f;
    public float dutchSpeed = 16f;

    [Header("Camera Shake")]
    public float maxShake = 0.35f;

    // ===== internal =====
    float currentFOV;
    float currentDutch;

    float dashFOVTimer;
    float dashVignetteTimer;

    float smoothedSpeedRatio;
    float speedVelocity;

    bool wasDashing;

    Vignette vignette;

    void Start()
    {
        if (vcam != null)
            currentFOV = vcam.m_Lens.FieldOfView;

        if (volume != null)
        {
            volume.profile = Instantiate(volume.profile);
            volume.profile.TryGetSettings(out vignette);
        }
    }

    void Update()
    {
        if (player == null || vcam == null) return;

        // ===== 速度割合（SmoothDampで震え防止）=====
        float rawSpeedRatio = Mathf.Clamp01(player.currentSpeed / player.maxSpeed);

        smoothedSpeedRatio = Mathf.SmoothDamp(
            smoothedSpeedRatio,
            rawSpeedRatio,
            ref speedVelocity,
            0.15f
        );

        // ===== 入力 =====
        bool isDash =
            Keyboard.current != null &&
            Keyboard.current.leftShiftKey.isPressed;

        float h =
            (Keyboard.current.dKey.isPressed ? 1 : 0) -
            (Keyboard.current.aKey.isPressed ? 1 : 0);

        bool isDashing = isDash && smoothedSpeedRatio > 0.85f;

        // ===== ダッシュ開始検出 =====
        if (isDashing && !wasDashing)
        {
            dashFOVTimer = dashFOVTime;
            dashVignetteTimer = dashVignetteTime;

            if (impulseSource != null)
                impulseSource.GenerateImpulse(Vector3.up * maxShake);
        }

        // ===== FOV =====
        float dashFOV = 0f;
        if (dashFOVTimer > 0f)
        {
            dashFOV = dashFOVBoost;
            dashFOVTimer -= Time.deltaTime;
        }

        float targetFOV =
            baseFOV +
            maxFOVBoost * smoothedSpeedRatio +
            dashFOV;

        currentFOV = Mathf.Lerp(
            currentFOV,
            targetFOV,
            Time.deltaTime * fovLerpSpeed
        );

        vcam.m_Lens.FieldOfView = currentFOV;

        // ===== Vignette（完全に速度連動・チカチカ防止）=====
        if (vignette != null)
        {
            float dashBoost = 0f;
            if (dashVignetteTimer > 0f)
            {
                dashBoost = dashVignettePeak - maxVignette;
                dashVignetteTimer -= Time.deltaTime;
            }

            float targetVignette =
                maxVignette * smoothedSpeedRatio +
                dashBoost;

            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                targetVignette,
                Time.deltaTime * vignetteLerpSpeed
            );
        }

        // ===== カメラ傾き（Dutch）=====
        float dutchMultiplier = Mathf.Lerp(0.6f, 1.3f, smoothedSpeedRatio);

        float targetDutch =
            -h * maxDutch * dutchMultiplier;

        if (isDashing)
            targetDutch *= 1.25f;

        currentDutch = Mathf.Lerp(
            currentDutch,
            targetDutch,
            Time.deltaTime * dutchSpeed
        );

        vcam.m_Lens.Dutch = currentDutch;

        // ===== 状態保存 =====
        wasDashing = isDashing;
    }
}
