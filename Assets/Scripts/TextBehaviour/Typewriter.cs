using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f; // Tiempo entre cada letra
    private Coroutine typingCoroutine;

    public void StartTyping(string textToType, TextMeshProUGUI textComponent, System.Action onComplete = null)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(textToType, textComponent, onComplete));
    }

    private IEnumerator TypeText(string textToType, TextMeshProUGUI textComponent, System.Action onComplete)
    {
        textComponent.text = ""; // Limpia el texto previo
        foreach (char letter in textToType.ToCharArray())
        {
            textComponent.text += letter; // Añade letra por letra
            yield return new WaitForSeconds(typingSpeed); // Espera antes de la siguiente letra
        }

        onComplete?.Invoke(); // Llama al callback si se pasa
    }
}
