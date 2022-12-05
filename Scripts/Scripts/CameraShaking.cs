using UnityEngine;

public  class CameraShaking : MonoBehaviour
{
    private Transform camera_transform;
    private float shaking_duration = 1f;
    private readonly float shaking_amount=0.06f, decreaseFactor = 1.5f;
    private Vector3 origCameraPosition;
    private void Start()
    {
        camera_transform = GetComponent<Transform>();
        origCameraPosition = camera_transform.localPosition;

    }
    public void Update()
    {
        if (shaking_duration > 0)
        {
            camera_transform.localPosition = origCameraPosition + Random.insideUnitSphere * shaking_amount;
            shaking_duration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shaking_duration = 0;
            camera_transform.localPosition = origCameraPosition;
        }

    }

}
