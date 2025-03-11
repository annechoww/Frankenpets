// using System.Collections;
// using System.Diagnostics;
// using UnityEngine;

// public class FindTasks : MonoBehaviour
// {
//     [Header("List of main lighting components in this room")]
//     public Light[] roomLights;

//     [Header("Parent object containing all task lights")]
//     public GameObject taskLightsParent;

//     [Header("Parent object containing all arrows and particles")]
//     public GameObject arrowsAndParticlesParent;

//     [Header("Other variables")]
//     public InputHandler player1Input;
//     public InputHandler player2Input;

//     private GameObject[] taskLights;
//     private float[] roomLightIntensities;

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         // Get all task lights in the room
//         taskLights = GameObject.FindGameObjectsWithTag("TaskLight");
//         UnityEngine.Debug.Log("Found " + taskLights.Length + " task lights");

//         foreach (GameObject taskLight in taskLights)
//         {
//             taskLight.SetActive(false);
//         }

//         // Store the original intensities of the room lights
//         roomLightIntensities = new float[roomLights.Length];

//         for (int i = 0; i < roomLights.Length; i++)
//         {
//             roomLightIntensities[i] = roomLights[i].intensity;
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (player1Input.GetGlowPressed() || player2Input.GetGlowPressed())
//         {
//             UnityEngine.Debug.Log("Locating tasks");
//             startFindTasksEffect();
//         }

//         if (!player1Input.GetGlowPressed() && !player2Input.GetGlowPressed())
//         {
//             UnityEngine.Debug.Log("Stop locating tasks");
//             endFindTasksEffect();
//         }
//     }

//     private void startFindTasksEffect()
//     {
//         dimRoomLights();

//         arrowsAndParticlesParent.SetActive(true);

//         foreach (GameObject taskLight in taskLights)
//         {
//             if (taskLight != null)
//             {
//                 taskLight.SetActive(true);
//                 StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 15.0f, fadeDuration));
//             }
//         }
//     }

//     private void endFindTasksEffect()
//     {
//         foreach (GameObject taskLight in taskLights)
//         {
//             if (taskLight != null)
//             {
//                 StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 0.0f, fadeDuration));
//                 // StartCoroutine(DelaySetActive(taskLight, false, 2.5f));
//                 taskLight.SetActive(false);
//             }
//         }

//         arrowsAndParticlesParent.SetActive(false);

//         brightenRoomLights();
//     }

//     private void dimRoomLights()
//     {
//         for (int i = 0; i < roomLights.Length; i++)
//         {
//             StartCoroutine(LerpLightIntensity(roomLights[i], 0.0f, fadeDuration));
//         }
//     }

//     private void brightenRoomLights()
//     {
//         for (int i = 0; i < roomLights.Length; i++)
//         {
//             StartCoroutine(LerpLightIntensity(roomLights[i], roomLightIntensities[i], fadeDuration));
//         }
//     }

//     private IEnumerator LerpLightIntensity(Light light, float targetIntensity, float duration)
//     {
//         float startIntensity = light.intensity;
//         float time = 0.0f;

//         while (time < duration)
//         {
//             light.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
//             time += Time.deltaTime;
//             yield return null;
//         }

//         light.intensity = targetIntensity;
//     }

//     private IEnumerator DelaySetActive(GameObject obj, bool isActive, float seconds)
//     {
//         yield return new WaitForSeconds(seconds);
//         obj.SetActive(isActive);
//     }

//     public void DestroyFindTaskMechanic(GameObject arrow, GameObject particle, GameObject light)
//     {
//         Destroy(arrow);
//         Destroy(particle);
//         Destroy(light);
//     }
// }

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

        // foreach (GameObject taskLight in taskLights)
        // {
        //     taskLight.SetActive(false);
        // }

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
                glowCoroutine = StartCoroutine(StartFindTasksEffect());
            }

            // UnityEngine.Debug.Log("Locating tasks");
            // startFindTasksEffect();
        } else if (!player1Input.GetGlowPressed() && !player2Input.GetGlowPressed())
        {
            if (glowCoroutine != null)
            {
                StopCoroutine(glowCoroutine);
                StartCoroutine(EndFindTasksEffect());
            }
            // UnityEngine.Debug.Log("Stop locating tasks");
            // endFindTasksEffect();
        }
    }

    private IEnumerator StartFindTasksEffect()
    {
        isGlowing = true;

        dimRoomLights();

        arrowsAndParticlesParent.SetActive(true);

        foreach (GameObject taskLight in taskLights)
        {
            // if (taskLight != null)
            // {
                // taskLight.SetActive(true);
                StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 15.0f, fadeDuration));
            // }
        }
        
        yield return null;
    }

    private IEnumerator EndFindTasksEffect()
    {
        isGlowing = false;

        foreach (GameObject taskLight in taskLights)
        {
            // if (taskLight != null)
            // {
                StartCoroutine(LerpLightIntensity(taskLight.GetComponent<Light>(), 0.0f, fadeDuration));
                // StartCoroutine(DelaySetActive(taskLight, false, 2.5f));
                // taskLight.SetActive(false);
            // }
        }

        arrowsAndParticlesParent.SetActive(false);

        StartCoroutine(BrightenRoomLights());

        yield return null;
    }

    private void dimRoomLights()
    {
        // StopAllLightCoroutines();
        StopCoroutine("LerpLightIntensity");
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], 0.0f, fadeDuration));
        }
    }

    private IEnumerator BrightenRoomLights()
    {
        // StopAllLightCoroutines();
        for (int i = 0; i < roomLights.Length; i++)
        {
            StartCoroutine(LerpLightIntensity(roomLights[i], roomLightIntensities[i], fadeDuration));
        }
        yield return null;
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

    private void StopAllLightCoroutines()
{
    // StopAllCoroutines();  // This stops all coroutines on this script

    // Alternatively, if you want finer control, stop only specific coroutines:
    foreach (Light light in roomLights)
    {
        StopCoroutine("LerpLightIntensity");
    }
}

    public void DestroyFindTaskMechanic(GameObject arrow, GameObject particle, GameObject light)
    {
        Destroy(arrow);
        Destroy(particle);
        Destroy(light);
    }
}
