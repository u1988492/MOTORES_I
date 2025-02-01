using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class TutorialManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private CanvasGroup textCanvasGroup;

    [Header("Configuración")]
    [SerializeField] private float fadeSpeed = 2f;

    private bool movementTutorialCompleted = false;
    private bool eKeyTutorialCompleted = false;
    private bool vKeyTutorialCompleted = false;
    private bool iKeyTutorialCompleted = false;
    private bool showV = false;

    private void Start()
    {
        textCanvasGroup.alpha = 0;
    }

    public void StartTutorial()
    {
        textCanvasGroup.alpha = 1;
        StartCoroutine(WaitStartTime());
        ShowMovementTutorial();
    }

    private void Update()
    {
        if (!showV && GameManager.instance.IsVisionUnlocked())
        {
            showV = true;
            StartCoroutine(ShowNextTutorial(ShowVKeyTutorial));
        }
        if (!movementTutorialCompleted)
        {
            CheckMovementInput();
        }
        else if (!eKeyTutorialCompleted)
        {
            CheckEKeyInput();
        }
        else if (!iKeyTutorialCompleted)
        {
            CheckIKeyInput();
        }
        else if (!vKeyTutorialCompleted && GameManager.instance.IsVisionUnlocked())
        {
            CheckVKeyInput();
        }
    }

    private void CheckMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(FadeOutText());
            movementTutorialCompleted = true;
            StartCoroutine(ShowNextTutorial(ShowEKeyTutorial));
        }
    }

    private void CheckEKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeOutText());
            eKeyTutorialCompleted = true;
            StartCoroutine(ShowNextTutorial(ShowIKeyTutorial));
        }
    }

    private void CheckIKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(FadeOutText());
            iKeyTutorialCompleted = true;
            Debug.Log("Tutorial completado! (sin V)");
        }
    }

    public void StartVisionTutorial()
    {
        StartCoroutine(ShowNextTutorial(ShowVKeyTutorial));
    }

    private void CheckVKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(FadeOutText());
            vKeyTutorialCompleted = true;
            Debug.Log("Tutorial completado! (con V)");
        }
    }

    private void ShowMovementTutorial()
    {
        tutorialText.text = "Press WASD to move";
    }

    private void ShowEKeyTutorial()
    {
        tutorialText.text = "Press E to interact with interactable objects";
    }

    private void ShowVKeyTutorial()
    {
        tutorialText.text = "Press V to activate and desactivate the guardian's vision";
    }

    private void ShowIKeyTutorial()
    {
        tutorialText.text = "Press I to open the inventory";
    }

    private IEnumerator WaitStartTime()
    {
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator FadeOutText()
    {
        while (textCanvasGroup.alpha > 0)
        {
            textCanvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    private IEnumerator ShowNextTutorial(System.Action showNextText)
    {
        yield return new WaitForSeconds(3f);
        textCanvasGroup.alpha = 1;
        showNextText();
    }
}