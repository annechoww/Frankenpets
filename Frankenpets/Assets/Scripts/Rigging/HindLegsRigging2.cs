using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class HindLegsRigging2: MonoBehaviour
{
    [Header("Dog Back Bones")]
    public Transform dogFrontTorso;
    public Transform dogBackTorso;
    public Transform dogLeg1;
    public Transform dogLeg2;

    private Quaternion dogFrontTorsoOrigRotation;
    private Quaternion dogFrontTorsoTargetRotation;
    private Quaternion dogBackTorsoOrigRotation;
    private Quaternion dogBackTorsoTargetRotation;
    private Quaternion dogL1OGRotation;
    private Quaternion dogL1TargetRotation;
    private Quaternion dogL2OGRotation;
    private Quaternion dogL2TargetRotation;

    [Header("Cat Front Bones")]
    public Transform catFrontTorso;
    public Transform catLeg1;
    public Transform catLeg2;

    private Quaternion catFrontTorsoOrigRotation;
    private Quaternion catFrontTorsoTargetRotation;
    private Quaternion catL1OGRotation;
    private Quaternion catL1TargetRotation;
    private Quaternion catL2OGRotation;
    private Quaternion catL2TargetRotation;

    [Header("Hind Legs Variables")]
    public Rig catFrontRig;
    public float rotateSpeed;
    public float FrontTorsoRotation;
    public float BackTorsoRotation;
    public float BackLegRotation;
    public float catTorsoRotation;
    public float FrontLegRotation;

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
        dogBackTorsoOrigRotation = dogBackTorso.localRotation;
        dogL1OGRotation = dogLeg1.localRotation;
        dogL2OGRotation = dogLeg2.localRotation;

        catFrontTorsoOrigRotation = catFrontTorso.localRotation;
        catL1OGRotation = catLeg1.localRotation;
        catL2OGRotation = catLeg2.localRotation;
    }

    public void stand()
    {
        catFrontRig.weight = 0f;
        Debug.Log("standing script");
        // dog
        Vector3 dogFrontTorsoEuler = dogFrontTorso.localRotation.eulerAngles;
        dogFrontTorsoTargetRotation = Quaternion.Euler(FrontTorsoRotation, dogFrontTorsoEuler.y, dogFrontTorsoEuler.z);

        Vector3 dogBackTorsoEuler = dogBackTorso.localRotation.eulerAngles;
        dogBackTorsoTargetRotation = Quaternion.Euler(BackTorsoRotation, dogBackTorsoEuler.y, dogBackTorsoEuler.z);

        Vector3 dogBackLeg1Euler = dogLeg1.localRotation.eulerAngles;
        dogL1TargetRotation = Quaternion.Euler(BackLegRotation, dogBackLeg1Euler.y, dogBackLeg1Euler.z);

        Vector3 dogBackLeg2Euler = dogLeg2.localRotation.eulerAngles;
        dogL2TargetRotation = Quaternion.Euler(BackLegRotation, dogBackLeg2Euler.y, dogBackLeg2Euler.z);

        dogFrontTorso.localRotation = Quaternion.Lerp(dogFrontTorso.localRotation,dogFrontTorsoTargetRotation, rotateSpeed);
        dogBackTorso.localRotation = Quaternion.Lerp(dogBackTorso.localRotation,dogBackTorsoTargetRotation, rotateSpeed);
        dogLeg1.localRotation = Quaternion.Lerp(dogLeg1.localRotation,dogL1TargetRotation, rotateSpeed);
        dogLeg2.localRotation = Quaternion.Lerp(dogLeg2.localRotation,dogL2TargetRotation, rotateSpeed);

        //cat
        Vector3 catTorsoEuler = catFrontTorso.localRotation.eulerAngles;
        catFrontTorsoTargetRotation = Quaternion.Euler(catTorsoRotation, catTorsoEuler.y, catTorsoEuler.z);

        Vector3 catLeg1Euler = catLeg1.localRotation.eulerAngles;
        catL1TargetRotation = Quaternion.Euler(catLeg1Euler.x, catLeg1Euler.y, FrontLegRotation);

        Vector3 catLeg2Euler = catLeg2.localRotation.eulerAngles;
        catL2TargetRotation = Quaternion.Euler(catLeg2Euler.x, catLeg2Euler.y, FrontLegRotation);

        catFrontTorso.localRotation = Quaternion.Lerp(catFrontTorso.localRotation, catFrontTorsoTargetRotation, rotateSpeed);
        catLeg1.localRotation = Quaternion.Lerp(catLeg1.localRotation, catL1TargetRotation, rotateSpeed);
        catLeg2.localRotation = Quaternion.Lerp(catLeg2.localRotation, catL2TargetRotation, rotateSpeed);

    }

    public void release()
    {
        // }else if (Input.GetKeyDown(KeyCode.C))
        // {
        catFrontRig.weight = 1.0f;
        dogFrontTorso.localRotation = Quaternion.Lerp(dogFrontTorso.localRotation,dogFrontTorsoOrigRotation, rotateSpeed);
        dogBackTorso.localRotation = Quaternion.Lerp(dogBackTorso.localRotation,dogBackTorsoOrigRotation, rotateSpeed);
        dogLeg1.localRotation = Quaternion.Lerp(dogLeg1.localRotation,dogL1OGRotation, rotateSpeed);
        dogLeg2.localRotation = Quaternion.Lerp(dogLeg2.localRotation,dogL2OGRotation, rotateSpeed);

        catFrontTorso.localRotation = Quaternion.Lerp(catFrontTorso.localRotation, catFrontTorsoOrigRotation, rotateSpeed);
        catLeg1.localRotation = Quaternion.Lerp(catLeg1.localRotation, catL1OGRotation, rotateSpeed);
        catLeg2.localRotation = Quaternion.Lerp(catLeg2.localRotation, catL2OGRotation, rotateSpeed);
        
    }


}