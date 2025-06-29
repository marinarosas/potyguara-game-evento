using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SkinIntegrationController : MonoBehaviour
{
    public XRNode inputSource = XRNode.LeftHand; // Define qual controle será utilizado
    private Animator animator; // Referência ao Animator do avatar;
    private void Start()
    {
        Transform mainCam = transform.GetChild(0).GetChild(0);
        Transform avatar = transform.GetChild(0).GetChild(5);
        if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5)
        {
            mainCam.GetChild(4).gameObject.SetActive(false);
            avatar.gameObject.SetActive(false);
        }
        else
        {
            mainCam.GetChild(4).gameObject.SetActive(true);
            avatar.gameObject.SetActive(true);
            GetSkin();
        }
    }

    private void GetSkin()
    {
        int skinIndex = FindFirstObjectByType<PotyPlayerController>().GetIndex();
        int skinMaterial = FindFirstObjectByType<PotyPlayerController>().GetMaterial();
        int skinGender = FindFirstObjectByType<PotyPlayerController>().GetGender();
        Transform avatar = transform.GetChild(0).GetChild(5);
        avatar.GetComponent<SetSkin>().SetSkinAvatar(skinIndex, skinMaterial, skinGender);
        animator = avatar.GetComponent<SetSkin>().UpdateAnimator();
    }

    // Update is called once per frame
    void Update()
    {
        /*InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        bool isMoving = inputAxis.magnitude > 0.1f;
        if (animator != null)
        {
            animator.SetBool("isWalking", isMoving);
        }*/
    }
}
