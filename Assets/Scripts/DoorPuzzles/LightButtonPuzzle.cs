using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

public class LightButtonPuzzle : MonoBehaviour, IPuzzle
{
    public int lightButtonNumber = 1;
    public UnityEvent LightButtonClicked;

    public Material onMaterial;
    public Material offMaterial;
    public MeshRenderer buttonRenderer;
    public Light pointLight;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private float pressDepth = -0.05f; // Profundidad de la pulsación en unidades de Unity
    private float pressDuration = 0.3f; // Duración de la animación en segundos
    private bool isAnimating = false;
    private bool isButtonOn;
    private bool isVisionActive = false;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition - (Vector3.left * pressDepth);
        pointLight.intensity = 0f;
        buttonRenderer.material = offMaterial;
        isVisionActive = GameManager.instance.IsVisionActive();
        isButtonOn = false;
    }

    void Update()
    {
        bool currentVisionState = GameManager.instance.IsVisionActive();

        // Si ha habido un cambio en el estado de la visión
        if (currentVisionState != isVisionActive)
        {
            if (!currentVisionState) // Si se desactiva la visión
            {
                buttonRenderer.material = offMaterial;
                pointLight.intensity = 0f;
            }
            else // Si se activa la visión
            {
                if (isButtonOn)
                {
                    buttonRenderer.material = onMaterial;
                    pointLight.intensity = 1f;
                }
            }

            isVisionActive = currentVisionState; // Actualizamos el estado
        }
    }

    public void Interact(InventorySystem inventory)
    {
        if (!isAnimating) //Que no se pueda pulsar si está animándose
        {
            isButtonOn = !isButtonOn;
            if (isVisionActive) 
            {
                if (isButtonOn) { buttonRenderer.material = onMaterial; }
                else { buttonRenderer.material = offMaterial; }
                StartCoroutine(SmoothLightTransition(isButtonOn));
            }
            StartCoroutine(AnimateButtonPress()); //Empezar animación
            LightButtonClicked.Invoke();
        }
    }

    private IEnumerator AnimateButtonPress()
    {
        isAnimating = true;

        // Animación de presionar
        float elapsedTime = 0;
        while (elapsedTime < pressDuration / 2) //Dura la mitad del tiempo
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (pressDuration / 2); //Calculamos un valor entre 0-1 para saber por donde vamos de la animación
            transform.localPosition = Vector3.Lerp(originalPosition, pressedPosition, t); //Interpolación de la animación (t=0 inicio / t=1 final)
            yield return null;
        }

        // Animación de soltar
        elapsedTime = 0;
        while (elapsedTime < pressDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (pressDuration / 2);
            transform.localPosition = Vector3.Lerp(pressedPosition, originalPosition, t);
            yield return null;
        }

        transform.localPosition = originalPosition;
        isAnimating = false;
    }

    private IEnumerator SmoothLightTransition(bool turnOn)
    {
        float duration = 0.3f;
        float elapsedTime = 0;
        float startIntensity = pointLight.intensity;
        float targetIntensity = turnOn ? 1f : 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            pointLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            yield return null;
        }
    }

    public void StopInteract()
    {
        //No necesario para este puzzle
    }
}
