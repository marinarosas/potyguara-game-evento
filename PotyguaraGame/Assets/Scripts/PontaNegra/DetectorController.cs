using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            if (transform.name.Equals("Detector1"))
            {
                GameObject menuShow = FindFirstObjectByType<MenuShowController>().gameObject.transform.GetChild(0).gameObject;
                menuShow.SetActive(true);
                menuShow.GetComponent<FadeController>().FadeIn();
                FindFirstObjectByType<LiftShowController>().ChangeThePoint(1);
            }
            else if (transform.name.Equals("Detector2")) 
            {
                FindFirstObjectByType<LiftShowController>().ChangeThePoint(0);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            if (transform.name.Equals("Detector1"))
            {
                GameObject menuShow = FindFirstObjectByType<MenuShowController>().gameObject.transform.GetChild(0).gameObject;
                menuShow.GetComponent<FadeController>().FadeOutWithDeactivationOfGameObject(menuShow);
            }
        }
    }
}