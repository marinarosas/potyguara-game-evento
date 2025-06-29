using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Report : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    public void UpdateTitle(string message)
    {
        title.text = message;
    }
    public void UpdateDescription(string message)
    {
        description.text = message;
    }
}
