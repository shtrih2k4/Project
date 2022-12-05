using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
   
    public float rotateSpeed = 10f;
    private Transform camera_transform;
    private void Awake()
    {
        camera_transform = GetComponent<Transform>();
    }
    
    void Update()
    {
        camera_transform.Rotate(new Vector3(0, rotateSpeed * Time.deltaTime, 0));
    }
}
