using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CannonController : MonoBehaviour
{
    public float cannonRotMod = 0.9f;
    public float horizontalMax = 0.7f;
    public float verticalMax = 0.7f;
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
        x = Mathf.Clamp(x, horizontalMax * -1, horizontalMax);
        y = Mathf.Clamp(y, verticalMax * -1, verticalMax);
        Vector3 aimDir = new Vector3(x,-y,0).normalized + Vector3.forward;
        aimDir = transform.right * aimDir.x + transform.up * aimDir.y + transform.forward * aimDir.z;
        Vector3 resetPos = resetTransform.forward;
        
        transform.forward = Vector3.Lerp(aimDir, resetPos, cannonRotMod);
    }
}
