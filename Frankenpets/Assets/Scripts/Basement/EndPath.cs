using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EndPath : MonoBehaviour
{
    public GameObject endPathContainer;
    public GameObject arrow;
    public GameObject lightsParent;
    private Light[] lights;
    private List<Task> basementTasks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endPathContainer.SetActive(false);
        lights = lightsParent.GetComponentsInChildren<Light>();
        basementTasks = TaskManager.GetAllTasksOfLevel(2);
    }

    private void Update()
    {
        if (TaskManager.CheckTaskCompletion(basementTasks) && !endPathContainer.activeSelf)
        {
            Debug.Log("free pet task completed - activating end path");

            endPathContainer.SetActive(true);

            lightsParent.SetActive(true);
            foreach (Light light in lights)
            {
                light.gameObject.SetActive(true);
                StartCoroutine(LerpLightIntensity(light, 0.5f, 2.0f));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("cat front") || other.gameObject.CompareTag("dog front"))
        {
            arrow.gameObject.SetActive(false);
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
}
