using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BulletController : MonoBehaviour
{
    private float timeToDestroy = 10f;
    [SerializeField] private VisualEffect BloodVisualEffectController;
    [SerializeField] private CanvasGroup bloodOnScreeen;
    private Coroutine currentCoroutine;

    private void Start()
    {
        BloodVisualEffectController.gameObject.SetActive(false);
        BloodVisualEffectController.SendEvent("Reset");
        Invoke("autoDestroy", timeToDestroy);
    }
    private void autoDestroy()
    {
        Destroy(gameObject);
    }

    public void playEffect()
    {
        if (BloodVisualEffectController != null)
        {
            BloodVisualEffectController.gameObject.SetActive(true);
            BloodVisualEffectController.SendEvent("Bleeding");
            BloodVisualEffectController.transform.parent = null;
        }
        else
            Debug.LogWarning("VisualEffect reference is missing, cannot play effect on " + gameObject.name);
    }

    public void StartBloodEffect(float waitDuration, float fadeOutDuration){
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(BloodEffectCoroutine(waitDuration, fadeOutDuration));
    }

    private IEnumerator BloodEffectCoroutine(float waitDuration, float fadeOutDuration){
        // Fade In
        float elapsedTime = 0f;
        bloodOnScreeen.alpha = 1;

        // Wait
        yield return new WaitForSeconds(waitDuration);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            bloodOnScreeen.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            yield return null;
        }
        bloodOnScreeen.alpha = 0;
    }
}
