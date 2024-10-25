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
        else
        {
            HandleZoomedInteraction();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitZoom();
            }
        }
    }

    void CheckForInteractables()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, interactionDistance))
        {
            bool shouldInteract = false;

            // Comprueba si es un objeto interactuable y si golpea la zona correcta
            if (hit.collider.CompareTag("Interactable"))
            {
                InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>();
                if (interactable != null)
                {
                    // Si tiene zona de interacción específica, comprueba si golpeó esa zona
                    if (interactable.interactionZone == null || hit.collider == interactable.interactionZone)
                    {
                        shouldInteract = true;
                        currentInteractable = interactable.gameObject;
                    }
                }
            }
            // Para recolectables y usables mantiene el comportamiento original
            else if (hit.collider.CompareTag("Recolectable") || hit.collider.CompareTag("Usable"))
            {
                shouldInteract = true;
                currentInteractable = hit.collider.gameObject;
            }

            // Si podemos interactuar, muestra el prompt y maneja la interacción
            if (shouldInteract)
            {
                HandleInteraction(hit.collider);
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

    void HandleInteraction(Collider hitCollider)
    {
        canInteract = true;
        interactionText.text = interactionPrompt;
        interactionText.gameObject.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hitCollider.CompareTag("Interactable"))
            {
                Interact();
            }
            else if (hitCollider.CompareTag("Recolectable"))
            {
                Recolect();
            }
            else if (hitCollider.CompareTag("Usable"))
            {
                IUsable usableObject = hitCollider.GetComponent<IUsable>();
                if (usableObject != null)
                {
                    Use(usableObject);
                }
            }
        }
    }

    void Interact()
    {
        isZoomed = true;
        mainCamera.gameObject.SetActive(false);

        InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();

        if (interactableScript.interactionZone != null)
        {
            interactableScript.interactionZone.enabled = false;
        }

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

    void HandleZoomedInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo del ratón
        {
            Ray ray = zoomCameras[GetActiveZoomCameraIndex()].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                IPuzzle puzzle = hit.collider.GetComponent<IPuzzle>();
                if (puzzle != null)
                {
                    puzzle.Interact(inventorySystem);
                }
            }
        }
    }

    void Recolect()
    {
        RecolectableObject recolectableObject = currentInteractable.GetComponent<RecolectableObject>();
        string nameObject = recolectableObject.nameObject;

        inventorySystem.AddItem(nameObject);

        interactionText.gameObject.SetActive(false);

        recolectableObject.Recolect();
    }

    void Use(IUsable usableObject)
    {
        usableObject.Usar(inventorySystem);
    }

    int GetActiveZoomCameraIndex()
    {
        for (int i = 0; i < zoomCameras.Length; i++)
        {
            if (zoomCameras[i].gameObject.activeInHierarchy)
            {
                return i;
            }
        }
        return -1;
    }

    public void ExitZoom()
    {
        isZoomed = false;

        if (currentInteractable != null)
        {
            InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();
            if (interactableScript.interactionZone != null)
            {
                interactableScript.interactionZone.enabled = true;
            }
        }

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
