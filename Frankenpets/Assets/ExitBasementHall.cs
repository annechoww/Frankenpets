using UnityEngine;
using System.Collections;

public class ExitBasementHall : MonoBehaviour
{
    public GameObject lightsParent;
    private Light[] lights;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lights = lightsParent.GetComponentsInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("cat front") || other.gameObject.CompareTag("dog front")) 
        {
            foreach (Light light in lights)
            {
                StartCoroutine(LerpLightIntensity(light, 0.0f, 2.0f));
            }

            StartCoroutine(DelaySetActive(lightsParent, false, 2.5f));

        }
    }

    private IEnumerator LerpLightIntensity(Light light, float targetIntensity, float duration)
    {
        float startIntensity = light.intensity;
        float time = 0.0f;

        while (time < duration)
        {
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        light.intensity = targetIntensity;
    }

    private IEnumerator DelaySetActive(GameObject obj, bool isActive, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(isActive);
    }
}
