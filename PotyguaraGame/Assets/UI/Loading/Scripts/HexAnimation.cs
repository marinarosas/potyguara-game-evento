using UnityEngine;
using System.Collections;

public class HexAnimation : MonoBehaviour
{
    [SerializeField] private float loop = 2f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private float animationTime = 1f;

    private IEnumerator Start()
    {
        Vector3 originalScale = transform.localScale;

        yield return new WaitForSeconds(delay);

        while (true)
        {
            yield return new WaitForSeconds(loop);

            float timer = 0f;
            while (timer < animationTime)
            {
                float t = timer / animationTime;
                t = Mathf.SmoothStep(0f, 1f, t);
                float scale = Mathf.Lerp(1f, maxScale, t);
                transform.localScale = originalScale * scale;
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0f;
            while (timer < animationTime)
            {
                float t = timer / animationTime;
                t = Mathf.SmoothStep(0f, 1f, t);
                float scale = Mathf.Lerp(maxScale, 1f, t);
                transform.localScale = originalScale * scale;
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
