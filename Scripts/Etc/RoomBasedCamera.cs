using UnityEngine;
using System.Collections;

public class RoomBasedCamera : MonoBehaviour
{
    public Camera mainCamera;
    public float moveDuration = 0.2f;
    private bool isMoving = false;
    public bool IsMoving { get; private set; } = false;

    private IEnumerator SmoothMoveCamera(Vector3 start, Vector3 end)
    {
        IsMoving = true;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(start, end, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = end;
        IsMoving = false;
    }



    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    public void MoveToPosition(Vector3 targetPosition)
    {
        if (isMoving || mainCamera == null) return;

        Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, mainCamera.transform.position.z);
        StartCoroutine(SmoothMoveCamera(mainCamera.transform.position, newPosition));
    }

    




    public Bounds GetCameraBounds()
    {
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        return new Bounds(mainCamera.transform.position, new Vector3(cameraWidth, cameraHeight, 0));
    }

}
