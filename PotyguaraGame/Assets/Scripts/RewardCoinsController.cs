using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RewardCoinsController : MonoBehaviour
{
    private bool potycoinContabilized = false;

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
            if (NetworkManager.Instance.newDay)
                Invoke("RewardCoins", 0.6f);
    }

    private void RewardCoins()
    {
        AudioSource audio = transform.GetChild(0).GetChild(4).GetComponent<AudioSource>();
        ParticleSystem confetti = transform.GetChild(1).GetComponent<ParticleSystem>();
        GetComponent<FadeController>().FadeInForFadeOutWithDeactivationOfGameObject(2f, gameObject);

        confetti.Play();
        audio.Play();

        if (!potycoinContabilized)
        {
            FindFirstObjectByType<PotyPlayerController>().SetPotycoins(50);
            potycoinContabilized = true;
            NetworkManager.Instance.SendRewardCoins();
            NetworkManager.Instance.newDay = false;
            StartCoroutine("ResetBoolean");
        }
    }

    private IEnumerator ResetBoolean()
    {
        yield return new WaitForSeconds(2f);
        potycoinContabilized = false;
    }
}
