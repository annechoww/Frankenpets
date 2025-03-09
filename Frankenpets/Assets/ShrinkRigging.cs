using UnityEngine;

public class ShirnkRigging : MonoBehaviour
{
    public Transform dogFront; 
    public Transform catFront;
    public Transform dogBack; 
    public Transform catBack;
    public float shrinkScale = 0.7f;
    public float shrinkSpeed = 2.0f; 
    public float expandSpeed = 20.0f;
    public float shrinkDuration = 1.5f;

    private float elapsedTime = 0.0f;

    private Vector3 dogFrontOriginalScale;
    private Vector3 catFrontOriginalScale;
    private Vector3 dogBackOriginalScale;
    private Vector3 catBackOriginalScale;

    private Vector3 dogFrontTargetScale;
    private Vector3 catFrontTargetScale;
    private Vector3 dogBackTargetScale;
    private Vector3 catBackTargetScale;

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
        dogFrontOriginalScale = dogFront.localScale;
        catFrontOriginalScale = catFront.localScale;
        dogBackOriginalScale = dogBack.localScale;
        catBackOriginalScale = catBack.localScale;
    }

    void Update()
    {
        if (playerManager.CheckSwitchInput() && playerManager.getCanSwitch() && elapsedTime < shrinkDuration)
        {
            dogFrontTargetScale = dogFrontOriginalScale * shrinkScale;
            catFrontTargetScale = catFrontOriginalScale * shrinkScale;
            dogBackTargetScale = dogBackOriginalScale * shrinkScale;
            catBackTargetScale = catBackOriginalScale * shrinkScale;

            elapsedTime += Time.deltaTime;
        }
        else
        {
            elapsedTime = 0.0f;

            dogFrontTargetScale = dogFrontOriginalScale;
            catFrontTargetScale = catFrontOriginalScale;
            dogBackTargetScale = dogBackOriginalScale;
            catBackTargetScale = catBackOriginalScale;
        }

        if (elapsedTime >= shrinkDuration)
        {
            dogFront.localScale = dogFrontTargetScale;
            catFront.localScale = catFrontTargetScale;
            dogBack.localScale = dogBackTargetScale;
            catBack.localScale = catBackTargetScale;
        }
        else
        {
            float speed = (playerManager.CheckSwitchInput() ? shrinkScale : expandSpeed);

            dogFront.localScale = Vector3.Lerp(dogFront.localScale, dogFrontTargetScale, Time.deltaTime * speed);
            catFront.localScale = Vector3.Lerp(catFront.localScale, catFrontTargetScale, Time.deltaTime * speed);
            dogBack.localScale = Vector3.Lerp(dogBack.localScale, dogBackTargetScale, Time.deltaTime * speed);
            catBack.localScale = Vector3.Lerp(catBack.localScale, catBackTargetScale, Time.deltaTime * speed);
        }
    }
}
