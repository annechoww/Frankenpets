using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class MouthRigging : MonoBehaviour
{
    public float moveSpeed;
    public float horizontalOffset; 
    public float verticalOffset;
    
    private Vector3 oldPosition, newPosition;
    private Vector3 oldNormal, newNormal;

    void Start()
    {
        oldPosition=transform.localPosition;
        oldNormal=newNormal=transform.up;
    }

    void Update()
    {   
        // newPosition = transform.localPosition + Vector3.down * verticalOffset;
        // Ray ray =  new Ray(transform.position, Vector3.down);
        // Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        
    }

    public void openMouth(){
        UnityEngine.Debug.Log("open mouth");
        // newPosition = transform.localPosition + Vector3.up * verticalOffset;
        // transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, moveSpeed);
        StartCoroutine(OpenAndClose());
    }

    private IEnumerator OpenAndClose()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        newPosition = transform.localPosition + Vector3.up * verticalOffset;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f); // Wait 1 second before closing

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, oldPosition, Time.deltaTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = oldPosition;
        }

}
