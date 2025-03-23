using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;

public class MudTracks : MonoBehaviour
{
    [Header("Mud paw print variables")]
    public GameObject dogPawPrintPrefab;
    public GameObject catPawPrintPrefab;
    public float spawnInterval = 0.5f; // Time between prints
    public float pawPrintsDuration = 6.0f; // How long the prints will appear 
    private float timer = 0f;
    private Dictionary<GameObject, Coroutine> activePawPrints = new Dictionary<GameObject, Coroutine>();

    [Header("Targets for cat front's paw prints")]
    public Transform CFTarget1;
    public Transform CFTarget2;
    [Header("Targets for cat back's paw prints")]
    public Transform CBTarget1;
    public Transform CBTarget2;
    [Header("Targets for dog front's paw prints")]
    public Transform DFTarget1;
    public Transform DFTarget2;   
    [Header("Targets for dog back's paw prints")] 
    public Transform DBTarget1;
    public Transform DBTarget2;    

    [Header("Script references")] 
    public PlayerActions playerActions; 
    public PlayerActions playerManager; 


    void OnTriggerExit(Collider other)
    {
        UnityEngine.Debug.Log("muddy " + other);
        
        if (!activePawPrints.ContainsKey(other.gameObject))
        {
            if (other.gameObject.CompareTag("cat front"))
            {
                Coroutine routine = StartCoroutine(SpawnPawPrints(other.gameObject, catPawPrintPrefab, CFTarget1, CFTarget2));
                activePawPrints.Add(other.gameObject, routine);
            }

            else if (other.gameObject.CompareTag("cat back"))
            {
                Coroutine routine = StartCoroutine(SpawnPawPrints(other.gameObject, catPawPrintPrefab, CBTarget1, CBTarget2));
                activePawPrints.Add(other.gameObject, routine);
            }

            else if (other.gameObject.CompareTag("dog front"))
            {
                Coroutine routine = StartCoroutine(SpawnPawPrints(other.gameObject, dogPawPrintPrefab, DFTarget1, DFTarget2));
                activePawPrints.Add(other.gameObject, routine);
            }

            else if (other.gameObject.CompareTag("dog back"))
            {
                Coroutine routine = StartCoroutine(SpawnPawPrints(other.gameObject, dogPawPrintPrefab, DBTarget1, DBTarget2));
                activePawPrints.Add(other.gameObject, routine);
            }
        }
    }

    // void OnTriggerStay(Collider other)
    // {
    //     if (playerActions.isPaw)
    //     {
    //         // Instantiate mud particles
    //     }
    // }

    private IEnumerator SpawnPawPrints(GameObject half, GameObject pawPrintPrefab, Transform foot1, Transform foot2)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < pawPrintsDuration)
        {
            if (half == null) break;
            
            Quaternion pawPrintRotation = Quaternion.Euler(90, half.transform.eulerAngles.y, half.transform.eulerAngles.z);
            Vector3 foot1Position = new Vector3(foot1.position.x, 0, foot1.position.z); 
            Vector3 foot2Position = new Vector3(foot2.position.x, 0, foot2.position.z); 

            // check that player is moving ?
            Instantiate(pawPrintPrefab, foot1Position, pawPrintRotation);
            Instantiate(pawPrintPrefab, foot2Position, pawPrintRotation);

            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }

        activePawPrints.Remove(half);
    }
}
