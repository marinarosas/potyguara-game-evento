using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DayController : MonoBehaviour
{
    private string apiKey = "b88f7145e1044e2cbed01332251701";
    private string city = "Natal";
    private string url;
    public float precip_mm;    
    public int clouds;        

    private DateTime currentTime;
    private Transform sun;
    private Transform moon;
    private Material skyBox;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 5)
        {
            moon = GameObject.FindWithTag("Moon").transform;
            sun = GameObject.FindWithTag("Sun").transform;
        }
        skyBox = RenderSettings.skybox;
        url = $"http://api.weatherapi.com/v1/current.json?key={apiKey}&q={city}&aqi=no";
        rotationSpeed = 360f / dayLenght;
    }

    void RequestAPIWeather()
    {
        // Faz a requisição à API
        StartCoroutine(GetWeatherData());
    }

    float dayLenght = 86400f;
    float rotationSpeed;

    public float GetPrecip()
    {
        return precip_mm;
    }

    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    IEnumerator GetWeatherData()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            // Verifica se houve algum erro
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse do JSON retornado
                string jsonResponse = request.downloadHandler.text;
                WeatherResponse weather = JsonUtility.FromJson<WeatherResponse>(jsonResponse);

                precip_mm = weather.current.precip_mm;
                clouds = weather.current.cloud;
            }
            else
            {
                Debug.LogError("Erro na requisição: " + request.error);
            }
        }
    }

    // Definição da classe para parse do JSON
    [Serializable]
    public class WeatherResponse
    {
        public CurrentWeather current;
    }

    [Serializable]
    public class CurrentWeather
    {
        public float precip_mm;      // precipitação em milimetros
        public int cloud;         // Cobertura de nuvens (%)
    }

    void Update()
    {
        currentTime = DateTime.Now.ToLocalTime();
        //GetComponent<TextMeshProUGUI>().text = currentTime.ToString("HH:mm:ss");

        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 5)
        {
            if (NetworkManager.Instance.modeWeatherOn)
            {
                InvokeRepeating("RequestAPIWeather", 0f, 3600f);

                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    RenderSettings.sun = sun.GetComponent<Light>();
                    sun.rotation = Quaternion.Euler(164f, 0f, 0f);
                    skyBox.SetFloat("_AtmosphereThickness", 0.8f);
                    return;
                }

                if (sun != null && moon != null)
                {
                    float hours = currentTime.Hour + (currentTime.Minute / 60f) + (currentTime.Second / 3600f);
                    float sunAngle = (hours / 24f) * 360f;

                    if (currentTime.Hour >= 18)
                    {
                        rotationSpeed = 180f / dayLenght;
                        sunAngle = (hours / 24f) * 180f;
                        moon.GetComponent<Light>().enabled = true;
                        RenderSettings.sun = moon.GetComponent<Light>();
                        moon.rotation = Quaternion.Euler(137f, 95f, 0f);
                        skyBox.SetFloat("_AtmosphereThickness", 0.2f);
                    }
                    if (currentTime.Hour >= 5 && currentTime.Hour < 18)
                    {
                        rotationSpeed = 360f / dayLenght;
                        sun.GetComponent<Light>().enabled = true;
                        RenderSettings.sun = sun.GetComponent<Light>();
                        sun.rotation = Quaternion.Euler(137f, 95f, 0f);
                        skyBox.SetFloat("_AtmosphereThickness", 0.8f);
                    }
                    if (currentTime.Hour < 5)
                    {
                        rotationSpeed = 180f / dayLenght;
                        sunAngle = (hours / 24f) * 180f;
                        moon.GetComponent<Light>().enabled = true;
                        RenderSettings.sun = moon.GetComponent<Light>();
                        moon.rotation = Quaternion.Euler(137f, 95f, 0f);
                        skyBox.SetFloat("_AtmosphereThickness", 0.2f);
                    }
                }
            }
            else
            {
                RenderSettings.sun = sun.GetComponent<Light>();
                sun.rotation = Quaternion.Euler(164f, 0f, 0f);
                skyBox.SetFloat("_AtmosphereThickness", 0.8f);
            }
        }
    }
}
