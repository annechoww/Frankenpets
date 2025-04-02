using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class TailRigging : MonoBehaviour
{
    public float speed;
    public float range;
    public float naturalSpeed;
    public float naturalRange;

    private Vector3 oldPosition;
    private bool isMoving = false;
    private bool isMovingNaturally = false;

    void Start(){
        oldPosition = transform.localPosition;
    }

    void OnEnable()
    {
        isMoving = false;
        isMovingNaturally = false;
    }

      void Update()
    {   
        // newPosition = transform.localPosition + Vector3.down * verticalOffset;
        Ray ray =  new Ray(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        
    }

    public void naturalTailMovement()
    {
        if (!isMoving && !isMovingNaturally){
            isMovingNaturally = true;
            isMoving = false;
            StopCoroutine(swingTailCoroutine()); 
            StartCoroutine(naturalTailCoroutine());
            
        }
    }

    private IEnumerator naturalTailCoroutine()
    {
        float duration = naturalSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float phase = (elapsedTime / duration) * Mathf.PI * 2;
            float targetZ = Mathf.Sin(phase) * naturalRange;

            Vector3 targetPosition = oldPosition + Vector3.forward * targetZ;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime*naturalSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving= false;
        isMovingNaturally = false;
    }

    public void useTail()
    {
        isMoving = true;
        isMovingNaturally = false;
        transform.localPosition = oldPosition;
        StopCoroutine(naturalTailCoroutine());
        StartCoroutine(swingTailCoroutine());
        transform.localPosition = oldPosition;
    }

    private IEnumerator swingTailCoroutine()
    {
        float duration = speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float phase = (elapsedTime / duration) * Mathf.PI * 2; // Maps time to sin wave
            float targetZ = Mathf.Sin(phase) * range; // Move left → right → neutral

            Vector3 targetPosition = oldPosition + Vector3.forward * targetZ;
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime*speed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isMoving= false;
        isMovingNaturally = false;
    }


}