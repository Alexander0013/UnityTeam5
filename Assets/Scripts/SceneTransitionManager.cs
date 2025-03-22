using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager instance;
    [SerializeField] Animator TransitionAnimator;

    // Reference to the CanvasGroup component on the fade canvas.
    public CanvasGroup fadeCanvasGroup;
    private void Awake()
    {
        // Singleton pattern: ensure only one SceneTransitionManager exists.
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
    public void NextScene()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        TransitionAnimator.SetTrigger("End");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex +1);
        TransitionAnimator.SetTrigger("End");

    }
    
}
