using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    public CanvasGroup switchSceneCG;
    public CanvasGroup EndindCG;
    public CanvasGroup QuitButtomCG;
    public float fadeDuration = 1f;
    [TextArea(3, 10)]
    public string endingStory;
    public TextMeshProUGUI endingText;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        switchSceneCG.alpha = 0;
        EndindCG.alpha = 0;
        EndindCG.gameObject.SetActive(false);
    }

    public IEnumerator FadeOutAndLoadSingle(int buildIndex)
    {
        yield return StartCoroutine(Fade(1f, switchSceneCG));
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
        yield return StartCoroutine(Fade(0f, switchSceneCG));
    }


    public IEnumerator Fade(float targetAlpha,CanvasGroup canvasGroup)
    {
        float startAlpha = canvasGroup.alpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha,timeElapsed / fadeDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    public void EndGame()
    {
        EndindCG.gameObject.SetActive(true);
        StartCoroutine(Fade(1f, EndindCG));        
        StartCoroutine(DisplaySectence(endingStory));
        
        //Time.timeScale = 0;
    }

    IEnumerator DisplaySectence(string sentence)
    {
        endingText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            endingText.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
        StartCoroutine(Fade(1f, QuitButtomCG));
    }

    public void OnQuitButtonClick()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
