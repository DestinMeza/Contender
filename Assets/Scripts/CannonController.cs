using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CannonController : MonoBehaviour
{
    public float cannonRotMod = 0.8f;
    Vector3 velocity = Vector3.zero;
    public Transform resetTransform;
    MeshRenderer cannonMesh;
    void Awake(){
        cannonMesh = GetComponentInParent<MeshRenderer>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 aimDir = new Vector3(x,-y,0).normalized + Vector3.forward;
        aimDir = transform.right * aimDir.x + transform.up * aimDir.y + transform.forward * aimDir.z;
        Vector3 resetPos = resetTransform.forward;
        transform.forward = Vector3.Lerp(aimDir, resetPos, cannonRotMod);
    }
}
