using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;

    float xRotation = 0f;

    void Start()
    {
        // Bloquea el cursor en el centro de la pantalla y lo oculta
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Obtiene la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rota la cámara verticalmente (mirando arriba y abajo)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Aplica la rotación a la cámara
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rota el cuerpo del jugador horizontalmente
        playerBody.Rotate(Vector3.up * mouseX);
    }
}