using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TechGuaraController : MonoBehaviour
{
    private Report report;
    private AudioSource audioSource;
    [SerializeField] private List<AudioClip> audios;

    private void InitReport()
    {
        if (NetworkManager.Instance.isTheFirstAcess)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                CreateReport("Techyguara.InicioDoJogo.CriaçãoDeCadastro+Avatar", new Vector3(0f, 2f, -32.35f), 0f, "Bem-vindo(a) ao Potyguara Verse!", "Você acaba de entrar " +
                    "em um mundo onde a cultura e a tecnologia se encontram em uma experiência imersiva única. Eu sou a Techyguara, sua guia, e juntos vamos explorar esse " +
                    "universo cheio de novidades! No Potyguara Verse, você poderá participar de eventos incríveis, jogar minigames, visitar nossa loja exclusiva e muito mais.");
            }
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (FindFirstObjectByType<PotyPlayerController>().GetIndex() == -1)
            {
                CreateReport("Techyguara.CriaçãodePerfil+Avatar", transform.position, 0f, "Crie seu Avatar!", "Agora que você já se apresentou, é hora de criar seu avatar! Escolha " +
                "suas características, como rosto, cabelo, roupas e acessórios para refletir sua personalidade no Potyguara Verse. Depois, você estará pronto para explorar" +
                " este mundo como nunca antes!");
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (!NetworkManager.Instance.firstInPN)
            {
                CreateReport("Techyguara.ApresentaçãoPraiadePontaNegra", new Vector3(177f, 3f, 76f), 0f, "Praia de Ponta Negra", "Você está na famosa Praia de Ponta Negra, " +
                    "uma das mais conhecidas da Cidade do Natal, principalmente por conta do imponente Morro do Careca, com seus 110 metros de altura. Aqui, você encontrará " +
                    "diversos eventos, como shows, exposições e o emocionante minigame Hoverbunda. Sinta-se livre para caminhar pela praia, explorar as atrações e aproveitar " +
                    "os eventos incríveis!");
                NetworkManager.Instance.firstInPN = true;
                NetworkManager.Instance.SendSignalTutorialOK("pnTutorialOK");
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            if (!NetworkManager.Instance.firstInForte)
            {
                if (!TransitionController.Instance.GetIsSkip())
                {
                    CreateReport("Techyguara.ApresentaçãoFortalezaDosReisMagos", new Vector3(804.55f, 10.34f, 400.19f), 0f, "Forte dos Reis Magos", "Agora, vamos à Fortaleza " +
                        "dos Reis Magos, um dos locais mais históricos da cidade de Natal. Este lugar foi palco de batalhas importantes que mudaram o rumo da nossa região." +
                        "Aqui, você poderá jogar minigames inspirados em épocas passadas. Sabia que, durante as invasões holandesas, a cidade de Natal foi chamada de Nova " +
                        "Amsterdã? Explore os muros de pedra e descubra mais sobre essa fascinante história enquanto se diverte!");
                    NetworkManager.Instance.firstInForte = true;
                    NetworkManager.Instance.SendSignalTutorialOK("forteTutorialOK");
                }
            }
        }
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            if (!NetworkManager.Instance.firstInHover)
            {
                SelectAudioReport("Techyguara.ApresentaçãoHoverbunda");
                CreateReport("HoverBunda", "Prepare-se para a adrenalina no Hoverbunda, uma corrida emocionante onde você se lança no seu skibunda voador! Compita contra seus amigos e mostre que você é o melhor, pois " +
                    "apenas o mais rápido cruzará a linha de chegada!", new Vector3(480.1024f, 65.43f, -410.83f));
                transform.GetChild(0).GetComponent<FadeController>().FadeWithDeactivationAndActivationOfGameObject(audioSource.clip.length, transform.GetChild(0).gameObject, GameObject.Find("InitialCanva").transform.GetChild(0).gameObject);
                audioSource.Play();
                NetworkManager.Instance.firstInHover = true;
                NetworkManager.Instance.SendSignalTutorialOK("hoverTutorialOK");
            }
            else
            {
                GameObject.Find("InitialCanva").transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    void Start()
    {
        audioSource = transform.GetChild(2).GetComponent<AudioSource>();
        report = transform.GetChild(0).GetComponent<Report>();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        Invoke("InitReport", 0.3f);
    }

    public void StopTutorial()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        audioSource.Stop();
    }

    void Update()
    {
        if (audioSource.isPlaying && SceneManager.GetActiveScene().buildIndex == 0)
        {
            if (audioSource.time >= 27.0)
            {
                report.UpdateTitle("Complete seu Perfil!");
                report.UpdateDescription("Antes de começarmos, vamos conhecer um pouco mais sobre você! Crie o seu avatar para começar a sua jornada!");
            }
        }
    }

    #region ReportConfig

    public void CreateReport(string clipName, Vector3 pos, float direction, string title, string description)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        SelectAudioReport(clipName);
        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        transform.GetChild(0).GetComponent<FadeController>().FadeInForFadeOutWithDeactivationOfGameObject(audioSource.clip.length, transform.GetChild(0).gameObject);
        transform.position =pos;

        report.UpdateTitle(title);
        report.UpdateDescription(description);
        audioSource.Play();
    }

    public void CreateReport(string title, string description, float duration)
    {
        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        transform.GetChild(0).GetComponent<FadeController>().FadeInForFadeOutWithDeactivationOfGameObject(duration, transform.GetChild(0).gameObject);

        report.UpdateTitle(title);
        report.UpdateDescription(description);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CreateReport(string title, string description, Vector3 pos)
    {
        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        transform.position = pos;

        report.UpdateTitle(title);
        report.UpdateDescription(description);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CreateReport(string title, string description, float duration, Vector3 pos, float direction)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, direction, 0f);

        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        transform.GetChild(0).GetComponent<FadeController>().FadeInForFadeOutWithDeactivationOfGameObject(duration, transform.GetChild(0).gameObject);

        report.UpdateTitle(title);
        report.UpdateDescription(description);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void CreateReport(string title, string description, Vector3 pos, float direction)
    {
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0f, direction, 0f);

        transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        transform.GetChild(0).GetComponent<FadeController>().FadeIn();

        report.UpdateTitle(title);
        report.UpdateDescription(description);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    private void SelectAudioReport(string name)
    {
        foreach (AudioClip clip in audios)
        {
            if (clip.name.Equals(name))
            {
                audioSource.clip = clip;
                return;
            }
        }
    }

    public AudioSource SelectReport(string name)
    {
        foreach (AudioClip clip in audios)
        {
            if (clip.name.Equals(name))
            {
                audioSource.clip = clip;
                return audioSource;
            }
        }
        return null;
    }
    #endregion
}
