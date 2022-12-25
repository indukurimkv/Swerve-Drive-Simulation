using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The important math happens from line 79 to line 100. The rest is more or less boilerplate.
public class WheelController : MonoBehaviour
{
    [SerializeField] private WheelCollider FR;
    [SerializeField] private WheelCollider FL;
    [SerializeField] private WheelCollider BR;
    [SerializeField] private WheelCollider BL;

    [SerializeField] private Vector3 FRpos = new Vector3(1, 1);
    [SerializeField] private Vector3 FLpos = new Vector3(1, -1);
    [SerializeField] private Vector3 BRpos = new Vector3(-1, 1);
    [SerializeField] private Vector3 BLpos = new Vector3(-1, -1);


    [SerializeField] private Transform FLMesh;
    [SerializeField] private Transform FRMesh;
    [SerializeField] private Transform BRMesh;
    [SerializeField] private Transform BLMesh;

    [SerializeField] private float accelForce = 500f;
    [SerializeField] private float breakForce = 100f;


    private Vector3 targetVelocity = new Vector3();
    private Vector3 targetRotation = new Vector3();




    private void FixedUpdate(){
        targetVelocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        targetRotation = new Vector3(0, 0, Input.GetAxis("Rotate"));
        if (targetVelocity.magnitude != 0 || targetRotation.magnitude != 0){
            brakeWheels(0);
            updateWheel();
        }
        else{
            brakeWheels(breakForce);
        }

        positionMesh(FR, FRMesh);
        positionMesh(FL, FLMesh);
        positionMesh(BR, BRMesh);
        positionMesh(BL, BLMesh);
    }
    private void brakeWheels(float breakForce){
        FR.brakeTorque = breakForce;
        FL.brakeTorque = breakForce;
        BR.brakeTorque = breakForce;
        BL.brakeTorque = breakForce;
        FR.motorTorque = 0;
        FL.motorTorque = 0;
        BR.motorTorque = 0;
        BL.motorTorque = 0;
    }
    //Update wheel velocity and angle
    private void updateWheel(){
        
        Vector2 FRV = getWheelVelocity(FRpos);
        Vector2 FLV = getWheelVelocity(FLpos);
        Vector2 BRV = getWheelVelocity(BRpos);
        Vector2 BLV = getWheelVelocity(BLpos);



        FR.motorTorque = FRV.y;
        FL.motorTorque = FLV.y;
        BR.motorTorque = BRV.y;
        BL.motorTorque = BLV.y;

        FR.steerAngle = FRV.x;
        FL.steerAngle = FLV.x;
        BR.steerAngle = BRV.x;
        BL.steerAngle = BLV.x;
    }
    //calculate velocity vectors for each wheel
    private Vector2 getWheelVelocity(Vector3 pos){
        //Wheel velocity vector in rectangluar form
        Vector3 VRect = transformCoordinates() + Vector3.Cross(-targetRotation, pos.normalized);

        //Convert velocity Vector into polar form
        //The 90 makes it so that 0 degrees is forward and not to the right
        //Also note the negative sign on cross.y (if directions get switched, this may be the culprit)
        Vector2 VPol = new Vector2(Mathf.Rad2Deg * Mathf.Atan(-VRect.y/VRect.x) + 90, VRect.magnitude * accelForce);
        //ATan returns principle values, so you have to add 180 if vector points to 2nd or 3rd quadrants
        VPol.x += VRect.x < 0 ? 180: 0;
        //Make sure to not return NaN if VRect.x is 0
        VPol.x = float.IsNaN(VPol.x) ? 0: VPol.x;
        return VPol;
    }
    //Convert controller input fom robot-oriented to field-oriented
    private Vector3 transformCoordinates(){
        float determinant = transform.right.x * transform.forward.z - transform.right.z * transform.forward.x;
        Vector2 right = new Vector2(determinant * transform.forward.z, -determinant * transform.right.z);
        Vector2 forward = new Vector2(-determinant * transform.forward.x, determinant * transform.right.x);
        Vector3 fieldVelocity = new Vector3(targetVelocity.x * right.x + targetVelocity.y*forward.x, targetVelocity.x * right.y + targetVelocity.y * forward.y, 0); 
        return fieldVelocity;
    }

    //Make the wheel mesh position and rotation match the collider position and rotation.
    //This is not important, and it wouldn't change drive characteristics if it weren't present.
    //It's just here to make the simulation look better and simplyify debugging.
    private void positionMesh(WheelCollider col, Transform mesh){
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);

        mesh.position = pos;
        mesh.rotation = rot;

    }
}
