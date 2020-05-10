using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CannonController : MonoBehaviour
{
    public Transform resetTransform;

    void FixedUpdate()
    {
        Vector3 joyInput = new Vector3(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0);
        Vector3 aimDirToLocalSpace = (transform.right * joyInput.x) + (transform.up * joyInput.y) + transform.forward;
        transform.forward = Vector3.Lerp(resetTransform.transform.forward, aimDirToLocalSpace.normalized, joyInput.magnitude /4);
    }
}
