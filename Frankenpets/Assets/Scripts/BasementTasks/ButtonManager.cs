using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    [Header("Button References")]
    public float pressDistance = 0.1f;
    public float pressSpeed = 8.0f;
    private Vector3 originalPosition;
    
    [Header("Tube References")]
    public GameObject leftTube;
    public GameObject rightTube;
    public GameObject brokenLeftTubePrefab;
    public GameObject brokenRightTubePrefab;

    [Header("Explosion References")]
    public float explosionForce = 300f;
    public AudioClip shatterSound;

    [Header("Test Subjects")]
    public Rigidbody catFront;
    public Rigidbody dogBack;
    public Rigidbody dogFront;
    public Rigidbody catBack;

    // Flags
    private bool isPressed = false;
    private bool isColliding = false;
    private bool hasExploded = false;

    [Header("Task Manager")]
    public GameObject taskItem;
    public Task task = new Task("Free the pets", 2);

    [Header("Locate Task Variables")]
    public GameObject taskLight;
    public GameObject taskParticle;
    public GameObject arrow;


    private void Start()
    {
        originalPosition = transform.localPosition;
        TaskManager.RegisterTask(task);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isColliding = true;
        isPressed = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
        isPressed = false;
    }

    private void Update()
    {
        if (isPressed && isColliding)
        {
            // Move button down
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition - new Vector3(0, pressDistance, 0), pressSpeed * Time.deltaTime);
            
            // Only explode once
            if (!hasExploded)
            {
                ExplodeTubes();
                hasExploded = true;
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, pressSpeed * Time.deltaTime);
        }
    }

    void ExplodeTubes()
    {
        if (leftTube != null && rightTube != null)
        {
            Vector3 leftPos = leftTube.transform.position;
            Vector3 rightPos = rightTube.transform.position;
            
            Destroy(leftTube);
            Destroy(rightTube);
            
            GameObject brokenLeft = Instantiate(brokenLeftTubePrefab, leftPos, Quaternion.identity);
            GameObject brokenRight = Instantiate(brokenRightTubePrefab, rightPos, Quaternion.identity);

            if (shatterSound != null)
            {
                AudioManager.Instance.PlaySFX(shatterSound);
            }

            // Add explosion force to each piece
            foreach (Rigidbody rb in brokenLeft.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, brokenLeft.transform.position, 2f);
            }
            
            foreach (Rigidbody rb in brokenRight.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, brokenRight.transform.position, 2f);
            }

            // Unfreeze test subjects
            catFront.constraints = RigidbodyConstraints.None;
            dogBack.constraints = RigidbodyConstraints.None;
            catBack.constraints = RigidbodyConstraints.None;
            dogFront.constraints = RigidbodyConstraints.None;

            // Task complete
            taskItem.transform.GetChild(1).gameObject.SetActive(true);
            FindTasks.Instance.DestroyFindTaskMechanic(arrow, taskParticle, taskLight);
            task.IsComplete = true;
            TaskManager.Instance.CompleteTask();
        }
    }
}