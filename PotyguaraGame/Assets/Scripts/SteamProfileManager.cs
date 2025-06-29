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
        //if (!SteamManager.Initialized) // Verifica se a Steam está inicializada
        //    return;

        // Obtém a foto de perfil (avatar)
        //GetSteamAvatar(SteamUser.GetSteamID());

        // obtem o nickname
        //FindFirstObjectByType<PotyPlayerController>().playerData.name = SteamFriends.GetPersonaName();
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

    /*void GetSteamAvatar(CSteamID steamID)
    {
        int imageID = SteamFriends.GetLargeFriendAvatar(steamID); // Obtém o ID da imagem

        if (imageID == -1)
        {
            Debug.LogError("Avatar ainda não está carregado!");
            return;
        }

        // Obtém o tamanho do avatar
        uint width, height;
        if (!SteamUtils.GetImageSize(imageID, out width, out height))
        {
            Debug.LogError("Erro ao obter tamanho do avatar!");
            return;
        }

        // Cria um array de bytes para armazenar os pixels da imagem
        byte[] imageData = new byte[width * height * 4];

        if (!SteamUtils.GetImageRGBA(imageID, imageData, imageData.Length))
        {
            Debug.LogError("Erro ao obter imagem do avatar!");
            return;
        }

        // Converte os bytes para uma textura
        Texture2D avatarTexture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        avatarTexture.LoadRawTextureData(imageData);
        avatarTexture.Apply();

        avatarTexture = FlipTextureVertically(avatarTexture);

        // Exibe a textura na UI
        avatarImage.texture = avatarTexture;
    }*/

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
