using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CameraFocusManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera;
    public float moveSpeed;
    public float rotateSpeed;

    [Header("Click Settings")]
    public string exitTag;
    public Vector3 fixedOffset; // Defined in Inspector

    [Header("Free Look Settings")]
    public float lookSpeed;
    public Transform orbitTarget;

    private Vector3 mainCameraPosition;
    private Quaternion mainCameraRotation;
    private bool isMoving = false;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        mainCameraPosition = mainCamera.transform.position;
        mainCameraRotation = mainCamera.transform.rotation;

        Vector3 angles = mainCamera.transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        HandleClick();
        HandleFreeLook();
    }

    private void HandleClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!isMoving)
                {
                    if (hit.collider.CompareTag(exitTag))
                    {
                        StartCoroutine(MoveCamera(mainCameraPosition, mainCameraRotation));
                    }
                    else
                    {
                        Vector3 targetPos = hit.collider.transform.position + fixedOffset;
                        Quaternion targetRot = Quaternion.LookRotation(hit.collider.transform.position - targetPos);
                        StartCoroutine(MoveCamera(targetPos, targetRot));
                    }
                }
            }
        }
    }

    private void HandleFreeLook()
    {
        if (!isMoving)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();

            yaw += delta.x * lookSpeed;
            pitch -= delta.y * lookSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            if (orbitTarget != null)
            {
                // Orbit around a target
                mainCamera.transform.RotateAround(orbitTarget.position, Vector3.up, delta.x * lookSpeed);
                mainCamera.transform.RotateAround(orbitTarget.position, mainCamera.transform.right, -delta.y * lookSpeed);
            }
            else
            {
                // Smoothly apply rotation
                Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
                mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRotation, Time.deltaTime * 15f);
            }
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        isMoving = true;

        while (Vector3.Distance(mainCamera.transform.position, targetPos) > 0.01f ||
               Quaternion.Angle(mainCamera.transform.rotation, targetRot) > 0.1f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, moveSpeed * Time.deltaTime);
            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            yield return null;
        }

        mainCamera.transform.position = targetPos;
        mainCamera.transform.rotation = targetRot;

        Vector3 angles = mainCamera.transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        isMoving = false;
    }
}
