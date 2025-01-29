using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform pet;

    // Update is called once per frame
    void Update()
    {
        // need to change
        transform.position = pet.transform.position + new Vector3(0, 0, 0);
    }
}
