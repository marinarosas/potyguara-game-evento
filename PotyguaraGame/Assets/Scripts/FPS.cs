using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPS : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        InvokeRepeating(nameof(CalculateFPS), 0, 1f);
    }

    void CalculateFPS()
    {
        text.text = (1f / Time.deltaTime).ToString("00") + " FPS";
    }
}
