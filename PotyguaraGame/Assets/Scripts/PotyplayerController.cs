using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PotyPlayerController : MonoBehaviour
{
    public Player playerData = new Player();

    private string positionRankingZombieMode = "N/A";
    private string positionRankingNormalMode = "N/A";
    private string scoreNormalMode = "";
    private string scoreZombieMode = "";
   

    public float updateServerTimesPerSecond = 10;

    //  Singleton stuff
    public static PotyPlayerController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        SetSkin(-1, -1, -1);
    }

    public void AddSkin(int value)
    {
        if (GetGender() == 0)
        {
            SkinUser newSkin = new SkinUser();
            newSkin.gender = 0;
            newSkin.index = value;
            newSkin.material = 0; // Default material, can be changed later

            playerData.skinsMASC.Add(newSkin);
        }
        else
        {
            SkinUser newSkin = new SkinUser();
            newSkin.gender = 0;
            newSkin.index = value;
            newSkin.material = 0; // Default material, can be changed later

            playerData.skinsFEM.Add(newSkin);
        }
    }
    public void ResetSkins() {
        if (GetGender() == 0)
            playerData.skinsMASC.Clear();
        else
            playerData.skinsFEM.Clear();
    }
    public bool VerifSkins(int index)
    {
        if (GetGender() == 0)
        {
            foreach (SkinUser skin in playerData.skinsMASC)
            {
                return skin.index == index ? true : false;
            }
        }
        else
        {
            foreach (SkinUser skin in playerData.skinsFEM)
            {
                return skin.index == index ? true : false;
            }
        }
        return false;
    }

    public void AddTicket(string ticket) {  playerData.tickets.Add(ticket); }
    public List<string> GetTickets() {  return playerData.tickets; }
    public bool VerifTickets(string id) { return playerData.tickets.Contains(id) ? true : false; }

    public void AddSession(string session) { playerData.sessions.Add(session); }
    public List<string> GetSessions() { return playerData.sessions; }
    public bool VerifSessions(string id) { return playerData.sessions.Contains(id) ? true : false; }

    public void HideControllers()
    {
        Transform left = FindFirstObjectByType<LeftHandController>().gameObject.transform;
        left.GetChild(1).gameObject.SetActive(false);
        left.GetChild(2).gameObject.SetActive(false);

        Transform right = FindFirstObjectByType<RightHandController>().gameObject.transform;
        right.GetChild(1).gameObject.SetActive(false);
        right.GetChild(2).gameObject.SetActive(false);

        GameObject.FindWithTag("Player").transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ShowControllers()
    {
        Transform left = FindFirstObjectByType<LeftHandController>().gameObject.transform;
        left.GetChild(1).gameObject.SetActive(true);
        left.GetChild(2).gameObject.SetActive(true);

        Transform right = FindFirstObjectByType<RightHandController>().gameObject.transform;
        right.GetChild(1).gameObject.SetActive(true);
        right.GetChild(2).gameObject.SetActive(true);

        GameObject.FindWithTag("Player").transform.GetChild(1).gameObject.SetActive(true);

        if (FindFirstObjectByType<LiftShowController>().GetStatus() == 1)
        {
            FindFirstObjectByType<MenuShowController>().showLiberated = true;
            GameObject locomotion = GameObject.Find("Locomotion").transform.GetChild(1).gameObject;
            if (locomotion != null)
                locomotion.SetActive(false);
        }
        else
        {
            Destroy(GameObject.Find("Dragon(Clone)"));
            Destroy(GameObject.Find("Guitaura(Clone)"));
            FindFirstObjectByType<LiftShowController>().hasTicket = false;
            GameObject locomotion = GameObject.Find("Locomotion").transform.GetChild(1).gameObject;
            if (locomotion != null)
                locomotion.SetActive(true);
        }
    }

    public void SetSkin(int skinGender, int skinIndex, int skinMaterial)
    {
        playerData.skin.gender = skinGender;
        playerData.skin.index = skinIndex; 
        playerData.skin.material = skinMaterial;   
    }

    public void SetGender(int gender)
    {
        playerData.skin.gender = gender;
    }

    public int GetIndex() { return playerData.skin.index; }
    public int GetGender() { return playerData.skin.gender; }
    public int GetMaterial() { return playerData.skin.material; }

    public void GetPotycoinsOfTheServer(int value)
    {
        playerData.potycoins = value;
    }

    public int GetPotycoins()
    {
        return playerData.potycoins;
    }

    public void SetScoreNormalMode(string value) { scoreNormalMode = value; }
    public void SetScoreZombieMode(string value){  scoreZombieMode = value; }

    public int GetScoreNormalMode()
    {
        return int.Parse(scoreNormalMode);
    }

    public int GetScoreZombieMode()
    {
        return int.Parse(scoreZombieMode);
    }

    public string GetScoreZombie(){
        string formatedScore = playerData.name + ": " + scoreZombieMode + "pt";
        return formatedScore;
    }
    public string GetScoreNormal() {
        string formatedScore = playerData.name + ": " + scoreNormalMode + "pt";
        return formatedScore;
    }

    public void SetPositionRanking(string value, int mode)
    {
        if (mode == 0)
            positionRankingZombieMode = value.Equals("0") ? "N/A" : value;
        else
            positionRankingNormalMode = value.Equals("0") ? "N/A" : value;
    }
    public string GetPositionZombie() { return positionRankingZombieMode; }
    public string GetPositionNormal() { return positionRankingNormalMode; }
}
