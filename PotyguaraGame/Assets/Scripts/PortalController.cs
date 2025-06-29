using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("MainCamera"))
        {
            if (transform.parent.name.Equals("ForteDosReis"))
            {
                int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
                if (potycoins >= 10)
                {
                    TransitionController.Instance.LoadSceneAsync(3);
                }
            }
            else if(transform.parent.name.Equals("PontaNegra"))
            {
                int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
                if (potycoins >= 10)
                {
                    TransitionController.Instance.LoadSceneAsync(2);
                }
            }
            else if(transform.parent.name.Equals("HoverBunda"))
            {
                int potycoins = FindFirstObjectByType<PotyPlayerController>().GetPotycoins();
                if (potycoins >= 10)
                {
                    TransitionController.Instance.LoadSceneAsync(4);
                }
            }
            else
            {
                TransitionController.Instance.TeleportMeditationRoom();
                transform.parent.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
