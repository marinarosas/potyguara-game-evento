using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenController : MonoBehaviour
{
    public Slider slider;
    private int index;

    private void Awake()
    {
        index = TransitionController.Instance.GetTempIndex();
    }

    void Start()
    {
        StartCoroutine(LoadSceneWithProgress(index));
    }
    private IEnumerator LoadSceneWithProgress(int sceneIndex)
    {

        slider.value = 0;
        yield return new WaitForSeconds(4f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress;
            slider.value = progress;

            yield return null;
        }

    }
}
