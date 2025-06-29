using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScreen : MonoBehaviour
{
    public bool fadeOnStart = true;
    public float fadeDuration = 2;
    public CanvasGroup cg;

    // Start is called before the first frame update
    void Start()
    {
        if (fadeOnStart && SceneManager.GetActiveScene().buildIndex != 5)
            FadeIn();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        FadeIn();
    }
    public void FadeIn()
    {
        Fade(1, 0);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeInRoutine(cg, alphaIn, alphaOut));
    }

    public IEnumerator FadeInRoutine(CanvasGroup canvasGroup, float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }
}
