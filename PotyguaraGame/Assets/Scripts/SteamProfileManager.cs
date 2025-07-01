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
        UpdatePotycoins(FindFirstObjectByType<PotyPlayerController>().GetPotycoins());
    }

    public void UpdatePotycoins(int value)
    {
        qnt.text = value.ToString();
    }

    public void ShootAlert()
    {
        transform.GetChild(4).GetComponent<FadeController>().FadeInForFadeOut(2f);
    }

    Texture2D FlipTextureVertically(Texture2D original)
    {
        Texture2D flipped = new Texture2D(original.width, original.height);

        for (int i = 0; i < original.height; i++)
        {
            flipped.SetPixels(0, i, original.width, 1, original.GetPixels(0, original.height - i - 1, original.width, 1));
        }

        flipped.Apply();
        return flipped;
    }
}
