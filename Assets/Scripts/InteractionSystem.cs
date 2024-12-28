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
    public TMP_Text zoomPromptText;
    public string interactionPrompt = "'E' to interact";
    public string zoomPrompt = "'E' to exit interaction";

    
    private bool canInteract = false;
    private GameObject currentInteractable; //Guardar el objeto interactuable
    private IPuzzle currentPuzzle;
    private bool isZoomed = false;
    private InventorySystem inventorySystem; //Inventario
    private PauseSystem pauseSystem; 
    private bool isInventoryOpen = false;
    private bool isMenuOpen = false;
    private FirstPersonCamera cameraController; //Controlador para la cámara
    private bool isDialogueActive = false; // Nueva variable para bloquear el movimiento

    void Start()
    {
        inventorySystem = GetComponent<InventorySystem>(); //Asegurarnos que tiene el inventario
        pauseSystem = GetComponent<PauseSystem>();
        if (inventorySystem == null)
        {
            inventorySystem = gameObject.AddComponent<InventorySystem>();
        }
        cameraController = mainCamera.GetComponent<FirstPersonCamera>();
    }

    void Update()
    {
        if (isDialogueActive) return; // Si el diálogo está activo, bloquea todas las acciones
        
        if (Input.GetKeyDown(KeyCode.Escape)) // Presionar 'ESC' para ver el Menú de pausa
        {
            DisplayMenu();
        }
        else if(!isMenuOpen){
            if (Input.GetKeyDown(KeyCode.I))  // Presionar 'I' para ver el inventario
            {
                DisplayInventory();
            }
            if (!isZoomed) //Si no está dentro de un puzzle, busca cosas interactuables
            {
                CheckForInteractables();
            }
            else //Sino, verificamos dentro del puzzle clicks del ratón
            {
                HandleZoomedInteraction();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ExitZoom();
                }
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
                InteractableObject interactable = hit.collider.GetComponentInParent<InteractableObject>(); //Verificamos si el padre del objeto tiene colisión
                if (interactable != null)
                {
                    // Si tiene zona de interacci�n espec�fica, comprueba si golpe� esa zona (ya sea el objeto o su padre)
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

            // Si podemos interactuar, muestra el prompt y maneja la interacci�n
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
        interactionText.text = interactionPrompt; //mostramos texto
        interactionText.gameObject.SetActive(true);

        if (Input.GetKeyDown(KeyCode.E)) //Si se pulsa la E: 
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
                IUsable usableObject = hitCollider.GetComponent<IUsable>(); //usamos una interfaz 
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
        mainCamera.gameObject.SetActive(false); //Desactivamos cámara

        InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();

        if (interactableScript.interactionZone != null)
        {
            interactableScript.interactionZone.enabled = false; //Desactivamos zona de interacción
        }

        int zoomCameraIndex = interactableScript.zoomCameraIndex;
        zoomCameras[zoomCameraIndex].gameObject.SetActive(true); //activamos la cámara del puzzle

        // Activa el cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Desactiva el texto de interacción
        interactionText.gameObject.SetActive(false);

        // Desactiva el movimiento del jugador
        GetComponent<PlayerMovement>().enabled = false;

        // Muestra el texto del prompt de zoom
        if (zoomPromptText != null)
        {
            zoomPromptText.text = zoomPrompt;
            zoomPromptText.gameObject.SetActive(true);
        }
    }

    void HandleZoomedInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo del rat�n
        {
            //Debug.Log("Pulsé");
            Ray ray = zoomCameras[GetActiveZoomCameraIndex()].ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                currentPuzzle = hit.collider.GetComponent<IPuzzle>();
                if (currentPuzzle != null)
                {
                    currentPuzzle.Interact(inventorySystem); //Si se detecta la interfaz de puzle y que da click, se interactua con el puzzle
                }
            }
        }
    }

    void Recolect()
    {
        RecolectableObject recolectableObject = currentInteractable.GetComponent<RecolectableObject>();
        string nameObject = recolectableObject.nameObject; //Conseguimos nombre del objeto

        inventorySystem.AddItem(nameObject); //lo añadimos

        interactionText.gameObject.SetActive(false); //lo desactivamos

        recolectableObject.Recolect();
    }

    void Use(IUsable usableObject)
    {
        usableObject.Usar(inventorySystem); //Interfaz para usar el objeto
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

        if (currentPuzzle != null)
        {
            currentPuzzle.StopInteract();
        }

        if (currentInteractable != null)
        {
            InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();
            if (interactableScript.interactionZone != null)
            {
                interactableScript.interactionZone.enabled = true;
            }
        }

        // Reactiva la c�mara principal
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

        // Oculta el texto del prompt de zoom
        if (zoomPromptText != null)
        {
            zoomPromptText.gameObject.SetActive(false);
        }
    }

    void ResetInteraction()
    {
        canInteract = false;
        currentInteractable = null;
        interactionText.gameObject.SetActive(false);
    }

    void ActivePlayer()
    {
        GetComponent<PlayerMovement>().enabled = true;
        cameraController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void DesactivePlayer()
    {
        GetComponent<PlayerMovement>().enabled = false;
        cameraController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void DisplayInventory()
    {

        isInventoryOpen = !isInventoryOpen;

        if (!isInventoryOpen)
        {
            ActivePlayer();
        } 
        else
        {
            DesactivePlayer();
        }
        
        inventorySystem.ToggleVisibilityInventory();
    }

     public void DisplayMenu() //Público para poder llamarlo desde el menú de Pausa
    {
        isMenuOpen = !isMenuOpen;
        if (!isMenuOpen)
        {
            ActivePlayer();
        }
        else
        {
            DesactivePlayer();
        }

        pauseSystem.ToggleVisibilityMenu();
    }

        public void OnDialogue()
    {
        isDialogueActive = !isDialogueActive;

        if (!isDialogueActive)
        {
            GetComponent<PlayerMovement>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } 
        else
        {
            GetComponent<PlayerMovement>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
