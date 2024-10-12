using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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
    private InventorySystem inventorySystem;

    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>();
        if (inventorySystem == null)
        {
            inventorySystem = gameObject.AddComponent<InventorySystem>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))  // Presionar 'I' para ver el inventario
        {
            DisplayInventory();
        }
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
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("Recolectable"))
            {
                canInteract = true;
                currentInteractable = hit.collider.gameObject;
                interactionText.text = interactionPrompt;
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.collider.CompareTag("Interactable"))
                    {
                        Interact();
                    }
                    else if (hit.collider.CompareTag("Recolectable"))
                    {
                        Recolect();
                    }
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
        isZoomed = true;
        mainCamera.gameObject.SetActive(false);

        InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();

        int zoomCameraIndex = interactableScript.zoomCameraIndex;
        zoomCameras[zoomCameraIndex].gameObject.SetActive(true);

        // Activa el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Desactiva el texto de interacción
        interactionText.gameObject.SetActive(false);

        // Desactiva el movimiento del jugador
        GetComponent<PlayerMovement>().enabled = false;
    }

    void Recolect()
    {
        RecolectableObject recolectableObject = currentInteractable.GetComponent<RecolectableObject>();
        string nameObject = recolectableObject.nameObject;

        inventorySystem.AddItem(nameObject);

        interactionText.gameObject.SetActive(false);

        recolectableObject.Recolect();
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

    void DisplayInventory()
    {
        inventorySystem.DisplayInventory();
    }
}
