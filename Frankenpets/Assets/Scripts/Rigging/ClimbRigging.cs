using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class ClimbRigging: MonoBehaviour
{
    [Header("Cat Front Bones")]
    public Transform catTorso;
    private Quaternion catTorsoOrigRotation;
    private Quaternion catTorsoTargetRotation;

    [Header("Dog Back Bones")]
    // public Transform dogTorso;
    // public Transform dogTorsoPoint;
    // public Transform dogTailbone;
    // public Transform dogLeftLeg;
    // public Transform dogRightLeg;

    // private Quaternion dogTorsoOrigRotation;
    // private Quaternion dogTorsoTargetRotation;
    // private Quaternion dogTorsoPointOrigRotation;
    // private Quaternion dogTorsoPointTargetRotation;
    // private Quaternion dogTailboneOrigRotation;
    // private Quaternion dogTailboneTargetRotation;
    // private Quaternion dogLeftLegOrigRotation;
    // private Quaternion dogLeftLegTargetRotation;
    // private Quaternion dogRightLegOrigRotation;
    // private Quaternion dogRightLegTargetRotation;
    public Transform dogParentBone;
    private Quaternion dogParentBoneOrigRotation;
    private Quaternion dogParentBoneTargetRotation;
    private Vector3 dogParentBoneOrigPosition;
    private Vector3 dogParentBoneTargetPosition;

    private Quaternion origRotation;

    [Header("Climbing Variables")]
    public float rotateSpeed;
    public float catRotation;
    public float dogRotation;
    public float dogVerticalOffset;

    void Start()
    {
        catTorsoOrigRotation = catTorso.localRotation;
        // dogTorsoOrigRotation = dogTorso.localRotation;
        // dogTorsoPointOrigRotation = dogTorsoPoint.localRotation;
        // dogTailboneOrigRotation = dogTailbone.localRotation;
        // dogLeftLegOrigRotation = dogLeftLeg.localRotation;
        // dogRightLegOrigRotation = dogRightLeg.localRotation;
        dogParentBoneOrigRotation = dogParentBone.localRotation;
    }

    public void climb()
    {   
        //cat
        Vector3 catTorsoEuler = catTorso.localRotation.eulerAngles;
        catTorsoTargetRotation = Quaternion.Euler(catRotation, catTorsoEuler.y, catTorsoEuler.z);
        catTorso.localRotation = Quaternion.Lerp(catTorso.localRotation, catTorsoTargetRotation, rotateSpeed);
        
        dogParentBoneOrigPosition = dogParentBone.localPosition;
        // dog
        Vector3 dogParentBoneEuler = dogParentBone.localRotation.eulerAngles;
        dogParentBoneTargetRotation = Quaternion.Euler(dogRotation, dogParentBoneEuler.y, dogParentBoneEuler.z);
        dogParentBone.localRotation = Quaternion.Lerp(dogParentBone.localRotation, dogParentBoneTargetRotation, rotateSpeed);
        
        dogParentBoneTargetPosition = dogParentBone.localPosition + Vector3.forward * dogVerticalOffset;
        dogParentBone.localPosition = Vector3.Lerp(dogParentBone.localPosition, dogParentBoneTargetPosition, rotateSpeed);
    }

    public void release()
    {
        catTorso.localRotation = Quaternion.Lerp(catTorso.localRotation, catTorsoOrigRotation, rotateSpeed);
        dogParentBone.localRotation = Quaternion.Lerp(dogParentBone.localRotation, dogParentBoneOrigRotation, rotateSpeed);
        dogParentBone.localPosition = Vector3.Lerp(dogParentBone.localPosition, dogParentBoneOrigPosition, rotateSpeed);
        
    }

    // current bugs: facing the right direction


}
