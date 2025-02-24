using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ScatterBoxesTask : MonoBehaviour
{
    // public float forceMagnitude;


    void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("cat front") || collision.gameObject.CompareTag("dog front")) && gameObject.layer == LayerMask.NameToLayer("Front Task")){
            BoxCollisionManager.RegisterCollision(gameObject, "front");
            //HalvesInstruction.hideInstruction();
            // Debug.Log($"cat front collided.");
            // Debug.Log($"layer: {gameObject.layer}");
            // Vector3 forceDirection = collision.gameObject.transform.position - transform.position;
            // forceDirection.y = 0;
            // forceDirection.Normalize();
            // collision.gameObject.AddForceAtPosition(forceDirection * forceMagnitude, transform.position, ForceMode.Impulse);
        } else if ((collision.gameObject.CompareTag("dog back") || collision.gameObject.CompareTag("cat back")) && gameObject.layer == LayerMask.NameToLayer("Back Task")){
            // Debug.Log($"cat back collided.");
            // Debug.Log($"layer: {gameObject.layer}");
            BoxCollisionManager.RegisterCollision(gameObject, "back");
            
        }
    }

}
