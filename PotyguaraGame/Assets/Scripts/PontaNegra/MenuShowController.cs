using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WaypointsFree;

public class MenuShowController : MonoBehaviour
{
    [Header("Button Prefab")]
    [SerializeField] public GameObject buttonPrefab;

    [Header("Menu Container")]
    [SerializeField] private Show[] shows;

    [Header("Menu Container")]
    [SerializeField] private Transform content;

    public bool showLiberated = false;
    private Show currentShow;

    private void Start()
    {
        foreach (Show show in shows)
        {
            CreateButton(show.image, show.description);
        }
        CheckTickets();
        UnclockShow("2001");
    }

    private void Update()
    {
        if (showLiberated)
        {
            GameObject banda = Instantiate(currentShow.banda);
            banda.transform.position = new Vector3(180.46f, 6.89f, 251.19f);
            banda.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            foreach (GameObject extra in currentShow.extras)
            {
                GameObject obj = Instantiate(extra);
                obj.transform.position = new Vector3(184.1f, 34.6f, 210.1f);
            }

            banda.GetComponent<BandController>().StartShow(currentShow.clip);
            showLiberated = false;
        }
    }
    private void CreateButton(Sprite image, string description)
    {
        GameObject newButton = Instantiate(buttonPrefab, content);
        newButton.GetComponent<Image>().sprite = image;
        newButton.GetComponent<Button>().interactable = false;
        Destroy(newButton.transform.GetChild(1).gameObject);
    }

    public void UnclockShow(string id)
    {
        for (int ii = 0; ii < shows.Length; ii++)
        {
            if (shows[ii].id == id)
            {
                content.GetChild(ii).GetComponent<Button>().interactable = true;
                content.GetChild(ii).GetComponent<Button>().onClick.AddListener(() => UnclockDeck(shows[ii]));
                return;
            }
        }
    }

    public void CheckTickets()
    {
        List<string> tickets = FindFirstObjectByType<PotyPlayerController>().GetTickets();
        if (tickets.Count != 0)
        {
            foreach(string ticket in tickets)
                UnclockShow(ticket);
        }
    }

    private void UnclockDeck(Show show)
    {
        currentShow = show;
        FindFirstObjectByType<LiftShowController>().hasTicket = true;
        transform.GetChild(0).GetComponent<FadeController>().FadeOutWithDeactivationOfGameObject(transform.GetChild(0).gameObject);
        FindObjectOfType<LiftShowController>().UnleashLift();
        FindObjectOfType<LiftShowController>().OpenCatraca2();
    }
}
