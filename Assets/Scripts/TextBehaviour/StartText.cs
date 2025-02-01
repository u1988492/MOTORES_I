using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage; // Fondo
    public TextMeshProUGUI dialogueText; // Texto para mostrar
    public TextMeshProUGUI promptText; // Texto informativo
    public float canvasFadeDuration = 1f; // Duración de la transición
    public float textDelay = 1f; // Tiempo de espera del  texto

    public string[] dialogues; // Array de diálogos
    private int currentDialogueIndex = 0; // Índice para controlar el diálogo actual
    private bool isWaitingForInput = false; // Para controlar los clics del jugador y saltar las líneas de diálogo de una en una

    private TypewriterEffect typewriterEffect; // Referencia al efecto de máquina de escribir
    public float typingSpeed = 0.1f; // Velocidad de la máquina de escribir
    public bool hasFinished = false; // Variable para controlar si se ha acabado la cinemática

    public TutorialManager tutorial;

    private InteractionSystem interactionSystem;

    private void Awake() // Añade el script y configura la velocidad de la máquina de escribir
    {
        interactionSystem = FindObjectOfType<InteractionSystem>(); // Busca el script InteractionSystem en la escena
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>(); 
        typewriterEffect.typingSpeed = typingSpeed;
    }

    private void Start()
    {
        dialogues = new string[] // Definición de los diálogos
        {
            "Listen to me - do not come out now, it’s dangerous outside.",
            "You have to stay here until you are ready.",
            "Don’t look at me like that, please. \nI will be okay, I promise.",
            "Once you are ready to come out, you will know.",
            "Trust me.",
            "Here, take this. \nIt will protect you from all evil.",
            "Now go, I will be waiting here for you.",
            "Stay safe.",
            ""
        };

        StartCoroutine(ShowIntro()); // Empieza la intro
    }

    private IEnumerator ShowIntro()
    {
        interactionSystem.ActivateCameraController(); 
        interactionSystem.OnDialogue(); // Bloquea el movimiento del jugador al iniciar el diálogo

        // Se muestra el canvas al inicio y espera un poco antes de mostrar el texto
        fadeImage.canvasRenderer.SetAlpha(1f);
        yield return new WaitForSeconds(textDelay);

        // Comienza la primera línea de diálogo
        yield return ShowDialogue(dialogues[currentDialogueIndex]);
        
        // Cambia el diálogo tras un clic o tecla
        while (currentDialogueIndex < dialogues.Length - 1)
        {
            isWaitingForInput = true; // Esperamos la entrada del jugador
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));

            if (isWaitingForInput) // Si hemos recibido una entrada, cambiamos al siguiente diálogo
            {
                currentDialogueIndex++;
                yield return ShowDialogue(dialogues[currentDialogueIndex]);
                isWaitingForInput = false;
            }
        }

        hasFinished = true;

        // Espera a que el texto haya desaparecido y empieza el fade out del canvas
        yield return new WaitForSeconds(textDelay); 
        fadeImage.CrossFadeAlpha(0f, canvasFadeDuration, false);
        promptText.canvasRenderer.SetAlpha(0f);

        // Espera que el canvas haya hecho el fade out y lo destruye
        yield return new WaitForSeconds(canvasFadeDuration); 
        Destroy(fadeImage.transform.root.gameObject);

        tutorial.StartTutorial();
        interactionSystem.OnDialogue(); // Bloquea el movimiento del jugador al iniciar el diálogo
    }

    private IEnumerator ShowDialogue(string dialogue) //Aplicación del efecto de máquina de escibrir en la línea
    {
        bool dialogueFinished = false;
        bool skipEffectTriggered = false; // Flag para ver si el usuario hace skip del texto o no

        typewriterEffect.StartTyping(dialogue, dialogueText, () => dialogueFinished = true); // Espera a que termine de escribir el texto o el usuario lo salte
        
        while (!dialogueFinished){
            if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && !skipEffectTriggered){

                typewriterEffect.SkipTyping(); // Salta el efecto
            }

            yield return null; // Espera hasta el siguiente frame
        }
                
        skipEffectTriggered = false; // Reset del skip flag

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));// Espera a que termine de escribir el texto
    }

}