using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class HindLegsRigging2: MonoBehaviour
{
    [Header("Dog Back Bones")]
    public Transform dogFrontTorso;
    public Transform dogLeg1;
    public Transform dogLeg2;
    public Transform dogTail;

    private Quaternion dogFrontTorsoOrigRotation;
    private Quaternion dogFrontTorsoTargetRotation;
    private Quaternion dogL1OGRotation;
    private Quaternion dogL1TargetRotation;
    private Quaternion dogL2OGRotation;
    private Quaternion dogL2TargetRotation;
    private Quaternion dogTailOGRotation;
    private Quaternion dogTailTargetRotation;

    [Header("Cat Front Bones")]
    public Transform catFrontTorso;
    public Transform catLeg1;
    public Transform catLeg2;
    public Transform catCalf1;
    public Transform catCalf2;

    private Quaternion catFrontTorsoOrigRotation;
    private Quaternion catFrontTorsoTargetRotation;
    private Quaternion catL1OGRotation;
    private Quaternion catL1TargetRotation;
    private Quaternion catL2OGRotation;
    private Quaternion catL2TargetRotation;

    private Vector3 catCalf1OGPosition;
    private Vector3 catCalf1TargetPosition;
    private Vector3 catCalf2OGPosition;
    private Vector3 catCalf2TargetPosition;

    [Header("Hind Legs Variables")]
    public Rig catFrontRig;
    public float rotateSpeed;
    public float FrontTorsoRotation;
    public float BackLegRotation;
    public float tailRotation;
    public float catTorsoRotation;
    public float FrontLegRotation;
    public float FrontLegVerticalOffset;

    private PlayerManager playerManager;
    //private Stopwatch splitStopwatch = new Stopwatch();

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
        dogFrontTorsoOrigRotation = dogFrontTorso.localRotation;
        dogL1OGRotation = dogLeg1.localRotation;
        dogL2OGRotation = dogLeg2.localRotation;
        dogTailOGRotation = dogTail.localRotation;

        catFrontTorsoOrigRotation = catFrontTorso.localRotation;
        catL1OGRotation = catLeg1.localRotation;
        catL2OGRotation = catLeg2.localRotation;
        catCalf1OGPosition = catCalf1.localPosition;
        catCalf2OGPosition = catCalf2.localPosition;
    }

    public void stand()
    {
        catFrontRig.weight = 0f;
        Debug.Log("standing script");
        // dog
        Vector3 dogFrontTorsoEuler = dogFrontTorso.localRotation.eulerAngles;
        dogFrontTorsoTargetRotation = Quaternion.Euler(FrontTorsoRotation, dogFrontTorsoEuler.y, dogFrontTorsoEuler.z);

        Vector3 dogBackLeg1Euler = dogLeg1.localRotation.eulerAngles;
        dogL1TargetRotation = Quaternion.Euler(BackLegRotation, dogBackLeg1Euler.y, dogBackLeg1Euler.z);

        Vector3 dogBackLeg2Euler = dogLeg2.localRotation.eulerAngles;
        dogL2TargetRotation = Quaternion.Euler(BackLegRotation, dogBackLeg2Euler.y, dogBackLeg2Euler.z);

        Vector3 dogTailEuler = dogTail.localRotation.eulerAngles;
        dogTailTargetRotation = Quaternion.Euler(tailRotation, dogTailEuler.y, dogTailEuler.z);

        dogFrontTorso.localRotation = Quaternion.Lerp(dogFrontTorso.localRotation,dogFrontTorsoTargetRotation, rotateSpeed);
        dogLeg1.localRotation = Quaternion.Lerp(dogLeg1.localRotation,dogL1TargetRotation, rotateSpeed);
        dogLeg2.localRotation = Quaternion.Lerp(dogLeg2.localRotation,dogL2TargetRotation, rotateSpeed);
        dogTail.localRotation = Quaternion.Lerp(dogTail.localRotation,dogTailTargetRotation, rotateSpeed);

        //cat
        Vector3 catTorsoEuler = catFrontTorso.localRotation.eulerAngles;
        catFrontTorsoTargetRotation = Quaternion.Euler(catTorsoRotation, catTorsoEuler.y, catTorsoEuler.z);

        Vector3 catLeg1Euler = catLeg1.localRotation.eulerAngles;
        catL1TargetRotation = Quaternion.Euler(catLeg1Euler.x, catLeg1Euler.y, FrontLegRotation);

        Vector3 catLeg2Euler = catLeg2.localRotation.eulerAngles;
        catL2TargetRotation = Quaternion.Euler(catLeg2Euler.x, catLeg2Euler.y, -FrontLegRotation);

        catFrontTorso.localRotation = Quaternion.Lerp(catFrontTorso.localRotation, catFrontTorsoTargetRotation, rotateSpeed);
        catLeg1.localRotation = Quaternion.Lerp(catLeg1.localRotation, catL1TargetRotation, rotateSpeed);
        catLeg2.localRotation = Quaternion.Lerp(catLeg2.localRotation, catL2TargetRotation, rotateSpeed);

        //  Debug.Log("Current Position: " + catCalf1.localPosition);
        // catCalf1TargetPosition = catCalf1.localPosition + Vector3.right * FrontLegVerticalOffset;
        // catCalf1.localPosition = Vector3.Lerp(catCalf1.localPosition, catCalf1TargetPosition, rotateSpeed);
        // catCalf2TargetPosition = catCalf2.localPosition + Vector3.right * FrontLegVerticalOffset;
        // catCalf2.localPosition = Vector3.Lerp(catCalf2.localPosition, catCalf2TargetPosition, rotateSpeed);

    }

    public void release()
    {
        // }else if (Input.GetKeyDown(KeyCode.C))
        // {
        catFrontRig.weight = 1.0f;
        dogFrontTorso.localRotation = Quaternion.Lerp(dogFrontTorso.localRotation,dogFrontTorsoOrigRotation, rotateSpeed);
        dogLeg1.localRotation = Quaternion.Lerp(dogLeg1.localRotation,dogL1OGRotation, rotateSpeed);
        dogLeg2.localRotation = Quaternion.Lerp(dogLeg2.localRotation,dogL2OGRotation, rotateSpeed);
        dogTail.localRotation = Quaternion.Lerp(dogTail.localRotation,dogTailOGRotation, rotateSpeed);

        catFrontTorso.localRotation = Quaternion.Lerp(catFrontTorso.localRotation, catFrontTorsoOrigRotation, rotateSpeed);
        catLeg1.localRotation = Quaternion.Lerp(catLeg1.localRotation, catL1OGRotation, rotateSpeed);
        catLeg2.localRotation = Quaternion.Lerp(catLeg2.localRotation, catL2OGRotation, rotateSpeed);
        
        // catCalf1.localPosition = Vector3.Lerp(catCalf1.localPosition, catCalf1OGPosition, rotateSpeed);
        // catCalf2.localPosition = Vector3.Lerp(catCalf2.localPosition, catCalf2OGPosition, rotateSpeed);
        
    }


}