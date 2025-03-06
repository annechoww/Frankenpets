using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class GrabRigging: MonoBehaviour
{
    [Header("dog Front Bones")]
    public Transform dogTorso;
    public Transform dogNeck;
    public Transform dogHead;
    public Transform objectGrabPoint;

    private Quaternion dogTorsoOrigRotation;
    private Quaternion dogTorsoTargetRotation;
    private Vector3 dogTorsoOrigPosition;
    private Vector3 dogTorsoTargetPosition;

    private Quaternion dogNeckOrigRotation;
    private Quaternion dogNeckTargetRotation;
    private Vector3 dogNeckOrigPosition;
    private Vector3 dogNeckTargetPosition;

    private Quaternion dogHeadOrigRotation;
    private Quaternion dogHeadTargetRotation;
    private Vector3 dogHeadOrigPosition;
    private Vector3 dogHeadTargetPosition;

    [Header("Grabbing Variables")]
    public float rotateSpeed;
    public float headVerticalOffset;
    public float dogTorsoRotation;
    public float dogNeckRotation;
    public float dogHeadRotation;

    // grabbing
    private bool isGrab = false;
    private Rigidbody targetRigidbody;
    
    void Start()
    {
        dogTorsoOrigPosition = dogTorso.localPosition;
        dogTorsoOrigRotation = dogTorso.localRotation;
        dogNeckOrigPosition = dogNeck.localPosition;
        dogNeckOrigRotation = dogNeck.localRotation;
        dogHeadOrigPosition = dogHead.localPosition;
        dogHeadOrigRotation = dogHead.localRotation;
    }

    public void FixedUpdate()
    {
        if (isGrab){
            float lerpSpeed = 30f;
            Vector3 newPosition = Vector3.Lerp(targetRigidbody.position, objectGrabPoint.position, Time.deltaTime * lerpSpeed);
            targetRigidbody.MovePosition(newPosition);
        }

    }

    public void drag()
    {
            
        // Vector3 dogTorsoEuler = dogTorso.localRotation.eulerAngles;
        // dogTorsoTargetRotation = Quaternion.Euler(dogTorsoRotation, dogTorsoEuler.y, dogTorsoEuler.z);
        // dogTorso.localRotation = Quaternion.Lerp(dogTorso.localRotation, dogTorsoTargetRotation, rotateSpeed);

        Vector3 dogNeckEuler = dogNeck.localRotation.eulerAngles;
        dogNeckTargetRotation = Quaternion.Euler(dogNeckRotation, dogNeckEuler.y, dogNeckEuler.z);
        dogNeck.localRotation = Quaternion.Lerp(dogNeck.localRotation, dogNeckTargetRotation, rotateSpeed);

        Vector3 dogHeadEuler = dogHead.localRotation.eulerAngles;
        dogHeadTargetRotation = Quaternion.Euler(dogHeadRotation, dogHeadEuler.y, dogHeadEuler.z);
        dogHead.localRotation = Quaternion.Lerp(dogHead.localRotation, dogHeadTargetRotation, rotateSpeed);

        dogHeadTargetPosition = dogHead.localPosition + Vector3.forward * headVerticalOffset;
        dogHead.localPosition = Vector3.Lerp(dogHead.localPosition, dogHeadTargetPosition, rotateSpeed);
        
    }

    public void grab(Rigidbody target)
    {
        isGrab = true;
        targetRigidbody = target;
        targetRigidbody.useGravity = false;
    }
    
    
    public void release()
    {
        if (isGrab){
            isGrab = false;
            targetRigidbody.useGravity = true;
            targetRigidbody = null;
        }
            
        dogNeck.localRotation = Quaternion.Lerp(dogNeck.localRotation, dogNeckOrigRotation, rotateSpeed*5);
        dogHead.localRotation = Quaternion.Lerp(dogHead.localRotation, dogHeadOrigRotation, rotateSpeed*5);
        dogHead.localPosition = Vector3.Lerp(dogHead.localPosition, dogHeadOrigPosition, rotateSpeed*5);

    }

}