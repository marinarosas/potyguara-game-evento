using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeftHandController : MonoBehaviour
{
    public Animator ani;
    public bool controlMenu = false;
    public bool changedStatus = false;
    public bool isLeft = false;
    private List<InputDevice> devices = new List<InputDevice>();

    private void Update()
    {
        InputDeviceCharacteristics leftHandCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftHandCharacteristics, devices);
        devices[0].TryGetFeatureValue(CommonUsages.secondaryButton, out bool Ybutton);
        if (Ybutton) // Y button pressed
        {
            if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 1)
            {
                GameObject menu = GameObject.FindWithTag("MainMenu").transform.GetChild(0).gameObject;
                if (menu != null)
                {
                    if (!changedStatus)
                    {
                        controlMenu = !controlMenu;
                        menu.SetActive(controlMenu);
                        changedStatus = true;
                        GameObject.FindWithTag("MainCamera").transform.GetChild(4).gameObject.SetActive(!controlMenu);
                        Invoke("ChangeStatus", .3f);
                    }
                }
            }
        }
    }

    private void ChangeStatus()
    {
        changedStatus = !changedStatus;
    }

    public bool GetHand()
    {
        return isLeft;
    }

    public void ChangeHand()
    {
        InputDeviceCharacteristics leftHandCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftHandCharacteristics, devices);

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);
        isLeft = true;
    }

    public void ResetHand()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);
        isLeft = false;
    }

    public InputDevice GetTargetDevice()
    {
         return devices[0];
    }

    public void AnimationFinger()
    {
        ani.SetTrigger("IsFire");
        StartCoroutine(timeForStop());
    }
    
    IEnumerator timeForStop()
    {
        yield return new WaitForSecondsRealtime(0.3f);
        ani.ResetTrigger("IsFire");
    }
}
