using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.XR;

public class RightHandController : MonoBehaviour
{
    public Animator ani;
    public bool isRight = false;
    private List<InputDevice> devices = new List<InputDevice>();

    public bool GetHand()
    {
        return isRight;
    }
    public void ChangeHand()
    {
        InputDeviceCharacteristics rightHandCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightHandCharacteristics, devices);

        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(true);
        isRight = true;
    }

    public void ResetHand()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).gameObject.SetActive(true);
        transform.GetChild(3).gameObject.SetActive(false);
        isRight = false;
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
        yield return new WaitForSecondsRealtime(0.4f);

        ani.ResetTrigger("IsFire");
    }
}
