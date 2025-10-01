using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EndPath : MonoBehaviour
{
    public GameObject endPathContainer;
    public GameObject arrow;
    public GameObject lightsParent;
    private Light[] lights;
    public TaskManager taskManager;
    private List<Task> basementTasks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endPathContainer.gameObject.SetActive(false);
        lights = lightsParent.GetComponentsInChildren<Light>();
        basementTasks = TaskManager.GetAllTasksOfLevel(2);
    }

    private void Update()
    {
        if (TaskManager.CheckTaskCompletion(basementTasks) && !gameObject.activeSelf)
        {
            endPathContainer.gameObject.SetActive(true);

            lightsParent.gameObject.SetActive(true);
            foreach (Light light in lights)
            {
                StartCoroutine(LerpLightIntensity(light, 0.0f, 2.0f));
            }
        }
    }

    void OnTriggerEnter(Collider other)
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
