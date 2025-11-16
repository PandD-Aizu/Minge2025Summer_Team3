using System.Collections;
using UnityEngine;

namespace Minge2025Summer.Scripts.InGame.EnvironmentObject
{
    public class LightFlickObject : MonoBehaviour
    {
        [SerializeField] private Light targetLight;
        [SerializeField] private float minIntensity = 0f;
        [SerializeField] private float maxIntensity = 1f;
        [SerializeField] private float minInterval = 0.05f;
        [SerializeField] private float maxInterval = 0.25f;
        [SerializeField] private bool useSmoothTransition = true;
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private bool startOnAwake = true;

        private Coroutine flickerCoroutine;

        private void Start()
        {
            if (targetLight == null)
                targetLight = GetComponent<Light>();

            if (targetLight == null)
                return;

            if (startOnAwake)
                StartFlicker();
        }

        public void StartFlicker()
        {
            if (targetLight == null || flickerCoroutine != null)
                return;

            flickerCoroutine = StartCoroutine(FlickerRoutine());
        }

        public void StopFlicker()
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
            }
        }

        private IEnumerator FlickerRoutine()
        {
            while (true)
            {
                if (targetLight == null)
                    yield break;

                float current = targetLight.intensity;
                float target = Random.Range(minIntensity, maxIntensity);
                float wait = Random.Range(minInterval, maxInterval);

                if (useSmoothTransition)
                {
                    float t = 0f;
                    while (t < 1f)
                    {
                        if (targetLight == null) yield break;
                        targetLight.intensity = Mathf.Lerp(current, target, t);
                        t += Time.deltaTime * smoothSpeed;
                        yield return null;
                    }
                }
                else
                {
                    targetLight.intensity = target;
                }

                yield return new WaitForSeconds(wait);
            }
        }
    }
}
