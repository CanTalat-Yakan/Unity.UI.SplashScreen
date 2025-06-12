using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Unity.Essentials
{
    public class SplashScreenTimeline : MonoBehaviour
    {
        [Header("Customization")]
        [SerializeField] private SplashScreenCustomizer _customizer;

        [Header("Audio")]
        [SerializeField] private AudioSource _sound;

        [Header("Materials")]
        [SerializeField] private Material _unityLabel;
        [SerializeField] private Material _unityLogo;
        [SerializeField] private Material _unityLogoOutline;
        [SerializeField] private Material _glowTriangle;

        [Header("Lights")]
        [SerializeField] private Light _whitePointLight;
        [SerializeField] private float _whitePointLightIntensity;
        [SerializeField] private Light _greenPointLight;
        [SerializeField] private float _greenPointLightIntensity;

        [Header("Lens Flares")]
        [SerializeField] private LensFlareComponentSRP _logoLensFlare;
        [SerializeField] private LensFlareComponentSRP _greenPointLightLensFlare;

        [Header("Act 1")]
        [SerializeField] private GameObject _act1;
        [SerializeField] private int _act1Duration;

        [Header("Act 2")]
        [SerializeField] private UIDocument _act2Document;
        [SerializeField] private GameObject _act2GameObject;
        [SerializeField] private int _act2Duration;

        [Header("Act 3")]
        [SerializeField] private UIDocument _act3Document;
        [SerializeField] private GameObject _act3GameObject;
        [SerializeField] private int _act3Duration;

        [Space]

        [Header("Settings")]
        [Tooltip("A multiplier for slowing down or speeding up the entire timeline.")]
        [SerializeField, Range(0.1f, 5f)] private float _slowMotionMultiplier = 1f;
        [Tooltip("Controls how much to blend between linear interpolation (0) and smooth step (1).\r\n Values > 1 will “overdrive” the smooth step.")]
        [SerializeField, Range(0f, 2f)] private float _smoothStrength = 1f;

        private const string ExposureMapStrengthProperty = "_Exposure_Map_Strength";
        private const string ExposureMaskFalloffStrengthProperty = "_Exposure_Mask_Falloff_Strength";

        public void OnValidate() =>
            ValidateInputs();

        public void Awake() =>
            ValidateInputs();

        public void Start() =>
            StartCoroutine(ExecuteTimeline());

        public void Update()
        {
            if (!_customizer.IsLoadingScenes && Input.anyKeyDown)
            {
                StopAllCoroutines();

                _sound.Stop();

                _customizer.OnFinalization();
            }

            // Keep the lens flare intensity in sync with green light's intensity.
            if (_greenPointLightLensFlare != null && _greenPointLight != null && _greenPointLightIntensity > 0f)
            {
                _greenPointLightLensFlare.intensity = _greenPointLight.intensity / _greenPointLightIntensity;
                _logoLensFlare.intensity = _greenPointLight.intensity / _greenPointLightIntensity;

                if (_logoLensFlare.intensity > 0)
                    _logoLensFlare.intensity *= 0.1f;
            }
        }

        private void ValidateInputs()
        {
            ValidateMaterial(_unityLabel);
            ValidateMaterial(_unityLogo);
            ValidateMaterial(_unityLogoOutline);
            ValidateMaterial(_glowTriangle);

            if (!_whitePointLight)
                Debug.LogWarning("White Point Light is not assigned.", this);

            if (!_greenPointLight)
                Debug.LogWarning("Green Point Light is not assigned.", this);
        }

        private void ValidateMaterial(Material material)
        {
            if (material == null)
            {
                Debug.LogWarning("A Material is missing.", this);
                return;
            }

            if (!material.HasProperty(ExposureMapStrengthProperty))
                Debug.LogWarning($"Material '{material.name}' does not have property '{ExposureMapStrengthProperty}'.", this);
        }

        private IEnumerator ExecuteTimeline()
        {
            // Reset initial states for acts, materials and lights.
            _act1.gameObject.SetActive(false);
            _act2GameObject.gameObject.SetActive(false);
            _act3GameObject.gameObject.SetActive(false);

            ResetMaterialExposure(_unityLabel);
            ResetMaterialExposure(_unityLogo);
            ResetMaterialExposure(_unityLogoOutline);
            ResetMaterialExposure(_glowTriangle);

            ResetLightIntensity(_whitePointLight);
            ResetLightIntensity(_greenPointLight);

            ResetLensFlareScale(_logoLensFlare);
            ResetPositionX(_logoLensFlare.gameObject, -0.5f);

            yield return null;

            // 1. Outline appears then fades.
            StartCoroutine(ChangeMaterialExposureFalloff(_unityLogoOutline, value: 1.5f, delay: 0f));
            StartCoroutine(ChangeMaterialExposure(_unityLogoOutline, targetValue: 1f, duration: 1.5f, delay: 0f));

            StartCoroutine(ChangeMaterialExposureFalloff(_unityLogoOutline, value: 5f, delay: 2.25f));
            StartCoroutine(ChangeMaterialExposure(_unityLogoOutline, targetValue: 0f, duration: 1f, delay: 2.25f));

            // 2. Unity logo and label fade in.
            StartCoroutine(ChangeMaterialExposure(_unityLogo, targetValue: 1f, duration: 2f, delay: 2.5f));
            StartCoroutine(ChangeMaterialExposure(_unityLabel, targetValue: 1f, duration: 3f, delay: 2.5f));

            // 3. White light fades in and then out.
            StartCoroutine(ChangeLightIntensity(_whitePointLight, targetValue: _whitePointLightIntensity, duration: 1f, delay: 2.25f));
            StartCoroutine(ChangeLightIntensity(_whitePointLight, targetValue: 0f, duration: 2f, delay: 4.75f));

            // 4. Glow triangle and green point light fade in and then out.
            StartCoroutine(ChangeMaterialExposure(_glowTriangle, targetValue: 1f, duration: 2f, delay: 2.5f));
            StartCoroutine(ChangeLightIntensity(_greenPointLight, targetValue: _greenPointLightIntensity, duration: 2f, delay: 2.5f));
            StartCoroutine(ChangeMaterialExposure(_glowTriangle, targetValue: 0f, duration: 1.75f, delay: 4.5f));
            StartCoroutine(ChangeLightIntensity(_greenPointLight, targetValue: 0f, duration: 1.75f, delay: 4.5f));

            // 5. Big lens flare fade in and then out.
            StartCoroutine(ChangeLensFlareSize(_logoLensFlare, targetValue: 2f, duration: 2f, delay: 2.5f));
            StartCoroutine(ChangeLensFlareSize(_logoLensFlare, targetValue: 1f, duration: 1.75f, delay: 4.5f));
            StartCoroutine(ChangePositionX(_logoLensFlare.gameObject, targetValue: -1f, duration: 4f, delay: 2.5f));

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            _sound.pitch /= _slowMotionMultiplier;
            _sound.Play();

            // Act 1
            _act1.gameObject.SetActive(true);
            _customizer.OnAct1Enabled();
            yield return new WaitForSeconds(Mathf.CeilToInt(_act1Duration * _slowMotionMultiplier));
            _act1.gameObject.SetActive(false);

            // Act 2
            _act2GameObject.gameObject.SetActive(true);
            _customizer.OnAct2Enabled(_act2Document);
            yield return new WaitForSeconds(Mathf.CeilToInt(_act2Duration * _slowMotionMultiplier));
            _act2GameObject.gameObject.SetActive(false);

            // Act 3
            _act3GameObject.gameObject.SetActive(true);
            _customizer.OnAct3Enabled(_act3Document);
            yield return new WaitForSeconds(Mathf.CeilToInt(_act3Duration * _slowMotionMultiplier));
            _act3GameObject.gameObject.SetActive(false);

            yield return new WaitWhile(() => _customizer.IsLoadingScenes);

            _customizer.OnFinalization();

            yield return null;
        }

        public IEnumerator ChangeMaterialExposure(Material material, float targetValue, float duration, float delay)
        {
            if (!IsValidMaterial(material))
                yield break;

            if (delay > 0f)
                yield return new WaitForSeconds(delay * _slowMotionMultiplier);

            float initialValue = material.GetFloat(ExposureMapStrengthProperty);
            yield return StartCoroutine(ChangePropertyOverTime(
                initialValue,
                targetValue,
                duration,
                value => material.SetFloat(ExposureMapStrengthProperty, value)
            ));
        }

        public IEnumerator ChangeMaterialExposureFalloff(Material material, float value, float delay)
        {
            if (!IsValidMaterial(material))
                yield break;

            if (delay > 0f)
                yield return new WaitForSeconds(delay * _slowMotionMultiplier);

            material.SetFloat(ExposureMaskFalloffStrengthProperty, value);
        }

        public IEnumerator ChangeLightIntensity(Light light, float targetValue, float duration, float delay)
        {
            if (light == null)
                yield break;

            if (delay > 0f)
                yield return new WaitForSeconds(delay * _slowMotionMultiplier);

            float initialValue = light.intensity;
            yield return StartCoroutine(ChangePropertyOverTime(
                initialValue,
                targetValue,
                duration,
                value => light.intensity = Mathf.Clamp(value, 0f, Mathf.Infinity)
            ));
        }

        public IEnumerator ChangeLensFlareSize(LensFlareComponentSRP lensFlare, float targetValue, float duration, float delay)
        {
            if (lensFlare == null)
                yield break;

            if (delay > 0f)
                yield return new WaitForSeconds(delay * _slowMotionMultiplier);

            float initialValue = lensFlare.scale;
            yield return StartCoroutine(ChangePropertyOverTime(
                initialValue,
                targetValue,
                duration,
                value => lensFlare.scale = value
            ));
        }

        public IEnumerator ChangePositionX(GameObject gameObject, float targetValue, float duration, float delay)
        {
            if (gameObject == null)
                yield break;

            if (delay > 0f)
                yield return new WaitForSeconds(delay * _slowMotionMultiplier);

            float initialValue = gameObject.transform.localPosition.x;
            yield return StartCoroutine(ChangePropertyOverTime(
                initialValue,
                targetValue,
                duration,
                value => gameObject.transform.localPosition = new Vector3(value, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z)
            ));
        }

        private IEnumerator ChangePropertyOverTime(float initialValue, float targetValue, float duration, System.Action<float> updateProperty)
        {
            float elapsed = 0f;
            duration *= _slowMotionMultiplier;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Interpolate based on the selected mode.
                float newT = Interpolate(t);
                float newValue = Mathf.Lerp(initialValue, targetValue, newT);

                updateProperty(newValue);

                yield return null;
            }

            // Ensure final value is set.
            updateProperty(targetValue);
        }

        private float Interpolate(float t) =>
            SmoothStepOverdrive(t, _smoothStrength);

        private float SmoothStepOverdrive(float t, float strength)
        {
            // Standard smooth step 
            float smooth = t * t * (3f - 2f * t);  // same as Mathf.SmoothStep(0,1,t)

            // Linear
            float linear = t;

            // Blend: linear + strength * (smooth - linear)
            //  - strength=0   => linear
            //  - strength=1   => smooth
            //  - strength>1   => overshoot
            return linear + strength * (smooth - linear);
        }

        private void ResetMaterialExposure(Material material)
        {
            if (IsValidMaterial(material))
                material.SetFloat(ExposureMapStrengthProperty, 0f);
        }

        private void ResetLightIntensity(Light light)
        {
            if (light != null)
                light.intensity = 0f;
        }

        private void ResetLensFlareScale(LensFlareComponentSRP lensFlare)
        {
            if (lensFlare != null)
                lensFlare.scale = 1;
        }

        private void ResetPositionX(GameObject gameObject, float value)
        {
            if (gameObject != null)
                gameObject.transform.localPosition = new Vector3(value, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
        }

        private bool IsValidMaterial(Material material) =>
            material != null && material.HasProperty(ExposureMapStrengthProperty);
    }
}