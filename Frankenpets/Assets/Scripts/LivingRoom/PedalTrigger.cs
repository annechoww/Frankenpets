using UnityEngine;
using System.Collections;

public class PedalLidController : MonoBehaviour
{
    [Header("Lid Opening")]
    public Transform lidHinge;
    public float openAngle = 90f;
    public float speed = 2f;
    public float closeDelay = 5f;

    [Header("Audio")]
    public AudioClip pedalSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine moveCoroutine;
    private Coroutine delayCoroutine;
    private PlayerActions pet;
    
    // Flags
    private bool pedalPressed = false;
    private bool isClosing = false;

    void Start()
    {
        closedRotation = lidHinge.rotation;
        openRotation = Quaternion.Euler(lidHinge.rotation.eulerAngles.x, lidHinge.rotation.eulerAngles.y, lidHinge.rotation.eulerAngles.z - openAngle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            pet = other.GetComponent<PlayerActions>();
            if (pet != null && pet.isPaw)
            {
                UnityEngine.Debug.Log(lidHinge.rotation);
                
                // Cancel any closing delay
                if (delayCoroutine != null)
                {
                    StopCoroutine(delayCoroutine);
                    delayCoroutine = null;
                }
                
                // Cancel any movement in progress
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                
                isClosing = false;
                moveCoroutine = StartCoroutine(MoveLid(openRotation));
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            pet = other.GetComponent<PlayerActions>();
            if (pet != null && pet.isPaw)
            {
                // Play pedal sound if not already pressed
                if (pedalSound != null && !pedalPressed)
                {
                    pedalPressed = true;
                    AudioManager.Instance.PlaySFX(pedalSound);
                }
                
                // Cancel any closing delay
                if (delayCoroutine != null)
                {
                    StopCoroutine(delayCoroutine);
                    delayCoroutine = null;
                }
                
                // Open the lid
                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }
                
                isClosing = false;
                moveCoroutine = StartCoroutine(MoveLid(openRotation));
            }
            else if (pet != null && !pet.isPaw && !isClosing)
            {
                pedalPressed = false;
                
                // Start the closing delay
                if (delayCoroutine != null)
                {
                    StopCoroutine(delayCoroutine);
                }
                
                isClosing = true;
                delayCoroutine = StartCoroutine(DelayedClose());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            pedalPressed = false;
            
            // Start delayed closing
            if (!isClosing)
            {
                if (delayCoroutine != null)
                {
                    StopCoroutine(delayCoroutine);
                }
                
                isClosing = true;
                delayCoroutine = StartCoroutine(DelayedClose());
            }
        }
    }

    IEnumerator DelayedClose()
    {
        yield return new WaitForSeconds(closeDelay);
        
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        
        moveCoroutine = StartCoroutine(MoveLid(closedRotation));
        isClosing = false;
        delayCoroutine = null;
    }

    IEnumerator MoveLid(Quaternion targetRotation)
    {
        while (Quaternion.Angle(lidHinge.rotation, targetRotation) > 0.1f)
        {
            lidHinge.rotation = Quaternion.Lerp(lidHinge.rotation, targetRotation, Time.deltaTime * speed);
            yield return null;
        }
        
        lidHinge.rotation = targetRotation;
    }
}