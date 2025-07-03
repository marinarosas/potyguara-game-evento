using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class TransitionController : MonoBehaviour
{
    private GameObject player;
    private bool isSkip = false;
    private int tempMode;
    private int tempSceneIndex = -1;
    public bool isInShowArea = false;

    public static TransitionController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            GameObject locomotion = GameObject.Find("Locomotion");
            if (locomotion != null)
                locomotion.SetActive(false);
        }
        else
        {
            GameObject locomotion = GameObject.Find("Locomotion");
            if(locomotion != null)
               locomotion.SetActive(true);
        }
    }

    public void UpdateMainMenu(bool value)
    {
        if (value)
        {
            GameObject.Find("MainMenu").transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("MainMenu").transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => LoadSceneAsync(1));
            GameObject.Find("MainMenu").transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Criar Perfil";
            GameObject.Find("MainMenu").transform.GetChild(2).gameObject.SetActive(false);
            GameObject.Find("MainMenu").transform.GetChild(3).GetComponent<Button>().onClick.AddListener(ExitGame);
        }
        else
        {
            GameObject.Find("MainMenu").transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("MainMenu").transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => LoadSceneAsync(2));
            GameObject.Find("MainMenu").transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Iniciar Jogo";
            GameObject.Find("MainMenu").transform.GetChild(4).GetChild(2).GetComponent<Button>().onClick.AddListener(NetworkManager.Instance.DeletePerfil);
            GameObject.Find("MainMenu").transform.GetChild(2).gameObject.SetActive(true);
            GameObject.Find("MainMenu").transform.GetChild(3).GetComponent<Button>().onClick.AddListener(ExitGame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSkip && SceneManager.GetActiveScene().buildIndex == 3)
        {
            GameForteController.Instance.SetStartMode(tempMode);
            if (tempMode == 0)
                GameForteController.Instance.GetZombieModeButton().onClick.Invoke();
            else if (tempMode == 1)
                GameForteController.Instance.GetNormalModeButton().onClick.Invoke();

            isSkip = false;
        }
    }

    public int GetTempIndex()
    {
        return tempSceneIndex;
    }

    public void LoadSceneAsync(int sceneIndex)
    {
        tempSceneIndex = sceneIndex;
        StartCoroutine(LoadSceneAsyncRoutine(5));
    }

    IEnumerator LoadSceneAsyncRoutine(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            yield return null;
        }
    }

    public void LoadSceneWithTime(int sceneIndex, int time)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex, time));
    }

    IEnumerator GoToSceneRoutine(int sceneIndex, int time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadScene(int number)
    {
        try
        {
            if (SceneManager.GetActiveScene().buildIndex != number)
                SceneManager.LoadScene(number);
        }
        catch (Exception e)
        {
            Debug.Log("Error when load scenes: " + e);
        }
    }

    public void TeleportExitShow()
    {
        FindFirstObjectByType<HeightController>().NewHeight(0f);
        player.transform.position = new Vector3(177.5f, 0f, 72f);
    }

    public void TeleportGallery()
    {
        player = GameObject.FindWithTag("Player");
        FindFirstObjectByType<HeightController>().NewHeight(0f);
        player.transform.position = new Vector3(132.53f, 0f, 15.69f);
        player.transform.eulerAngles = new Vector3(0, 180f, 0);
    }

    public void TeleportMeditationRoom()
    {
        player = GameObject.FindWithTag("Player");
        FindFirstObjectByType<HeightController>().NewHeight(0f);
        player.transform.position = new Vector3(160.36f, 0f, 10.88f);
        player.transform.eulerAngles = new Vector3(0f, 180f, 0);
    }

    public void TeleportGameForteZombieMode()
    {
        tempMode = 0;
        isSkip = true;
        SceneManager.LoadSceneAsync(3);
    }

    public bool GetIsSkip()
    {
        return isSkip;
    }

    public void TeleporGameForteNormalMode()
    {
        tempMode = 1;
        isSkip = true;
        SceneManager.LoadSceneAsync(3);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
