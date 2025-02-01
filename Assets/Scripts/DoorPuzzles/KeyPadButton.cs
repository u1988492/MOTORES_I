using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonScript : MonoBehaviour, IPuzzle
{
    public int keyPadNumber = 1;
    public UnityEvent KeyPadClicked;

    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private float pressDepth = -0.05f; // Profundidad de la pulsaci�n en unidades de Unity
    private float pressDuration = 0.3f; // Duraci�n de la animaci�n en segundos
    private bool isAnimating = false;

    private void Start()
    {
        originalPosition = transform.localPosition;
        pressedPosition = originalPosition - (Vector3.right * pressDepth);
    }

    public void Interact(InventorySystem inventory)
    {
        if (!isAnimating) //Que no se pueda pulsar si est� anim�ndose
        {
            StartCoroutine(AnimateButtonPress()); //Empezar animaci�n
            SoundManager.Instance.PlaySFX("button_press");
            KeyPadClicked.Invoke();
        }
    }

    private IEnumerator AnimateButtonPress()
    {
        isAnimating = true;

        // Animaci�n de presionar
        float elapsedTime = 0;
        while (elapsedTime < pressDuration / 2) //Dura la mitad del tiempo
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (pressDuration / 2); //Calculamos un valor entre 0-1 para saber por donde vamos de la animaci�n
            transform.localPosition = Vector3.Lerp(originalPosition, pressedPosition, t); //Interpolaci�n de la animaci�n (t=0 inicio / t=1 final)
            yield return null;
        }

        // Animaci�n de soltar
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

    public void StopInteract()
    {
        //No necesario para este puzzle
    }
}