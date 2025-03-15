using UnityEngine;
using System.Collections;

public class PedalLidController : MonoBehaviour
{
    public Transform lidHinge;
    public float openAngle = 90f;
    public float speed = 2f;

    private Quaternion closedRotation; 
    private Quaternion openRotation; 
    private Coroutine moveCoroutine;

    void Start()
    {
        closedRotation = lidHinge.rotation;
        openRotation = Quaternion.Euler(lidHinge.rotation.eulerAngles.x, lidHinge.rotation.eulerAngles.y, lidHinge.rotation.eulerAngles.z - openAngle);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            if (moveCoroutine != null) StopCoroutine(moveCoroutine);
            moveCoroutine = StartCoroutine(MoveLid(openRotation));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
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
