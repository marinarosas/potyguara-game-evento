using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MeditationRoomController : MonoBehaviour
{
    private bool StartedClass = false;
    private int countClasses = 0;

    [SerializeField] private GameObject magicCircles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform content;
    [SerializeField] private List<AudioClip> audios;
    [SerializeField] private Font font;
    // Start is called before the first frame update
    private void Start()
    {
        FindFirstObjectByType<SalesCenterController>().CheckSessions();
        transform.GetChild(0).GetChild(0).GetChild(4).GetComponent<Button>().onClick.AddListener(ExitRoom);
        AddButton(0);
    }
    void Update()
    {
        if (audioSource != null)
        {
            if (!audioSource.isPlaying && StartedClass)
            {
                ResetRoom();
                StartedClass = false;
            }
        }
    }

    private void ResetRoom()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }

    public void StopClass()
    {
        audioSource.Stop();
        GameObject locomotion = GameObject.Find("Player").transform.GetChild(1).gameObject;
        if (locomotion != null)
            locomotion.SetActive(true);
        ResetRoom();
    }

    private void ExitRoom()
    {
        TransitionController.Instance.TeleportGallery();
        GameObject locomotion = GameObject.Find("Player").transform.GetChild(1).gameObject;
        if (locomotion != null)
            locomotion.SetActive(true);
        ResetRoom();
    }

    #region ButtonCreation
    public void AddButton(int index)
    {
        countClasses++;
        AudioClip clip = audios[index];
        GameObject buttonGo = new GameObject("Session " + countClasses);
        buttonGo.transform.SetParent(content);

        RectTransform rectTransform = buttonGo.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(230, 216);

        Vector3 localPos = rectTransform.localPosition;
        localPos.z = 0f;
        rectTransform.localPosition = localPos;

        rectTransform.localScale = new Vector3(1, 1, 1);

        Image image = buttonGo.AddComponent<Image>();
        Button button = buttonGo.AddComponent<Button>();

        GameObject textGo = new GameObject("Text");
        textGo.transform.SetParent(buttonGo.transform);

        RectTransform rectTransform2 = textGo.AddComponent<RectTransform>();
        rectTransform2.sizeDelta = new Vector2(200, 100);

        Vector3 localPos2 = rectTransform2.localPosition;
        localPos2.z = 0f;
        localPos2.x = 0f;
        rectTransform2.localPosition = localPos2;

        rectTransform2.localScale = new Vector3(1, 1, 1);

        Text text = textGo.AddComponent<Text>();
        text.font = font;
        text.fontSize = 36;
        text.text = "Sessão " + countClasses;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.9245283f, 0.3079158f, 0, 1);

        button.onClick.AddListener(() => PlayClass(clip));
    }

    private void PlayClass(AudioClip clip)
    {
        StartedClass = true;
        magicCircles.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(false);
        audioSource.clip = clip;
        audioSource.Play();
        GameObject.FindWithTag("Player").transform.GetChild(1).gameObject.SetActive(false);
    }
    #endregion
}
