using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionSystem : MonoBehaviour
{
    public Camera mainCamera;
    public Camera[] zoomCameras;
    public float interactionDistance = 5f;
    public TMP_Text interactionText;
    public string interactionPrompt = "Pulsa 'E' para interactuar";

    private bool canInteract = false;
    private GameObject currentInteractable;
    private bool isZoomed = false;

    void Update()
    {
        if (!isZoomed)
        {
            CheckForInteractables();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitZoom();
        }
    }

    void CheckForInteractables()
    {
        Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * interactionDistance, Color.red); //Debug

        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("Se detectó un objeto");
                canInteract = true;
                currentInteractable = hit.collider.gameObject;
                interactionText.text = interactionPrompt;
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Pulsó E");
                    Interact();
                }
            }
            else
            {
                ResetInteraction();
            }
        }
        else
        {
            ResetInteraction();
        }
    }

    void Interact()
    {
        Debug.Log("Método Interact() llamado");

        if (currentInteractable == null)
        {
            Debug.LogError("currentInteractable es null");
            return;
        }

        isZoomed = true;
        Debug.Log("Iniciando interacción en modo zoom");

        if (mainCamera == null)
        {
            Debug.LogError("mainCamera es null");
            return;
        }

        mainCamera.gameObject.SetActive(false);
        Debug.Log("Cámara principal desactivada");

        InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();
        if (interactableScript == null)
        {
            Debug.LogError("El objeto interactuable no tiene el componente InteractableObject");
            return;
        }

        int zoomCameraIndex = interactableScript.zoomCameraIndex;
        Debug.Log($"Índice de cámara de zoom del objeto: {zoomCameraIndex}");

        if (zoomCameras == null)
        {
            Debug.LogError("El array zoomCameras es null");
            return;
        }

        if (zoomCameraIndex >= 0 && zoomCameraIndex < zoomCameras.Length)
        {
            if (zoomCameras[zoomCameraIndex] == null)
            {
                Debug.LogError($"La cámara en el índice {zoomCameraIndex} es null");
                return;
            }
            zoomCameras[zoomCameraIndex].gameObject.SetActive(true);
            Debug.Log($"Cámara de zoom {zoomCameraIndex} activada: {zoomCameras[zoomCameraIndex].name}");
        }
        else
        {
            Debug.LogError($"Índice de cámara de zoom inválido: {zoomCameraIndex}. El array de cámaras tiene {zoomCameras.Length} elementos.");
            return;
        }

        // Activa la cámara de zoom correspondiente
        //int zoomCameraIndex = currentInteractable.GetComponent<InteractableObject>().zoomCameraIndex;
        //Debug.Log("La cámara que tiene el índice: " + zoomCameraIndex);
        //zoomCameras[zoomCameraIndex].gameObject.SetActive(true);

        // Activa el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Desactiva el texto de interacción
        interactionText.gameObject.SetActive(false);

        // Desactiva el movimiento del jugador
        GetComponent<PlayerMovement>().enabled = false;
    }

    void ExitZoom()
    {
        isZoomed = false;

        // Reactiva la cámara principal
        mainCamera.gameObject.SetActive(true);

        // Desactiva todas las cámaras de zoom
        foreach (Camera cam in zoomCameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Oculta y bloquea el cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Reactiva el movimiento del jugador
        GetComponent<PlayerMovement>().enabled = true;
    }

    void ResetInteraction()
    {
        canInteract = false;
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }
}
