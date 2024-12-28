using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BoundaryRestrictor : MonoBehaviour
{
    private RoomBasedCamera roomCamera;

    private void Start()
    {
        roomCamera = FindObjectOfType<RoomBasedCamera>();
    }

    private void LateUpdate()
    {
        if (roomCamera != null)
        {
            Bounds bounds = roomCamera.GetCameraBounds();
            Vector3 clampedPosition = transform.position;

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, bounds.min.x, bounds.max.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, bounds.min.y, bounds.max.y);

            transform.position = clampedPosition;
        }
    }
}
