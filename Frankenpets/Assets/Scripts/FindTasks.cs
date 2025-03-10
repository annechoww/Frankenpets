using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FindTasks : MonoBehaviour
{
    [Header("List of main lighting components in this room")]
    public Light[] roomLights;

    [Header("Parent object containing all task lights")]
    public GameObject taskLightsParent;

    [Header("Parent object containing all arrows and particles")]
    public GameObject arrowsAndParticlesParent;

    [Header("Other variables")]
    public InputHandler player1Input;
    public InputHandler player2Input;

    private float glowDuration = 2.5f;
    private Light[] taskLights;
    private float[] roomLightIntensities;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get all task lights in the room
        taskLights = taskLightsParent.GetComponentsInChildren<Light>();

        // Store the original intensities of the room lights
        roomLightIntensities = new float[roomLights.Length];

        for (int i = 0; i < roomLights.Length; i++)
        {
            roomLightIntensities[i] = roomLights[i].intensity;
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
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], 0.5f, 1.0f));
        }
    }

    private void brightenRoomLights()
    {
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], roomLightIntensities[i], 1.0f));
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
