using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleTurretController : MonoBehaviour
{
    public Transform firePos1;
    public Transform firePos2;
    public Transform turretHead;
    public Transform target;
    public float rotationalDamp = 1;
    public float maxFiringDis = 200;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 diff = target.position - turretHead.position;
        Quaternion rotation = Quaternion.LookRotation(diff);
        rotation.x = 0;
        rotation.z = 0;
        turretHead.rotation = Quaternion.Slerp(turretHead.rotation, rotation, rotationalDamp * Time.deltaTime);
        if(diff.magnitude < maxFiringDis){

        }
    }
}
