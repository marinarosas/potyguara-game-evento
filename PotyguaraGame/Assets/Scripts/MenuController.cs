using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private TransitionController transitionController;

    [SerializeField] private List<Sprite> galleryImages;
    [SerializeField] private Transform content;

    public Toggle toggleTutorial;
    public Toggle toggleWeather;

    void Start()
    {
        transitionController = FindFirstObjectByType<TransitionController>();
        int count = 0;
        foreach (var image in galleryImages) 
        { 
            content.GetChild(count).GetComponent<Image>().sprite = image;
            count++;
        }

        for (int ii = 0; ii < transform.childCount; ii++)
            content.GetChild(ii).gameObject.SetActive(true);

        Invoke("InitializeMenu", 0.8f);
    }

    public void InitializeMenu()
    {
        toggleTutorial.isOn = NetworkManager.Instance.modeTutorialOn;
        toggleWeather.isOn = NetworkManager.Instance.modeWeatherOn;
        if (SceneManager.GetActiveScene().buildIndex == 2) // ponta Negra
        {
            content.GetChild(0).gameObject.SetActive(false);
            content.GetChild(1).GetComponent<Button>().onClick.AddListener(GoToGallery);
            content.GetChild(2).GetComponent<Button>().onClick.AddListener(GoToMeditationRoom);
            content.GetChild(3).GetComponent<Button>().onClick.AddListener(LoadForte);
            int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
            if (potycoins >= 10)
            {
                content.GetChild(4).GetComponent<Button>().onClick.AddListener(GoToGameForte);
                content.GetChild(5).GetComponent<Button>().onClick.AddListener(GoToGameForteZombieMode);
                content.GetChild(6).GetComponent<Button>().onClick.AddListener(LoadHoverBunda);
            }
            else
            {
                content.GetChild(4).GetComponent<Button>().interactable = false;
                content.GetChild(5).GetComponent<Button>().interactable = false;
                content.GetChild(6).GetComponent<Button>().interactable = false;
            }
            content.GetChild(7).GetComponent<Button>().onClick.AddListener(LoadAvatarScene);
            content.GetChild(8).GetComponent<Button>().onClick.AddListener(ExitGame);
        }
        if (SceneManager.GetActiveScene().buildIndex == 3) // Reis Magos
        {
            content.GetChild(0).GetComponent<Button>().onClick.AddListener(LoadPontaNegra);
            content.GetChild(1).gameObject.SetActive(false);
            content.GetChild(2).gameObject.SetActive(false);
            content.GetChild(3).gameObject.SetActive(false);
            content.GetChild(4).gameObject.SetActive(false);
            content.GetChild(5).gameObject.SetActive(false);
            int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
            if (potycoins >= 10)
                content.GetChild(6).GetComponent<Button>().onClick.AddListener(LoadHoverBunda);
            else
                content.GetChild(6).GetComponent<Button>().interactable = false;
            content.GetChild(7).GetComponent<Button>().onClick.AddListener(LoadAvatarScene);
            content.GetChild(8).GetComponent<Button>().onClick.AddListener(ExitGame);
        }
        if (SceneManager.GetActiveScene().buildIndex == 4) // HoverBunda
        {
            content.GetChild(0).GetComponent<Button>().onClick.AddListener(LoadPontaNegra);
            content.GetChild(1).gameObject.SetActive(false);
            content.GetChild(2).gameObject.SetActive(false);
            content.GetChild(3).GetComponent<Button>().onClick.AddListener(LoadForte);
            int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
            if (potycoins >= 10)
            {
                content.GetChild(4).GetComponent<Button>().onClick.AddListener(GoToGameForte);
                content.GetChild(5).GetComponent<Button>().onClick.AddListener(GoToGameForteZombieMode);
            }
            else
            {
                content.GetChild(4).GetComponent<Button>().interactable = false;
                content.GetChild(5).GetComponent<Button>().interactable = false;
            }
            content.GetChild(6).gameObject.SetActive(false);
            content.GetChild(7).GetComponent<Button>().onClick.AddListener(LoadAvatarScene);
            content.GetChild(8).GetComponent<Button>().onClick.AddListener(ExitGame);
        }
    }

    public void SendModeWeather(bool value)
    {
        NetworkManager.Instance.SendModeWeather(value);
    }

    public void SendModeTutorial(bool value)
    {
        NetworkManager.Instance.SendModeTutorial(value);
    }

    void LoadAvatarScene()
    {
        transitionController.LoadSceneAsync(1);
    }
    void GoToMeditationRoom()
    {
        if (transitionController.isInShowArea)
        {
            GameObject locomotion = GameObject.Find("Locomotion").transform.GetChild(1).gameObject;
            if (locomotion != null)
                locomotion.SetActive(true);
            FindFirstObjectByType<LiftShowController>().hasTicket = false;
            transitionController.isInShowArea = false;
            Destroy(GameObject.Find("Dragon(Clone)"));
            Destroy(GameObject.Find("Guitaura(Clone)"));
            FindFirstObjectByType<LiftShowController>().GoOutFromTheShow();
            FindFirstObjectByType<LiftShowController>().BlockLift();
        }
        transitionController.TeleportMeditationRoom();
    }

    void LoadPontaNegra()
    {
        transitionController.LoadSceneAsync(2);
    }

    void LoadForte()
    {
        transitionController.LoadSceneAsync(3);
    }

    void LoadHoverBunda()
    {
       transitionController.LoadSceneAsync(4);
    }

    void GoToGallery()
    {
        if (transitionController.isInShowArea)
        {
            GameObject locomotion = GameObject.Find("Locomotion").transform.GetChild(1).gameObject;
            if (locomotion != null)
                locomotion.SetActive(true);
            FindFirstObjectByType<LiftShowController>().hasTicket = false;
            Destroy(GameObject.Find("Dragon(Clone)"));
            Destroy(GameObject.Find("Guitaura(Clone)"));
            transitionController.isInShowArea = false;
            FindFirstObjectByType<LiftShowController>().GoOutFromTheShow();
            FindFirstObjectByType<LiftShowController>().BlockLift();
        }
        FindFirstObjectByType<MeditationRoomController>().StopClass();
        transitionController.TeleportGallery();
    }

    void GoToGameForte()
    {
        transitionController.TeleporGameForteNormalMode();
    }

    void GoToGameForteZombieMode()
    {
        transitionController.TeleportGameForteZombieMode();
    }
    void ExitGame()
    {
        transitionController.LoadSceneAsync(0);
    }
}
