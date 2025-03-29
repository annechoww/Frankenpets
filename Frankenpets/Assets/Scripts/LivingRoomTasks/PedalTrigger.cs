using UnityEngine;
using System.Collections;

public class PedalLidController : MonoBehaviour
{
    [Header("Lid Opening")]
    public Transform lidHinge;
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Audio")]
    public AudioClip pedalSound;

    private Quaternion closedRotation; 
    private Quaternion openRotation; 
    private Coroutine moveCoroutine;
    private PlayerActions pet;

    // Flags
    private bool pedalPressed = false;

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
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
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
                // Play pedal sound
                if (pedalSound != null && !pedalPressed)
                {
                    pedalPressed = true;
                    AudioManager.Instance.PlaySFX(pedalSound);
                }
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveLid(openRotation));
            }
            else if (pet != null && !pet.isPaw)
            {
                pedalPressed = false;
                if (moveCoroutine != null) StopCoroutine(moveCoroutine);
                moveCoroutine = StartCoroutine(MoveLid(closedRotation));
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            pet = other.GetComponent<PlayerActions>();
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveLid(closedRotation));
        }
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
