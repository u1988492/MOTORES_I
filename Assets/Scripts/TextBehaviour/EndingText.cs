using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScene : MonoBehaviour
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

    public void StartEnding()
    {
        dialogues = new string[] // Definición de los diálogos
        {
            "You are greeted by the monster that waits outside: a green-scaled serpent with a yellowish belly, bigger than any human being. On its head grow two twisting horns that point in opposite directions. \nThe serpent stands tall on its belly, holding its head high, its long tongue hanging between two pointy fangs. Its piercing yellow eyes look down at you.",
            "The monster hisses, its voice a low, resonant growl that reverberates through the air.",
            "'What is it? So there was a mind that remained itself still?'",
            "The monster takes a step forward, coiling its massive body, each movement sending tremors through the ground.",
            "'But do you think you can be free, little one? \nThis is as far as you can reach. Soon, your world will grow dark too. \nThere is no escape from us, so come to me. Now.'",
            "You stand frozen, clutching the feather with both hands, your mind racing. You remember the guardian's words: 'Raise mine golden feather to the heavens, and the very thunder shall smite those who threaten thy people.'",
            "You hesitate. How could a simple feather erase the damage that had been done?",
            "The monster approaches, gliding slowly. But you remain still, as if your senses have left your body, hypnotized by those serpentine eyes. And still, thoughts keep flowing, and a rush of melancholy sweeps in.",
            "You thinks about your mother. You can't remember an exact image, but you are reminded of her voice the last time you saw her: a sound as sweet as honey, but with a bitter aftertaste. \nShe didn't want this. She wanted to stay with the you. But where is she now? Did she keep her promise? Is she okay, wherever she is?",
            "The feather glows with a brilliant light of gold, filled with raw emotion. The moment you notice, you snap out of the trance and slowly raise the feather high into the air.",
            "The wind picks up, and the sky starts to darken. Thunder rumbles in the distance, growing louder, and the serpent recoils, its yellow eyes flickering with unease.",
            "You bring one hand to your chest, grasping your heart. The light intensifies, and a bolt of thunder strikes the ground, shaking the earth beneath the monster. \nThe air crackles with electricity as the storm surges, and lightning strikes mercilessly into the monster and all throughout the town, wherever creatures stand and shadows lurk.",
            "The anxious wind carries the deep howls of agony, and you watch the monster as it crumbles into the earth. Its malevolent eyes now rest lifeless, and the green scales turn to dust with the swiftest gust of wind. \nThe corruption that plagued the town fades away as the clouds clear from the sky. You watch, breathless, as the darkness begins to lift. The feather dims as the last remnants of its power fade away.",
            "You stand alone in the silence, watching the breeze carry away the ashes that remain of the creature. Little by little, it is revealed: the body trapped within the shell.",
            "Her hair ripples over her head, shadowing a well-known face. Her eyelids will not open, nor will her hands move to reach out for you again. But the edges of her lips curve slightly upward. After all, she kept her promise and waited there for you to come out. And at last, she has found it — the long-awaited peace.",
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