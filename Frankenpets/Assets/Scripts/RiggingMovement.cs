using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RiggingMovement : MonoBehaviour
{

    public LayerMask terrainLayer;
    public RiggingMovement otherFoot;
    public float stepDistance, stepHeight, stepLength, footSpacing, speed;
    public Transform defaultTargetBody;
    public Transform altTargetBody;
    //public Transform stillBody;
    public Vector3 footOffset;

    Vector3 oldPosition, newPosition, currentPosition;
    Vector3 oldNormal, currentNormal, newNormal;
    float lerp; // lerp = 1 = move; lerp < 1 = can't move 
    private Transform body;

    void Start()
    {
        oldPosition=newPosition=currentPosition=transform.position;
        oldNormal=currentNormal=newNormal=transform.up;
        lerp = 1;
        body = defaultTargetBody;
    }

    public void changeTargetDirection(float direction){

         if (direction >= 0)  //forward
        {
            body = defaultTargetBody;
        }
        else{
            body = altTargetBody;
        }
    }

    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;
        Ray ray =  new Ray(body.position + (body.right * footSpacing), Vector3.down);
        // Ray ray2 =  new Ray(body.position, Vector3.down);
        //Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        // Debug.DrawRay(ray2.origin, ray2.direction * 2f, Color.blue);

        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.isMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
                newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = hit.normal;
            }
        }
        if (lerp < 1)
        {
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

}