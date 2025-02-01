using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Burst.CompilerServices;

public class InteractionSystem : MonoBehaviour
{   
    public Camera mainCamera;
    public GameObject crosshair;
    public float interactionDistance = 5f;
    public TMP_Text interactionText;
    public TMP_Text zoomPromptText;
    public string interactionPrompt = "'E' to interact";
    public string zoomPrompt = "'E' to exit interaction";

    [HideInInspector]
    public EnhancedFirstPersonCamera cameraController; // Hacemos público el controlador de cámara

    [SerializeField]
    public GameObject normalPostProcess;
    public GameObject guardianPostProcess;

    private bool canInteract = true;
    private GameObject currentInteractable; //Guardar el objeto interactuable
    private IPuzzle currentPuzzle;
    private bool isZoomed = false;
    private InventorySystem inventorySystem; //Inventario
    private PauseSystem pauseSystem;
    private bool isInventoryOpen = false;
    private bool isMenuOpen = false;
    //private FirstPersonCamera cameraController; //Controlador para la cámara
    private bool isDialogueActive = false; // Nueva variable para bloquear el movimiento
    private bool isCameraChanging = false;

    //private EnhancedFirstPersonCamera cameraController;
    private Camera currentPuzzleCamera;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            Debug.LogWarning("Main Camera no asignada, usando Camera.main");
        }

        ActivateCameraController();

        inventorySystem = GetComponent<InventorySystem>(); //Asegurarnos que tiene el inventario
        pauseSystem = GetComponent<PauseSystem>();
        if (inventorySystem == null)
        {
            inventorySystem = gameObject.AddComponent<InventorySystem>();
        }
    }

    public void ActivateCameraController() //Lo hacemos función porque se carga antes el diálogo que el start y para que no pete en el introDialogue
    {
        cameraController = mainCamera?.GetComponent<EnhancedFirstPersonCamera>();
        if (cameraController == null && mainCamera != null)
        {
            cameraController = mainCamera.gameObject.AddComponent<EnhancedFirstPersonCamera>();
            Debug.LogWarning("EnhancedFirstPersonCamera no encontrado, añadido automáticamente");
        }
        Debug.Log(cameraController);
    }

    void Update()
    {
        if (isDialogueActive) return; // Si el diálogo está activo, bloquea todas las acciones

        if (Input.GetKeyDown(KeyCode.Escape)) // Presionar 'ESC' para ver el Menú de pausa
        {
            DisplayMenu();
        }
        else if (!isMenuOpen)
        {
            if (Input.GetKeyDown(KeyCode.I))  // Presionar 'I' para ver el inventario
            {
                DisplayInventory();
            }
            if (Input.GetKeyDown(KeyCode.V) && GameManager.instance.IsVisionUnlocked())
            {
                ChangePostProcess();
            }
            if (!isZoomed && !isCameraChanging) //Si no está dentro de un puzzle, busca cosas interactuables
            {
                CheckForInteractables();
            }
            else if (!isCameraChanging)//Sino, verificamos dentro del puzzle clicks del ratón
            {
                HandleZoomedInteraction();
                if (Input.GetKeyDown(KeyCode.E) && !isCameraChanging)
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
            if (hit.collider != null && hit.collider.CompareTag("Interactable"))
            {
                // Primero verificamos si el objeto tiene el componente InteractableObject directamente
                InteractableObject interactable = hit.collider.GetComponent<InteractableObject>();

                // Si no lo tiene directamente, buscamos en el padre
                if (interactable == null)
                {
                    interactable = hit.collider.GetComponentInParent<InteractableObject>();
                }

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
            else if (hit.collider != null && (hit.collider.CompareTag("Recolectable") || hit.collider.CompareTag("Usable")))
            {
                shouldInteract = true;
                currentInteractable = hit.collider.gameObject;
            }

            // Si podemos interactuar, muestra el prompt y maneja la interacción
            if (shouldInteract)
            {
                if (currentInteractable.CompareTag("Recolectable")) currentInteractable.GetComponent<RecolectableObject>().toEmissive();
                else if (currentInteractable.CompareTag("Interactable")) currentInteractable.GetComponent<InteractableObject>().toEmissive();
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
        canInteract = false;
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
        canInteract = true;

    }

    void Interact()
    {
        if (currentInteractable == null)
        {
            Debug.LogError("No hay objeto interactuable");
            return;
        }

        InteractableObject interactableScript = currentInteractable.GetComponent<InteractableObject>();
        if (interactableScript == null)
        {
            Debug.LogError("El objeto no tiene componente InteractableObject");
            return;
        }

        if (interactableScript.Camera == null)
        {
            Debug.LogError("La cámara del objeto interactuable no está asignada");
            return;
        }

        isZoomed = true;
        currentPuzzleCamera = interactableScript.Camera;

        if (interactableScript.interactionZone != null)
        {
            interactableScript.interactionZone.enabled = false;
        }

        // Aseguramos que el cameraController existe
        if (cameraController != null)
        {
            cameraController.TransitionToTarget(currentPuzzleCamera);
            isCameraChanging = true;
            StartCoroutine(ActivateZoomCameraAfterTransition());
        }
        else
        {
            Debug.LogError("No se encontró el EnhancedFirstPersonCamera");
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        interactionText.gameObject.SetActive(false);
        GetComponent<PlayerMovement>().enabled = false;

        if (zoomPromptText != null)
        {
            zoomPromptText.text = zoomPrompt;
            zoomPromptText.gameObject.SetActive(true);
        }
    }
    private IEnumerator ActivateZoomCameraAfterTransition()
    {
        yield return new WaitForSeconds(cameraController.transitionDuration);
        mainCamera.gameObject.SetActive(false);
        crosshair.SetActive(false);
        currentPuzzleCamera.gameObject.SetActive(true);
        isCameraChanging = false;
    }

    void HandleZoomedInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = currentPuzzleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                currentPuzzle = hit.collider.GetComponent<IPuzzle>();
                if (currentPuzzle != null)
                {
                    currentPuzzle.Interact(inventorySystem);
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
    public void ExitZoom()
    {
        isZoomed = false;
        isCameraChanging = true;

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

        // Reactiva la cámara principal y hace la transición de vuelta

        mainCamera.gameObject.SetActive(true);
        crosshair.SetActive(true);
        if (currentPuzzleCamera != null)
        {
            currentPuzzleCamera.gameObject.SetActive(false);
            currentPuzzleCamera = null;
        }
        cameraController.TransitionBack();

        StartCoroutine(DeactivatePuzzleCameraAfterTransition());

        // Oculta y bloquea el cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Oculta el texto del prompt de zoom
        if (zoomPromptText != null)
        {
            zoomPromptText.gameObject.SetActive(false);
        }

    }

    private IEnumerator DeactivatePuzzleCameraAfterTransition()
    {
        yield return new WaitForSeconds(cameraController.transitionDuration);
        if (currentPuzzleCamera != null)
        {
            currentPuzzleCamera.gameObject.SetActive(false);
            currentPuzzleCamera = null;
        }
        isCameraChanging = false;
        // Reactiva el movimiento del jugador
        GetComponent<PlayerMovement>().enabled = true;
        mainCamera.gameObject.SetActive(true);
    }

    void ResetInteraction()
    {
        canInteract = false;
        if (currentInteractable != null)
        {
            if (currentInteractable.CompareTag("Recolectable")) currentInteractable.GetComponent<RecolectableObject>().toNormal();
            else if (currentInteractable.CompareTag("Interactable")) currentInteractable.GetComponent<InteractableObject>().toNormal();
        }
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
        if (!pauseSystem.isESCPressed())
        {
            ActivePlayer();
        }
        else
        {
            DesactivePlayer();

        }

        //pauseSystem.isESCPressed(ref isMenuOpen);
    }

    public void OnDialogue()
    {
        isDialogueActive = !isDialogueActive;

        if (!isDialogueActive)
        {
            ActivePlayer();
            crosshair.SetActive(true);
        }
        else
        {
            DesactivePlayer();
            crosshair.SetActive(false);
        }
    }

    void ChangePostProcess()
    {
        if (GameManager.instance.IsVisionActive()) //Si está activa, desactivarla
        {
            normalPostProcess.SetActive(true);
            guardianPostProcess.SetActive(false);
            GameManager.instance.DesactiveVision();
        }
        else
        {
            guardianPostProcess.SetActive(true);
            normalPostProcess.SetActive(false);
            GameManager.instance.ActiveVision();
        }
    }
}
