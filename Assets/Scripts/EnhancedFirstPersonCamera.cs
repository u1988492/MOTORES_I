using UnityEngine;
using System.Collections;

public class EnhancedFirstPersonCamera : MonoBehaviour
{
    public static float mouseSensitivity = 100f;
    public Transform playerBody;
    public float transitionDuration = 1.0f;

    private float xRotation = 0f;
    private bool isTransitioning = false;
    private Camera thisCamera;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        thisCamera = GetComponent<Camera>();
        SaveOriginalTransform();
    }

    void SaveOriginalTransform()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    void Update()
    {
        if (!isTransitioning && thisCamera.enabled)
        {
            HandleMouseLook();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void TransitionToTarget(Camera targetCamera)
    {
        if (!isTransitioning)
        {
            StartCoroutine(PerformTransition(targetCamera.transform.position, targetCamera.transform.rotation));
        }
    }

    public void TransitionBack()
    {
        if (!isTransitioning)
        {
            Vector3 targetPosition = playerBody.TransformPoint(originalPosition);
            Quaternion targetRotation = playerBody.rotation * originalRotation;
            StartCoroutine(PerformTransition(targetPosition, targetRotation));
        }
    }

    private IEnumerator PerformTransition(Vector3 targetPosition, Quaternion targetRotation)
    {
        isTransitioning = true;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // Usar una curva de suavizado para hacer la transición más natural
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        isTransitioning = false;
    }
}