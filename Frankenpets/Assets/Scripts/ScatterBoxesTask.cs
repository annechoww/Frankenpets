using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScatterBoxesTask : MonoBehaviour
{
    public float forceMagnitude;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")){
            BoxCollisionManager.RegisterCollision(gameObject);
            // Vector3 forceDirection = collision.gameObject.transform.position - transform.position;
            // forceDirection.y = 0;
            // forceDirection.Normalize();
            // collision.gameObject.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
            

        }
    }

}
