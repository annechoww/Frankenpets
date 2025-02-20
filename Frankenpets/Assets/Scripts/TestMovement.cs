using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TestMovement : MonoBehaviour
{

    public LayerMask terrainLayer;
    public TestMovement otherFoot;
    public float stepDistance, stepHeight, stepLength, footSpacing, speed;
    public Transform body;
    public Vector3 footOffset;

    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // lerp = 1 = move; lerp < 1 = can't move 

    void Start()
    {
        footSpacing = transform.localPosition.x;
        oldPosition=newPosition=currentPosition=transform.position;
        oldNormal=currentNormal=newNormal=transform.up;
        lerp = 1;
        
    }
    private void Update()
    {

        transform.position = currentPosition;
        transform.up = currentNormal;
        Ray ray =  new Ray(body.position + (body.right * footSpacing), Vector3.down);
        Debug.Log("Current position: " + transform.position);
        //UnityEngine.Debug.DrawLine(transform.position, Vector3.down, Color.green, 2, false);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            UnityEngine.Debug.Log("yes");
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.isMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + body.forward * stepLength * direction + footOffset;
                newNormal = hit.normal;
            }
        }
        if (lerp < 1)
        {
            UnityEngine.Debug.Log("lerp smaller than one");
            Vector3 tempPos = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPosition = tempPos;
            currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
        
    }
    public bool isMoving()
    {
        
        return lerp < 1;
    }



    // public Transform leftFootTarget;
    // public Transform rightFootTarget;
    // public AnimationCurve horizontalCurve;

    // private Vector3 leftTargetOffset;
    // private Vector3 rightTargetOffset;


    // void start()
    // {
    //     leftTargetOffset = leftFootTarget.localPosition;
    //     rightTargetOffset = rightFootTarget.localPosition;

        
    // }
    // void Update()
    // {
    //     UnityEngine.Debug.DrawLine(leftFootTarget.transform.position, leftFootTarget.transform.position+ Vector3.up, Color.red, 2, false);
    //     UnityEngine.Debug.DrawLine(leftFootTarget.transform.position, leftFootTarget.transform.position+ Vector3.forward, Color.blue, 2, false);
    //     UnityEngine.Debug.DrawLine(leftFootTarget.transform.position, leftFootTarget.transform.position+ Vector3.right, Color.green, 2, false);
        
    //     //leftFootTarget.localPosition = leftTargetOffset + this.transform.InverseTransformVector(-leftFootTarget.up) * horizontalCurve.Evaluate(Time.time);
    //     rightFootTarget.localPosition = rightTargetOffset + this.transform.InverseTransformVector(-rightFootTarget.up) * horizontalCurve.Evaluate(Time.time);
    //     leftFootTarget.position = Vector3.forward * horizontalCurve.Evaluate(Time.time);
    //     // rightFootTarget.position = Vector3.forward * horizontalCurve.Evaluate(Time.time);
    // }
}