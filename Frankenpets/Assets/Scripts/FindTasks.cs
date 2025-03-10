using UnityEngine;

public class FindTasks : MonoBehaviour
{
    [Header("List of main lighting components in this room")]
    public Light[] roomLights;

    [Header("Parent object containing all task lights")]
    public GameObject taskLightsParent;

    [Header("Parent object containing all arrows and particles")]
    public GameObject arrowsAndParticlesParent;

    private float glowDuration = 2.5f;
    private Light[] taskLights;
    private float[] roomLightIntensities;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        taskLights = taskLights.GetComponentsInChildren<Light>();

        foreach (Light light in roomLights)
        {
            roomLightIntensities.Add(light.intensity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
        {
            UnityEngine.Debug.Log("Glow button pressed");
            StartCoroutine(FindTasksEffect());
        }
    }

    private IEnumerator FindTasksEffect()
    {
        dimRoomLights();

        arrowsAndParticlesParent.SetActive(true);

        foreach (Light light in taskLights)
        {
            StartCoroutine(LerpLightIntensity(light, 0.1f, 1.0f));
        }

        yield return new WaitForSeconds(glowDuration);

        foreach (Light light in taskLights)
        {
            StartCoroutine(LerpLightIntensity(light, 0.0f, 1.0f));
        }

        arrowsAndParticlesParent.SetActive(false);

        brightenRoomLights();
    }

    private void dimRoomLights()
    {
        foreach (int idx in roomLights.Length)
        {
            StartCoroutine(LerpLightIntensity(roomLights[idx], 0.5f, 1.0f));
        }
    }

    private void brightenRoomLights()
    {
        foreach (int idx in roomLights.Length)
        {
            StartCoroutine(LerpLightIntensity(roomLights[idx], roomLightIntensities[idx], 1.0f));
        }
    }

    private IEnumerator LerpLightIntensity(Light light, float targetIntensity, float duration)
    {
        float startIntensity = light.intensity;
        float time = 0.0f;

        while (time < duration)
        {
            light.intensity = Mathf.lerp(startIntensity, targetIntensity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        light.intensity = targetIntensity;
    }
}
