using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private GameObject[] lights;
    // Start is called before the first frame update
    void Start()
    {
        lights = GameObject.FindGameObjectsWithTag("light");
    }

    // Update is called once per frame
    void Update()
    {
        if (FindFirstObjectByType<DayController>() != null)
        {
            if (NetworkManager.Instance.modeWeatherOn)
            {
                if (FindFirstObjectByType<DayController>().GetCurrentTime().Hour >= 18)
                {
                    foreach (var light in lights)
                        light.GetComponent<Light>().enabled = true;
                }
                else if (FindFirstObjectByType<DayController>().GetCurrentTime().Hour > 5 && FindFirstObjectByType<DayController>().GetCurrentTime().Hour < 18)
                {
                    foreach (var light in lights)
                        light.GetComponent<Light>().enabled = false;
                }
                else
                { // <= 5 horas
                    foreach (var light in lights)
                        light.GetComponent<Light>().enabled = true;
                }
            }
            else
            {
                foreach (var light in lights)
                    light.GetComponent<Light>().enabled = false;
            }
        }
    }
}
