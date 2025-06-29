using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class RankingController : MonoBehaviour
{
    public void UpdateRanking(string ranking, int mode)
    {
        string rankingS = ranking;
        string[] playersRanking = rankingS.Split('|');

        Transform parent = transform.GetChild(mode == 0 ? 3 : 4);
        parent.GetChild(0).GetComponent<Text>().text = playersRanking[0] + "pt";

        for (int ii = 1; ii < 6; ii++)
        {
            if (ii < playersRanking.Length)
            {
                if (playersRanking[ii].Length > 1)
                    parent.GetChild(ii).GetComponent<Text>().text = playersRanking[ii] + "pt";
            }
            else
                parent.GetChild(ii).GetComponent<Text>().text = "- : -";
        }
        if(mode == 0)
        {
            if(FindFirstObjectByType<PotyPlayerController>().GetPositionZombie() == "N/A")
            {
                parent.GetChild(7).GetComponent<Text>().text = "N/A";
                parent.GetChild(8).GetComponent<Text>().text = "- : -";
            }
            else
            {
                int temp = int.Parse(FindFirstObjectByType<PotyPlayerController>().GetPositionZombie());
                if (temp >= 1 && temp <= 6)
                {
                    parent.GetChild(7).gameObject.SetActive(false);
                    parent.GetChild(8).gameObject.SetActive(false);
                }
                else
                {
                    parent.GetChild(7).gameObject.SetActive(true);
                    parent.GetChild(8).gameObject.SetActive(true);
                    parent.GetChild(7).GetComponent<Text>().text = FindFirstObjectByType<PotyPlayerController>().GetPositionZombie() + "º";
                    parent.GetChild(8).GetComponent<Text>().text = FindFirstObjectByType<PotyPlayerController>().GetScoreZombie();
                }
            }
        }
        else
        {
            if (FindFirstObjectByType<PotyPlayerController>().GetPositionNormal() == "N/A")
            {
                parent.GetChild(7).GetComponent<Text>().text = "N/A";
                parent.GetChild(8).GetComponent<Text>().text = "- : -";
            }
            else
            {
                int temp = int.Parse(FindFirstObjectByType<PotyPlayerController>().GetPositionNormal());
                if (temp == 1)
                    Achievement.Instance.UnclockAchievement("heroi_guerra");
                if (temp >= 1 && temp <= 6)
                {
                    parent.GetChild(7).gameObject.SetActive(false);
                    parent.GetChild(8).gameObject.SetActive(false);
                }
                else
                {
                    parent.GetChild(7).gameObject.SetActive(true);
                    parent.GetChild(8).gameObject.SetActive(true);
                    parent.GetChild(7).GetComponent<Text>().text = FindFirstObjectByType<PotyPlayerController>().GetPositionNormal() + "º";
                    parent.GetChild(8).GetComponent<Text>().text = FindFirstObjectByType<PotyPlayerController>().GetScoreNormal();
                }
            }
        }
    }
}
