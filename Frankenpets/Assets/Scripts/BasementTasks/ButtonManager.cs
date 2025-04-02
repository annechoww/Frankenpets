using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public float pressDistance = 0.1f;
    public float pressSpeed = 8.0f;
    private Vector3 originalPosition;
    private bool isPressed = false;
    private bool isColliding = false;
    private bool hasExploded = false; // Add this flag

    // References to the intact tubes
    public GameObject leftTube;
    public GameObject rightTube;
    
    // References to the broken tube prefabs
    public GameObject brokenLeftTubePrefab;
    public GameObject brokenRightTubePrefab;
    
    // Explosion force
    public float explosionForce = 300f;

    private void Start()
    {
        originalPosition = transform.localPosition;
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
            // Move button up
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
            
            // Add explosion force to each piece
            foreach (Rigidbody rb in brokenLeft.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, brokenLeft.transform.position, 2f);
            }
            
            foreach (Rigidbody rb in brokenRight.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(explosionForce, brokenRight.transform.position, 2f);
            }
        }
    }
}