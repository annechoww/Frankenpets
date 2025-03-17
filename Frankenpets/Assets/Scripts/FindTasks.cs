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
    private float fadeDuration = 0.5f;

    private GameObject[] taskLights;
    private float[] roomLightIntensities;
    private Coroutine glowCoroutine;
    private bool isGlowing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get all task lights in the room
        taskLights = GameObject.FindGameObjectsWithTag("TaskLight");
        UnityEngine.Debug.Log("Found " + taskLights.Length + " task lights");

        foreach (GameObject taskLight in taskLights)
        {
            taskLight.SetActive(false);
        }

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
            if (!isGlowing)
            {
                StopAllCoroutines();
                glowCoroutine = StartCoroutine(StartFindTasksEffect());
            }

        } else if (!player1Input.GetGlowPressed() && !player2Input.GetGlowPressed())
        {
            if (glowCoroutine != null)
            {
                StopAllCoroutines();
                glowCoroutine = null;
            
                StartCoroutine(EndFindTasksEffect());
            }
        }
    }

    private IEnumerator StartFindTasksEffect()
    {
        isGlowing = true;

        StartCoroutine(DimRoomLights());

        arrowsAndParticlesParent.SetActive(true);

        foreach (GameObject taskLight in taskLights)
        {
            if (taskLight != null)
            {
                taskLight.SetActive(true);
                StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 15.0f, fadeDuration));
            }
        }
        
        yield return null;
    }

    private IEnumerator EndFindTasksEffect()
    {
        isGlowing = false;

        foreach (GameObject taskLight in taskLights)
        {
            if (taskLight != null)
            {
                StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 0.0f, fadeDuration));
                StartCoroutine(DelaySetActive(taskLight, false, 2.5f));
                taskLight.SetActive(false);
            }
        }

        arrowsAndParticlesParent.SetActive(false);

        StartCoroutine(BrightenRoomLights());

        yield return null;
    }

    private IEnumerator DimRoomLights()
    {
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], 0.0f, fadeDuration));
            yield return null;
        }
    }

    private IEnumerator BrightenRoomLights()
    {
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], roomLightIntensities[i], fadeDuration));
            yield return null;
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

    public void DestroyFindTaskMechanic(GameObject arrow, GameObject particle, GameObject light)
    {
        Destroy(arrow);
        Destroy(particle);
        Destroy(light);
    }
}
