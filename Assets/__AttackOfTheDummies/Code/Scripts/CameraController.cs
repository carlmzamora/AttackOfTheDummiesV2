using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3Variable targetPosition;
    public Vector3 offset;

    public void FixedUpdate()
    {
        transform.position = targetPosition + offset;
    }
}
