using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WaypointsFree;
using static UnityEngine.Rendering.DebugUI;
using Button = UnityEngine.UI.Button;

public class SpawnerController : MonoBehaviour
{
    [Header("Normal Mode")]
    public GameObject prefabNavio;
    public WaypointsGroup waypointsGroup;
    public GameObject cannons;

    [Header("Zombie Mode")]
    public GameObject prefabZumbi;
    public Transform destinyLevel1;
    public Transform destinyLevel2;
    public Transform slot;
    private List<Transform> spawnRandowZombie = new List<Transform>();

    [Header("General")]
    private bool levelIsRunning = false;
    private GameObject player;
    private GameObject finishUI;
    private int currentAmount;
    private int currentLevel = 1;

    private void Start()
    {
        GameForteController.Instance.SetCurrentLevel(currentLevel);
        player = GameObject.FindWithTag("Player");
        finishUI = GameObject.FindWithTag("MainCamera").transform.GetChild(0).GetChild(0).gameObject;

        for (var ii = 0; ii < destinyLevel1.childCount; ii++)
            spawnRandowZombie.Add(destinyLevel1.GetChild(ii));
    }

    #region Levels
    public void SetLevelIsRunning(bool value)
    {
        levelIsRunning = value;
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void SetDestinyRandow(int value)
    {
        spawnRandowZombie.Clear();
        if (value == 2)
        {
            for (var ii = 0; ii < destinyLevel2.childCount; ii++)
                spawnRandowZombie.Add(destinyLevel2.GetChild(ii));
        }
        else
        {
            for (var ii = 0; ii < destinyLevel1.childCount; ii++)
                spawnRandowZombie.Add(destinyLevel1.GetChild(ii));
        }
    }

    public void SetLevelZombieMode()
    {
        finishUI.SetActive(false);
        if (currentLevel == 1)
        {
            SetDestinyRandow(1);
            GameForteController.Instance.ChangeStateWalls(true);
            FindFirstObjectByType<HeightController>().NewHeight(8.07f);
            if (NetworkManager.Instance.modeTutorialOn)
            {
                AudioSource audio = FindFirstObjectByType<TechGuaraController>().SelectReport("Techyguara.Apresenta��oZombieMode");
                FindFirstObjectByType<TechGuaraController>().CreateReport("Zumbis a Vista!!!", "Em uma hist�ria misteriosa que voc� descobrir� em um futuro distante, os zumbis tomaram" +
                    " conta da cidade do Natal, e o �ltimo ref�gio da humanidade � a Fortaleza dos Reis Magos. Defenda a fortaleza contra hordas de zumbis famintos pela sobreviv�ncia!", 
                    audio.clip.length, new Vector3(752.74f, 11.35f, 400.29f), 90f);
                audio.Play();
            }
            NextLevel(90f, new Vector3(745.6f, 8.07f, 400.4f));
        }
        if (currentLevel == 2)
        {
            cannons.SetActive(false);
            GameForteController.Instance.handMenuLevel2.SetActive(true);
            GameForteController.Instance.handMenuLevel2.GetComponent<FadeController>().FadeIn();
            SetDestinyRandow(2);
            GameForteController.Instance.ResetCount();
            FindFirstObjectByType<HeightController>().NewHeight(17.25f);
            UpdateLevelBar();
            NextLevel(90f, new Vector3(659f, 17.25f, 401f));
        }
    }

    public void SetLevelNormalMode()
    {
        finishUI.SetActive(false);
        cannons.SetActive(true);
        if (NetworkManager.Instance.modeTutorialOn)
        {
            AudioSource audio = FindFirstObjectByType<TechGuaraController>().SelectReport("Techyguara.Apresenta��oBatalhadoForte");
            FindFirstObjectByType<TechGuaraController>().CreateReport("Defenda o Forte dos Invasores Maritimos!!!", "Na Batalha do Forte, voc� ser� um defensor da fortaleza durante uma �poca de invas�es no s�culo 15. Escolha um" +
                " canh�o e lute para proteger a fortaleza a todo custo! Prepare-se para desafios intensos enquanto vive momentos de pura " +
                "estrat�gia e a��o.", audio.clip.length, new Vector3(665f, 20.44f, 400.36f), 90f);
            audio.Play();
        }
        FindFirstObjectByType<HeightController>().NewHeight(17.19f);
        NextLevel(90f, new Vector3(659f, 17.25f, 401f));
        SetSpawn();
    }

    public void NextLevel(float angulationY, Vector3 initialPosition)
    {
        player.transform.position = initialPosition;
        player.transform.eulerAngles = new Vector3(0, angulationY, 0);
    }

    public void SendForRanking(int mode)
    {
        NetworkManager.Instance.SendPontuacionForte(GameForteController.Instance.GetTotalPoints(), mode);
        FindFirstObjectByType<RankingController>().gameObject.GetComponent<FadeController>().FadeIn();
    }

    private void UpdateLevelBar()
    {
        GameObject.FindWithTag("Level").GetComponent<Image>().fillAmount = 0.5f * currentLevel;
        GameObject.FindWithTag("Level").transform.GetChild(3).GetComponent<Text>().text = currentLevel+"";
    }
    #endregion

    private void Update()
    {
        if (levelIsRunning)
        {
            if (GameForteController.Instance.GetMode() == 1)
            {
                Transform timer = GameObject.FindWithTag("MainCamera").transform.GetChild(0).GetChild(2);
                timer.gameObject.SetActive(true);
                if (timer.GetChild(0).GetComponent<Text>().text == "0")
                {
                    foreach (Transform enemy in slot)
                        Destroy(enemy.gameObject);

                    timer.gameObject.SetActive(false);
                    timer.GetComponent<Image>().fillAmount = 1f;
                    levelIsRunning = false;
                    finishUI.transform.GetChild(1).GetComponent<Text>().text = "Parab�ns!!!";
                    finishUI.transform.GetChild(3).gameObject.SetActive(false);
                    finishUI.transform.GetChild(6).gameObject.SetActive(true);
                    finishUI.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(GameForteController.Instance.ResetGame);

                    finishUI.transform.GetChild(5).GetComponent<Text>().text = GameForteController.Instance.GetCurrrentScore() + "";
                    GameForteController.Instance.SetTotalPoints();
                    finishUI.SetActive(true);
                    finishUI.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                    SendForRanking(1);
                }
                else
                {
                    if (slot.childCount == 0)
                    {
                        FindFirstObjectByType<SpawnerController>().SetSpawn();
                    }
                }
            }
            else
            {
                Transform timer = GameObject.FindWithTag("MainCamera").transform.GetChild(0).GetChild(2);
                timer.gameObject.SetActive(true);
                if (timer.GetChild(0).GetComponent<Text>().text == "0")
                {
                    levelIsRunning = false;

                    finishUI.transform.GetChild(1).GetComponent<Text>().text = "Parab�ns!!!";

                    if (slot.childCount > 0)
                        foreach (Transform enemy in slot)
                            Destroy(enemy.gameObject);

                    timer.gameObject.SetActive(false);
                    timer.GetComponent<Image>().fillAmount = 1f;

                    if (currentLevel == 1)
                    {
                        currentLevel++;
                        GameForteController.Instance.SetCurrentLevel(currentLevel);
                        GameForteController.Instance.ChangeStateWalls(false);
                        finishUI.transform.GetChild(5).GetComponent<Text>().text = GameForteController.Instance.GetCurrrentScore() + "";
                        GameForteController.Instance.SetTotalPoints();
                        finishUI.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = "Proximo Nivel";
                        finishUI.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(GameForteController.Instance.NextLevel);
                    }
                    else if (currentLevel == 2)
                    {
                        finishUI.transform.GetChild(3).gameObject.SetActive(false);
                        finishUI.transform.GetChild(6).gameObject.SetActive(true);
                        finishUI.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(GameForteController.Instance.ResetGame);

                        currentLevel = 1;
                        GameForteController.Instance.SetCurrentLevel(currentLevel);
                        GameObject.FindWithTag("Level").transform.GetChild(3).GetComponent<Text>().text = currentLevel + "";
                        finishUI.transform.GetChild(5).GetComponent<Text>().text = GameForteController.Instance.GetCurrrentScore() + "";
                        GameObject.FindWithTag("Level").SetActive(false);
                        GameForteController.Instance.SetTotalPoints();
                        SendForRanking(0);
                    }

                    finishUI.SetActive(true);
                    finishUI.transform.GetChild(2).GetComponent<ParticleSystem>().Play();
                }
                else
                {
                    if (slot.childCount == 0)
                    {
                        FindFirstObjectByType<SpawnerController>().SetSpawn();
                    }
                }
            }
        }
    }

    public void CleanSlot()
    {
        foreach (Transform enemy in slot)
            Destroy(enemy.gameObject);
    }

    #region Spawners
    public void SetSpawn()
    {
        levelIsRunning = true;
        if (GameForteController.Instance.GetMode() == 0)
        {
            if (currentLevel == 1)
            {
                currentAmount = 12;
                InitSpawner(spawnRandowZombie);
            }
            if (currentLevel == 2)
            {
                currentAmount = 9;
                InitSpawner(spawnRandowZombie);
            }
        }
        else
        {
            currentAmount = 4;
            InitSpawner(waypointsGroup.waypoints);
        }
    }

    private void InitSpawner(List<Waypoint> waypoints)
    {
        for (int ii = 0; ii < currentAmount; ii++)
        {
            int numInt = Random.Range(0, waypoints.Count - 1);
            GameObject navio = Instantiate(prefabNavio, waypoints[numInt].position, Quaternion.identity, slot);
            if(ii==0)
                navio.GetComponent<WaypointsTraveler>().StartIndex = ii;
            else
                navio.GetComponent<WaypointsTraveler>().StartIndex = ii+1 > waypoints.Count-1 ? waypoints.Count-1 : ii+1;
        }
    }

    private void InitSpawner(List<Transform> points) {
        for (int ii = 0; ii < currentAmount; ii++)
        {
            int numInt = Random.Range(0, points.Count-1);
            Instantiate(prefabZumbi, points[numInt].position, Quaternion.identity, slot);
        }
    }
    #endregion
}
