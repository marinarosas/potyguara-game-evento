using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SteamProfileManager : MonoBehaviour
{
    public TextMeshProUGUI qnt;
    public RawImage avatarImage; // Referência para exibir a foto de perfil
    public Sprite defaultImage;

    // Start is called before the first frame update
    void Start()
    {
        avatarImage.texture = defaultImage.texture;
        FindFirstObjectByType<PotyPlayerController>().playerData.name = "PotyguaraVerse";
    }
    void OnEnable()
    {
        UpdatePotycoins("♾️");
    }

    public void UpdatePotycoins(string value)
    {
        qnt.text = value;
    }

    public void ShootAlert()
    {
        transform.GetChild(4).GetComponent<FadeController>().FadeInForFadeOut(2f);
    }
}
