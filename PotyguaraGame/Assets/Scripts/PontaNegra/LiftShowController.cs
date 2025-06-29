using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class LiftShowController : MonoBehaviour
{
    public bool isInsideLift = false;
    public bool isGoingToShow = false;
    public bool isGoOutOfTheShow = false;
    public bool hasTicket = false;

    private Transform player;
    private Animator ani;
    private int state = -1;

    public Transform catraca1;
    public Transform catraca2;

    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
        transform.GetChild(1).GetComponent<TeleportationArea>().enabled = false;
        player = GameObject.FindWithTag("Player").transform;
    }

    public void OpenCatraca1()
    {
        catraca1.GetComponent<Animator>().Play("CatracaOpen");
        player.parent = null;
        if (isInsideLift)
            FindFirstObjectByType<HeightController>().NewHeight(6.1f);
    }

    public void OpenCatraca2()
    {
        catraca2.GetComponent<Animator>().Play("CatracaOpen");
        player.parent = null;
        if (isInsideLift)
            FindFirstObjectByType<HeightController>().NewHeight(0f);
    }

    public void UnleashLift()
    {
        transform.GetChild(1).GetComponent<TeleportationArea>().enabled = true;
        catraca1.GetChild(0).GetChild(0).gameObject.SetActive(false);
        catraca2.GetChild(0).GetChild(0).gameObject.SetActive(false);
    }

    public void BlockLift()
    {
        transform.GetChild(1).GetComponent<TeleportationArea>().enabled = false;
        catraca1.GetComponent<Animator>().Play("CatracaClose");
        catraca2.GetComponent<Animator>().Play("CatracaClose");
        catraca1.GetChild(0).GetChild(0).gameObject.SetActive(true);
        catraca2.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }

    public void GoToShow()
    {
        ani.Play("GoingToTheShow");
    }

    public void GoOutFromTheShow()
    {
        ani.Play("GoOutShow");
    }

    public void StartTrasition()
    {
        FindFirstObjectByType<PotyPlayerController>().HideControllers();
        GameObject.FindWithTag("MainCamera").transform.GetChild(3).GetComponent<FadeController>().FadeInForFadeOutWithAnimator(6f, ani);
    }

    public void EndTransition()
    {
        FindFirstObjectByType<PotyPlayerController>().ShowControllers();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInsideLift)
        {
            if (state == 1)
            {
                ani.Play("GoingToTheShow");
                TransitionController.Instance.isInShowArea = true;
            }
            else
            {
                ani.Play("GoOutShow");
                TransitionController.Instance.isInShowArea = false;
            }
        }
    }

    public void ChangeThePoint(int state)
    {
        if(state != this.state)
            this.state = state;
    }
    public int GetStatus()
    {
        return state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            isInsideLift = true;
            if (state == 1)
                catraca2.GetComponent<Animator>().Play("CatracaClose");
            else
                catraca1.GetComponent<Animator>().Play("CatracaClose");
            player.parent = transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            isInsideLift = false;
            if (state == 0)
            {
                FindFirstObjectByType<LiftShowController>().BlockLift();
            }
        }
    }
}