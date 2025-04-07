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

    [Header("Mud particles")]
    public ParticleSystem mudJumpParticles;

    [Header("Script references")] 
    public PlayerActions playerActionsDF; 
    public PlayerActions playerActionsCF;
    public PlayerManager playerManager; 


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

    void OnTriggerStay(Collider other)
    {
        if (playerActionsCF.isPaw || playerActionsDF.isPaw)
        {
            // Instantiate mud particles
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("mud collision " + collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude > 0.2f && 
           (collision.gameObject.CompareTag("dog front") || 
            collision.gameObject.CompareTag("dog back")  ||
            collision.gameObject.CompareTag("cat front") ||
            collision.gameObject.CompareTag("cat back")))
        {
            // Play mud splash particle effect
            if (mudJumpParticles != null && !mudJumpParticles.isPlaying)
            {
                mudJumpParticles.Play();
            }
        }
    }

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
            GameObject pawPrint1 = Instantiate(pawPrintPrefab, foot1Position, pawPrintRotation);
            GameObject pawPrint2 = Instantiate(pawPrintPrefab, foot2Position, pawPrintRotation);

            // Destroy paw print after 5 seconds
            StartCoroutine(FadeAndDestroy(pawPrint1, 5.0f));
            StartCoroutine(FadeAndDestroy(pawPrint2, 5.0f));

            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;
        }

        activePawPrints.Remove(half);
    }

    private IEnumerator FadeAndDestroy(GameObject pawPrint, float fadeDuration)
    {
        SpriteRenderer sr = pawPrint.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color originalColor = sr.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0f, elapsedTime / fadeDuration);
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(pawPrint);
    }
}
