using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuardianDialogue : MonoBehaviour
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

    private InteractionSystem interactionSystem;

    private void Awake() // Añade el script y configura la velocidad de la máquina de escribir
    {
        interactionSystem = FindObjectOfType<InteractionSystem>(); // Busca el script InteractionSystem en la escena
        typewriterEffect = gameObject.AddComponent<TypewriterEffect>(); 
        typewriterEffect.typingSpeed = typingSpeed;
    }

    private void Start()
    {
        dialogues = new string[] // Definición de los diálogos - ******tengo que cambiar el dialogo pero hay que probar si
        {
            "Who dares trespass into mine domain and disturb my slumber?",
            "What’s this? A mortal soul? How dost a human come to know of this sacred place?",
            "Ah… I scent the blood of the ancient giant coursing through thy veins.",
            "But no – there is more… the faintest hint of the ancient wolf’s spirit.",
            "Thou bearest the scent of the claw. The very essence of the wolf who stood against the storm and spirits themselves.",
            "How dost thou come to possess suck a token?",
            "Thou seekest something, do thee not? I know what it is.",
            "I can smell it… The spirits have risen once more, have they?",
            "Worry not, for I shall not turn thee away, but I must ask in return: the wolf's claw is no mere trinket – it is the very lifeblood of its spirit.",
            "If thou wouldst seek my aid, it is only fair that I receive an offering. Will you barter this gift for the safety of thy town?",
            
            "[You take out the claw pendant from your pocket and take a look at it.]",
            "[Is this what your mother meant? Is this the key to finding a safe place?]",
            "[You raise the pendant above your head and offer it to the figure in front of you. A gust of wind takes it away from you, and the pendant flies before the guardian]",
            
            "Then take this — one of the sacred feathers plucked from mine own tail.",
            "Though I cannot depart from this place, raise mine golden feather to the heavens, and the very thunder shall smite those who threaten thy people.",
            "Go now, hasten. Time slips swiftly from thy grasp."
        };

        StartCoroutine(ShowText());
    }

    private IEnumerator ShowText()
    {
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

        GameManager.instance.tpBunker();
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