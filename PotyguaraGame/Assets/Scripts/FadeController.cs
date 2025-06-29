using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    private CanvasGroup canvas;
    private bool fadeIn = false;
    private bool fadeOut = false;
    private GameObject objToDesactive;
    private GameObject objToActive;
    private Animator animator;

    private bool status = true;
    void Awake()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
    }

    public void FadeWithStatus()
    {
        if (status) FadeIn();
        else FadeOut();

        status = !status;
    }

    public void FadeOut()
    {
        if (canvas.alpha == 1)
            fadeOut = true;
    }

    public void FadeWithTime(string type, float time)
    {
        if (type.Equals("Out"))
            Invoke("FadeOut", time);
        else
            Invoke("FadeIn", time);
    }

    public void FadeInForFadeOut(float time)
    {
        FadeIn();
        Invoke("FadeOut", time);
    }

    public void FadeInForFadeOutWithAnimator(float time, Animator ani)
    {
        animator = ani;
        FadeIn();
        Invoke("FadeOut", time);
    }

    public void FadeInForFadeOutWithDeactivationOfGameObject(float time, GameObject objeto)
    {
        FadeIn();
        SetObjectD(objeto);
        Invoke("FadeOut", time);
    }

    public void FadeWithDeactivationAndActivationOfGameObject(float time, GameObject objetoD, GameObject objetoA)
    {
        FadeIn();
        SetObjectD(objetoD);
        SetObjectA(objetoA);
        Invoke("FadeOut", time);
    }

    public void FadeOutWithDeactivationOfGameObject(GameObject objeto)
    {
        FadeOut();
        SetObjectD(objeto);
    }

    public void FadeOutForFadeIn(float time)
    {
        FadeOut();
        Invoke("FadeIn", time);
    }

    public void SetObjectD(GameObject obj)
    {
        this.objToDesactive = obj;
    }

    public void SetObjectA(GameObject obj)
    {
        this.objToActive = obj;
    }

    private void Update()
    {
        if (fadeIn)
        {
            if (canvas.alpha < 1f)
                canvas.alpha += Time.deltaTime;
            else
                fadeIn = false;
        }
        if (fadeOut)
        {
            if (canvas.alpha > 0f)
                canvas.alpha -= Time.deltaTime;
            else
            {
                if (objToDesactive != null)
                    objToDesactive.SetActive(false);
                else
                {
                    if (SceneManager.GetActiveScene().buildIndex == 2) // ponta negra
                    {
                        if (animator != null)
                        {
                            if (animator.GetCurrentAnimatorStateInfo(0).IsName("DownTerreo"))
                                FindFirstObjectByType<HeightController>().NewHeight(0f);
                            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("MoveUpTerreo"))
                                FindFirstObjectByType<HeightController>().NewHeight(9.158f);
                        }
                    }
                }
                if (objToDesactive != null && objToActive != null)
                {
                    objToDesactive.SetActive(false);
                    objToActive.SetActive(true);
                    objToActive = null;
                }
                canvas.alpha = 0;
                fadeOut = false;
            }
        }
    }
    public void FadeIn()
    {
        if(canvas.alpha == 0)
            fadeIn = true;
    }
}
