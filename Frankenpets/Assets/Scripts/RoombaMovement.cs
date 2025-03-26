using UnityEngine;

public class RoombaMovement : MonoBehaviour
{
    public float speed = 2f;
    public BoxCollider boundary;
    private Vector3 direction;
    
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
}
