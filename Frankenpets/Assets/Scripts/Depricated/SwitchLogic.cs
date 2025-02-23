// using System.Diagnostics;
// using System.Runtime.CompilerServices;
// using UnityEngine;

// public class SwitchLogic : MonoBehaviour
// {
//     private Stopwatch stopwatch = new Stopwatch();
//     public float switchTime = 1.5f;

//     private Player P1;
//     private Player P2;

//     private FixedJoint fixedJoint;
//     public GameObject frontHalf;
//     public GameObject backHalf;


//     [Header("Pet Halves")]
//     public GameObject catFront;
//     public GameObject dogFront;
//     public GameObject catBack;
//     public GameObject dogBack;

//     void Awake()
//     {
//         P1 = GetComponent<PlayerMovement>().P1;
//         P2 = GetComponent<PlayerMovement>().P2;
//     }

//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         UnityEngine.Debug.Log(P1.IsFront);
//         fixedJoint = frontHalf.GetComponent<FixedJoint>();

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         runSwitchLogic();
//     }

//     private void runSwitchLogic()
//     {
//         if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift)) tryStartSwitch();
//         if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) cancelSwitch();

//         tryFinishSwitch();
//     }

//     private void tryStartSwitch()
//     {
//         if (fixedJoint == null)
//         {
//             // not allowed; show text to players saying they must be together 

//         }
//         else
//         {
//             stopwatch.Start();
//         }
//     }

//     private void cancelSwitch()
//     {
//         stopwatch.Reset();
//     }

//     // NOT USING THIS IDEA B/C USING Instantiate() TOO MUCH HAS A LOT OF OVERHEAD (apparently)
//     // private void tryFinishSwitch()
//     // {
//     //     if ((stopwatch.Elapsed.TotalSeconds > switchTime) && (fixedJoint != null))
//     //     {
//     //         stopwatch.Reset();

//     //         // Switch which half the players are controlling
//     //         P1.IsFront = !P1.IsFront;
//     //         P2.IsFront = !P2.IsFront;

//     //         // Switch the half to the player's species (in the Player Class)
//     //         if (P1.Species == "cat")
//     //         {
//     //             if (P1.IsFront)
//     //             {
//     //                 P1.Half = catFront;
//     //                 P2.Half = dogBack;
//     //             }
//     //             else
//     //             {
//     //                 P1.Half = catBack;
//     //                 P2.Half = dogFront;
//     //             }
//     //         }
//     //         else // P2.Species == "cat"
//     //         {
//     //             if (P2.IsFront)
//     //             {
//     //                 P1.Half = dogBack;
//     //                 P2.Half = catFront;
//     //             }
//     //             else 
//     //             {
//     //                 P1.Half = dogFront;
//     //                 P2.Half = catBack;
//     //             }
//     //         }
           
//     //         // Switch the half to the player's species (in Unity)
//     //         Instantiate(); // make sure it's P1's child 
//     //         SetParent();
//     //         Instantiate();
//     //         SetParent();
//     //         Destroy();
//     //         Destroy();

//     //         // Restablish the fixed joint
//     //         PlayerManager set fixed joint 
//     //         fixedJoint = frontHalf.GetComponent<FixedJoint>();

//     //         UnityEngine.Debug.Log("curr fixed joint: ", fixedJoint);

//     //         if (fixedJoint == null) fixedJoint = frontHalf.AddComponent<FixedJoint>();
//     //         fixedJoint.connectedBody = backHalf.GetComponent<Rigidbody>();

//     //         // SplitLogic splitLogicScript = GetComponent<SplitLogic>();
//     //         // splitLogicScript.reconnect();
            
//     //         UnityEngine.Debug.Log("Switched!");
//     //     }
//     // }


// }
