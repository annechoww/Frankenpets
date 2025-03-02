using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class TailRigging : MonoBehaviour
{
    // target that the tail rig target will rig towards
    public Transform tailTarget;
    public float speed;
    public float range;
    public float naturalSpeed;
    public float naturalRange;
    public float horizontalOffset;
    public float verticalOffset;

    private Vector3 neutralPosition;
    private bool isMoving = false;
    private bool isMovingNaturally = false;

    void Update(){
        neutralPosition = tailTarget.position + transform.up * verticalOffset + transform.right * horizontalOffset;
        // Ray ray =  new Ray(neutralPosition, Vector3.down);
        // Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
    }

    public void naturalTailMovement()
    {
        // Debug.Log("natural movement");
        if (!isMoving && !isMovingNaturally){
            //transform.position += new Vector3(0, 0, Mathf.Sin(Time.time*3) * 0.001f);
            isMovingNaturally = true;
            isMoving = false;
            StartCoroutine(SwingTail(naturalSpeed, naturalRange));
        }
    }

    public void useTail()
    {
        Debug.Log("use tail");
        isMoving = true;
        isMovingNaturally = false;
        StartCoroutine(SwingTail(speed, range));
    }

    private IEnumerator SwingTail(float tailSpeed, float tailRange)
    {
        float duration = tailSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float phase = (elapsedTime / duration) * Mathf.PI * 2; // Maps time to sin wave
            float targetZ = Mathf.Sin(phase) * tailRange; // Move left → right → neutral

            Vector3 targetPosition = neutralPosition + transform.forward * targetZ;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*tailSpeed);
            
            //Ray ray =  new Ray(targetPosition, Vector3.down);
            //Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, duration=2.0f);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the tail returns exactly to neutral
        // transform.position = neutralPosition;
        isMovingNaturally = false;
        isMoving= false;
    }


}