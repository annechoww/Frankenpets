using UnityEngine;

public class StretchRigging : MonoBehaviour
{
    [Header("Dog Front Bones")]
    public Transform dfFrontTorso;
    private Vector3 dfFTOGPosition;
    private Vector3 dfFTTargetPosition;

    [Header("Dog Back Bones")]
    public Transform dbBackTorso;
    public Transform dbLeg1;
    public Transform dbLeg2;

    private Vector3 dbBTOGPosition;
    private Vector3 dbBTTargetPosition;
    private Vector3 dbL1OGPosition;
    private Vector3 dbL1TargetPosition;
    private Vector3 dbL2OGPosition;
    private Vector3 dbL2TargetPosition;
    

    [Header("Cat Front Bones")]
    public Transform cfFrontTorso;
    public Transform cfLeg1;
    public Transform cfLeg2;

    private Vector3 cfFTOGPosition;
    private Vector3 cfFTTargetPosition;
    private Vector3 cfL1OGPosition;
    private Vector3 cfL1TargetPosition;
    private Vector3 cfL2OGPosition;
    private Vector3 cfL2TargetPosition;

    [Header("Cat Back Bones")]
    public Transform cbBackTorso;
    public Transform cbLeg1;
    public Transform cbLeg2;

    private Vector3 cbBTOGPosition;
    private Vector3 cbBTTargetPosition;
    private Vector3 cbL1OGPosition;
    private Vector3 cbL1TargetPosition;
    private Vector3 cbL2OGPosition;
    private Vector3 cbL2TargetPosition;

    [Header("Stretch Variables")]
    public float frontStretchSpeed = 1.6f; 
    public float backStretchSpeed = 0.15f;
    public float destretchSpeed = 15f; 
    public float maxStretchDistance = 0.02f; 
    private Vector3 stretchDirection = new Vector3(0, 1, 0); // stretch along Y-axis
    
    private PlayerManager playerManager;

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
        // Dog Front
        dfFTOGPosition = dfFrontTorso.localPosition;
        dfFTTargetPosition = dfFTOGPosition;

        // Dog Back
        dbBTOGPosition = dbBackTorso.localPosition;
        dbBTTargetPosition = dbBTOGPosition;
        dbL1OGPosition = dbLeg1.localPosition;
        dbL1TargetPosition = dbL1OGPosition;
        dbL2OGPosition = dbLeg2.localPosition;
        dbL2TargetPosition = dbL2OGPosition;

        // Cat Front
        cfFTOGPosition = cfFrontTorso.localPosition;
        cfFTTargetPosition = cfFTOGPosition;
        cfL1OGPosition = cfLeg1.localPosition;
        cfL1TargetPosition = cfL1OGPosition;
        cfL2OGPosition = cfLeg2.localPosition;
        cfL2TargetPosition = cfL2OGPosition;

        // Cat Back
        cbBTOGPosition = cbBackTorso.localPosition;
        cbBTTargetPosition = cbBTOGPosition;
        cbL1OGPosition = cbLeg1.localPosition;
        cbL1TargetPosition = cbL1OGPosition;
        cbL2OGPosition = cbLeg2.localPosition;
        cbL2TargetPosition = cbL2OGPosition;
    }

    void Update()
    {
        if (playerManager.shouldStretch() && playerManager.getJoint() != null) 
        {
            // Dog Front
            dfFTTargetPosition = dfFTOGPosition + (stretchDirection.normalized * maxStretchDistance);

            // Cat Back
            cbBTTargetPosition = cbBTOGPosition + (stretchDirection.normalized * maxStretchDistance);
            cbL1TargetPosition = cbL1OGPosition + (stretchDirection.normalized * maxStretchDistance);
            cbL2TargetPosition = cbL2OGPosition + (stretchDirection.normalized * maxStretchDistance);

            // Cat Front
            cfFTTargetPosition = cfFTOGPosition + (stretchDirection.normalized * maxStretchDistance);
            cfL1TargetPosition = cfL1OGPosition + (stretchDirection.normalized * maxStretchDistance);
            cfL2TargetPosition = cfL2OGPosition + (stretchDirection.normalized * maxStretchDistance);
            
            // Dog Back
            dbBTTargetPosition = dbBTOGPosition + (stretchDirection.normalized * maxStretchDistance);
            dbL1TargetPosition = dbL1OGPosition + (stretchDirection.normalized * maxStretchDistance);
            dbL2TargetPosition = dbL2OGPosition + (stretchDirection.normalized * maxStretchDistance);

        }
        else 
        {
            // Return to normal when split is cancelled or pets split
            dfFTTargetPosition = dfFTOGPosition;

            dbBTTargetPosition = dbBTOGPosition;
            dbL1TargetPosition = dbL1OGPosition;
            dbL2TargetPosition = dbL2OGPosition;

            cfFTTargetPosition = cfFTOGPosition;
            cfL1TargetPosition = cfL1OGPosition;
            cfL2TargetPosition = cfL2OGPosition;

            cbBTTargetPosition = cbBTOGPosition;
            cbL1TargetPosition = cbL1OGPosition;
            cbL2TargetPosition = cbL2OGPosition;
        }

        Transform[] frontBones = { dfFrontTorso, cfFrontTorso, cfLeg1, cfLeg2 };
        Vector3[] frontTargetPositions = { dfFTTargetPosition, cfFTTargetPosition, cfL1TargetPosition, cfL2TargetPosition };
        Transform[] backBones = { dbBackTorso, dbLeg1, dbLeg2, cbBackTorso, cbLeg1, cbLeg2 };
        Vector3[] backTargetPositions = { dbBTTargetPosition, dbL1TargetPosition, dbL2TargetPosition, cbBTTargetPosition, cbL1TargetPosition, cbL2TargetPosition };

        float frontSpeed = (playerManager.shouldStretch() ? frontStretchSpeed : destretchSpeed) * Time.deltaTime;
        float backSpeed = (playerManager.shouldStretch() ? backStretchSpeed : destretchSpeed) * Time.deltaTime;
        
        for (int i = 0; i < frontBones.Length; i++)
        {
            frontBones[i].localPosition = Vector3.Lerp(frontBones[i].localPosition, frontTargetPositions[i], frontSpeed);
            backBones[i].localPosition = Vector3.Lerp(backBones[i].localPosition, backTargetPositions[i], backSpeed);
        }
    }
}
