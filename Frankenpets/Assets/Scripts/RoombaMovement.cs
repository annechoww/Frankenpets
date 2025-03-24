using UnityEngine;

public class RoombaMovement : MonoBehaviour
{
    public float speed = 2f;
    public BoxCollider boundary;
    private Vector3 direction;
    private PlayerActions pet;
    private bool draggingPet = false;

    void Start()
    {
        if (boundary == null)
        {
            Debug.LogError("Please assign a BoxCollider as the boundary.");
            return;
        }

        // Initialize a random movement direction on the X-Z plane
        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        if (boundary == null) return;

        transform.position += direction * speed * Time.deltaTime;

        // If dragging, move the pet with the Roomba
        if (draggingPet && pet != null && pet.isPaw)
        {
            pet.transform.position = transform.position; // Keep pet above the Roomba
        }
        else
        {
            draggingPet = false; // Stop dragging if pet.isPaw is false
            pet = null;
        }

        CheckBounds();
    }

    void CheckBounds()
    {
        Bounds bounds = boundary.bounds;
        Vector3 pos = transform.position;

        bool hitEdge = false;

        if (pos.x < bounds.min.x || pos.x > bounds.max.x)
        {
            direction.x = -direction.x;
            hitEdge = true;
        }

        if (pos.z < bounds.min.z || pos.z > bounds.max.z)
        {
            direction.z = -direction.z;
            hitEdge = true;
        }

        if (hitEdge)
        {
            pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
            pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);
            transform.position = pos;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet"))
        {
            pet = other.GetComponent<PlayerActions>();

            if (pet != null && pet.isPaw)
            {
                draggingPet = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pet") && pet != null)
        {
            draggingPet = false;
            pet = null;
        }
    }
}
