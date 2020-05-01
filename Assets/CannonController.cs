using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public float cannonRotSpeed = 0.7f;
    Vector3 velocity = Vector3.zero;
    public Transform resetTransform;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 aimDir = new Vector3(x,-y,0);
        Vector3 resetPos = resetTransform.forward;
        x = Mathf.Clamp(x, 0, 5);
        y = Mathf.Clamp(y, 0, 5);
        transform.forward = Vector3.Lerp(aimDir, resetPos, cannonRotSpeed);
    }
}
