using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ShirnkRigging : MonoBehaviour
{
    [Header("Models")]
    public Transform dogFront; 
    public Transform catFront;
    public Transform dogBack; 
    public Transform catBack;

    [Header("Shrink/Expand Settings")]
    public float shrinkScale = 0.7f;
    public float shrinkSpeed = 2.0f; 
    public float expandSpeed = 20.0f;
    public float shrinkDuration = 1.5f;

    // State trackers
    private bool isShrinking = false;
    private Coroutine shrinkCoroutine;

    // Original pet half scales
    private Vector3 dogFrontOriginalScale;
    private Vector3 catFrontOriginalScale;
    private Vector3 dogBackOriginalScale;
    private Vector3 catBackOriginalScale;

    // Script references
    private PlayerManager playerManager;

    void Awake()
    {
        GameObject petObject = GameObject.Find("Pet");
        if (petObject != null)
        {
            playerManager = petObject.GetComponent<PlayerManager>();
            if (playerManager == null)
            {
                UnityEngine.Debug.LogError("GameObject 'Pet' not found in the scene.");
            }
        }
    }

    void Start()
    {
        dogFrontOriginalScale = dogFront.localScale;
        catFrontOriginalScale = catFront.localScale;
        dogBackOriginalScale = dogBack.localScale;
        catBackOriginalScale = catBack.localScale;
    }

    void Update()
    {
        if (playerManager.CheckSwitchInput() && playerManager.getCanSwitch())
        {
            if (!isShrinking)
            {
                shrinkCoroutine = StartCoroutine(Shrink());
            }
        }
        else if (!playerManager.CheckSwitchInput())
        {
            if (shrinkCoroutine != null)
            {
                StopCoroutine(shrinkCoroutine);
                StartCoroutine(Expand());
            }
        }
    }

    private IEnumerator Shrink()
    {
        isShrinking = true; 
        float elapsedTime = 0.0f;

        while (elapsedTime < shrinkDuration)
        {
            if (!playerManager.CheckSwitchInput()) // cancel shrinking
            {
                StartCoroutine(Expand());
                yield break;
            }
            
            Transform frontHalf = playerManager.getFrontHalf().transform;
            Transform backHalf = playerManager.getBackHalf().transform;

            Vector3 frontOriginalScale = getFrontOriginalScale();
            Vector3 backOriginalScale = getBackOriginalScale();

            Vector3 frontTargetScale = frontOriginalScale * shrinkScale;
            Vector3 backTargetScale = backOriginalScale * shrinkScale;

            frontHalf.localScale = Vector3.Lerp(frontHalf.localScale, frontTargetScale, Time.deltaTime * shrinkSpeed);
            backHalf.localScale = Vector3.Lerp(backHalf.localScale, backTargetScale, Time.deltaTime * shrinkSpeed);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        StartCoroutine(Expand()); // expand after switch is complete
    }

    private IEnumerator Expand()
    {
        isShrinking = false;

        Transform frontHalf = playerManager.getFrontHalf().transform;
        Transform backHalf = playerManager.getBackHalf().transform;

        Vector3 frontOriginalScale = getFrontOriginalScale();
        Vector3 backOriginalScale = getBackOriginalScale();

        while (frontHalf.localScale != frontOriginalScale || backHalf.localScale != backOriginalScale)
        {
            frontHalf.localScale = Vector3.Lerp(frontHalf.localScale, frontOriginalScale, Time.deltaTime * expandSpeed);
            backHalf.localScale = Vector3.Lerp(backHalf.localScale, backOriginalScale, Time.deltaTime * expandSpeed);
            yield return null;
        }
    }

    private Vector3 getFrontOriginalScale()
    {
        Player P1 = playerManager.P1;
        Player P2 = playerManager.P2;

        if ((P1.IsFront && P1.Species == "cat") || (P2.IsFront && P2.Species == "cat")) return catFrontOriginalScale;
        else if ((P1.IsFront && P1.Species == "dog") || P2.IsFront && P2.Species == "dog") return dogFrontOriginalScale;
        else return Vector3.zero;
    }

    private Vector3 getBackOriginalScale()
    {
        Player P1 = playerManager.P1;
        Player P2 = playerManager.P2;

        if ((!P1.IsFront && P1.Species == "cat") || (!P2.IsFront && P2.Species == "cat")) return catBackOriginalScale;
        else if ((!P1.IsFront && P1.Species == "dog") || !P2.IsFront && P2.Species == "dog") return dogBackOriginalScale;
        else return Vector3.zero;
    }
}
