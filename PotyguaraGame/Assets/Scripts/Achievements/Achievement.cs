using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    public static Achievement Instance;

    public int zombies = 0;
    public int eventos = 0;
    public int ships_levas = 0;
    public int partidas_hover = 0;
    public int partidas_defesaForte = 0;
    public bool firstCompleteEvent = false;

    public bool isFirstShip = true;
    public bool isFirstDeadInZombieMode = true;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private void Start()
    {
        partidas_defesaForte = GetStat("partidas_defesaForte");
        partidas_hover = GetStat("partidas_hover");
        ships_levas = GetStat("navios_levas");
        zombies = GetStat("zombies");
        eventos = GetStat("eventos");
    }*/

    private void Update()
    {
        if(firstCompleteEvent)
        {
            UnclockAchievement("espectador");
            firstCompleteEvent = false;
        }
        if(partidas_defesaForte >= 1 && partidas_hover >= 1 && zombies >= 1)
        {
            UnclockAchievement("Inside_gamer");
        }
    }

    public void ResetAchievement()
    {
        /*if (!SteamManager.Initialized)
            return;*/

        //SteamUserStats.ResetAllStats(true);
        //SteamUserStats.RequestCurrentStats();

        zombies = 0;
        eventos = 0;
        ships_levas = 0;
        partidas_hover = 0;
        partidas_defesaForte = 0;
        firstCompleteEvent = false;

        isFirstShip = true;
        isFirstDeadInZombieMode = true;
    }

    public void UnclockAchievement(string id)
    {
        /*if(!SteamManager.Initialized)
            return;*/

        //SteamUserStats.SetAchievement(id);
        //SteamUserStats.StoreStats();
    }

    public void SetStat(string id, int value)
    {
        if(!SteamManager.Initialized) return;

        //SteamUserStats.SetStat(id, value);
        //SteamUserStats.StoreStats();
    }

    /*public int GetStat(string id)
    {
        if (!SteamManager.Initialized)
            return 0;

        //SteamUserStats.GetStat(id, out int qnt);
        //return qnt;
    }*/
}
