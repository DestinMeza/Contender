using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CannonController : MonoBehaviour
{
    public Transform resetTransform;

    void FixedUpdate()
    {
        Vector3 aimDir = new Vector3(Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0);
        aimDir = (resetTransform.right * aimDir.x) + (resetTransform.up * aimDir.y) + resetTransform.forward;
        transform.forward = Vector3.Lerp(resetTransform.localPosition.normalized, aimDir.normalized, aimDir.normalized.magnitude * Time.fixedDeltaTime);
    }
}
