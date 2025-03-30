using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public float typingSpeed = 0.05f; // Tiempo entre cada letra
    private Coroutine typingCoroutine;
    private bool skipTyping = false; // Controla si se salta el efecto

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
        skipTyping = false; // Reinicia el estado del salto

        foreach (char letter in textToType.ToCharArray())
        {
            if (skipTyping){ // Si el usuario decide saltar
            
                textComponent.text = textToType; // Muestra todo el texto
                break;
            }
            else{

                textComponent.text += letter; // Añade letra por letra

                if(!char.IsWhiteSpace(letter)){
                    SoundManager.Instance.PlaySFX("typewriter");
                }

                yield return new WaitForSeconds(typingSpeed); // Espera antes de la siguiente letra
            }
            
        }

        onComplete?.Invoke(); // Llama al callback si se pasa
        
        /*textComponent.text = ""; // Limpia el texto previo
        foreach (char letter in textToType.ToCharArray())
        {
            textComponent.text += letter; // Añade letra por letra
            yield return new WaitForSeconds(typingSpeed); // Espera antes de la siguiente letra
        }

        onComplete?.Invoke(); // Llama al callback si se pasa*/
    }

    public void SkipTyping() // Método para forzar el salto
    {
        skipTyping = true;
    }
}
