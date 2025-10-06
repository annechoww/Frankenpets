using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExitBasementHall : MonoBehaviour
{
    public GameObject lightsParent;
    public GameObject pawPath;
    private Light[] lights;
    private List<Task> basementTasks;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lights = lightsParent.GetComponentsInChildren<Light>();
        pawPath.SetActive(true);
        lightsParent.SetActive(true);
        basementTasks = TaskManager.GetAllTasksOfLevel(2);
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
            pawPath.SetActive(false);
            Debug.Log($"Initial lightsParent and pawPath deactivated");

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
