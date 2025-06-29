using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using System.Collections.Concurrent;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.Analytics;
using NodaTime;
using NodaTime.TimeZones;

public class NetworkManager : MonoBehaviour
{
    // Prefab do jogador local
    public GameObject LocalPlayerPrefab;

    // Prefab do jogador remoto
    public GameObject RemotePlayerPrefab;

    // Endereço do servidor
    public string serverAddress = "wss://potyws.ffcloud.com.br";

    // WebSocket para comunicação com o servidor
    private WebSocket ws;

    private string rankingZ = "";
    private string rankingN = "";
    private string posRankingZ = "";
    private string posRankingN = "";

    public bool newDay = true;
    public bool isTheFirstAcess = true;
    public bool modeTutorialOn = true;
    public bool modeWeatherOn = true;
    public bool firstInPN = false;
    public bool firstInHover = false;
    public bool firstInForte = false;

    private ConcurrentQueue<int> potycoins = new ConcurrentQueue<int>();
    private ConcurrentQueue<string> pointingNormalMode = new ConcurrentQueue<string>();
    private ConcurrentQueue<string> pointingZombieMode = new ConcurrentQueue<string>();
    private ConcurrentQueue<string> skin = new ConcurrentQueue<string>();
    private ConcurrentQueue<int> skinsFEM = new ConcurrentQueue<int>();
    private ConcurrentQueue<int> skinsMAL = new ConcurrentQueue<int>();
    private ConcurrentQueue<string> tickets = new ConcurrentQueue<string>();
    private ConcurrentQueue<string> sessions = new ConcurrentQueue<string>();

    //  Singleton stuff
    private static NetworkManager _instance;

    public static NetworkManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("NetworkManager");
                    _instance = obj.AddComponent<NetworkManager>();
                }
            }
            return _instance;
        }
    }

    // Awake é chamado antes do Start, e é chamado apenas uma vez
    // conecta ao servidor, e mantém a conexão aberta, assim como não destrói o objeto ao mudar de cena
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        ConnectToServer();
    }
     
    // Id do jogador local. É definido pelo servidor após a conexão com o evento "Wellcome"
    public string playerId;
    
    // Estado do jogo (posições dos jogadores, etc)
    public GameState gameState;

    // Conecta ao servidor e define os eventos de recebimento de mensagens
    void ConnectToServer() {
        Debug.Log("conectando");
        
        ws = new WebSocket(this.serverAddress);
        ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;

        // OnMessage é chamado sempre que uma mensagem é recebida
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message received: " + e.Data);
            // Processar a mensagem recebida
            ProcessServerMessage(e.Data);
        };

        // Depois de definir os eventos, conectar ao servidor
        ws.Connect();

        /*if (!SteamManager.Initialized) // Verifica se a Steam está inicializada
            return;*/

        string windowsTimeZone = TimeZoneInfo.Local.Id;
        var ianaTimeZone = GetIanaTimeZone(windowsTimeZone);

            SendConnectionSignal("PotyguaraVerse", ianaTimeZone);
            //SendConnectionSignal(SteamFriends.GetPersonaName(), ianaTimeZone);
    }

    private string GetIanaTimeZone(string windowsTimeZone)
    {
        var tzdbSource = TzdbDateTimeZoneSource.Default;
        var mappings = tzdbSource.WindowsToTzdbIds;

        if (mappings.TryGetValue(windowsTimeZone, out var ianaZone))
        {
            return ianaZone;
        }

        return "Unknown";
    }

    NetworkManager() {
        gameState = new GameState();
    }
    
    /// <summary>
    /// Processa a mensagem recebida do servidor
    /// </summary>
    /// <param name="message"></param>
    private void ProcessServerMessage(string message)
    {
        Debug.Log("Processing message from server");
        try {
            // converter a mensagem recebida(JSON) para um objeto ServerResponse
            var response = JsonConvert.DeserializeObject<ServerResponse>(message);
            Debug.Log("response: " + response);
            Debug.Log("response.type: " + response.type);
            Debug.Log("response.parameters: " + response.parameters);
            Debug.Log("response.gameState: " + response.gameState);

            // verificar o tipo da mensagem recebida
            switch (response.type)
            {
                case "Wellcome":
                    // Se for uma mensagem "Wellcome", o servidor enviou o id do jogador que será usado para
                    // identificar o jogador local. Esse id é gerado pelo servidor.
                    Debug.Log("::: WELCOME RECEIVED" + response.parameters);
                    this.playerId = response.parameters["playerId"];
                    SendNewDay();
                    break;
                case "GameState":
                    // Aqui o servidor enviou o estado atual do jogo, com as posições dos jogadores
                    Debug.Log(":: GAMESTATE RECEIVED:: ");
                    Debug.Log(response.gameState);
                    // Note que aqui não estamos atualizando o gameState, mas sim substituindo o objeto
                    // pelo novo estado do jogo. Isso é feito para que o objeto gameState seja sempre o
                    // estado atual do jogo.
                    // Entretanto, isso pode causar problemas se houver alguma referência ao objeto gameState
                    // Então, é importante sempre usar o gameState para acessar o estado do jogo, e não manter
                    // referências ao objeto gameState que pode ser substituído a qualquer momento.
                    // Note também que não estamos trocando a posição dos jogadores, mas sim atualizando
                    // o objeto gameState com as novas posições dos jogadores.          
                    // A mudança de posição dos jogadores é feita no método Update() que irá consultar o
                    // gameState para saber a posição dos jogadores e atualizar a posição dos objetos na cena.
                    gameState = response.gameState;
                    break;
                case "Ranking":
                    rankingZ = response.parameters["rankingZ"];
                    rankingN = response.parameters["rankingN"];
                    posRankingN = response.parameters["posRankingN"];
                    posRankingZ = response.parameters["posRankingZ"];
                    break;
                case "Skins":
                    string skinsStringFEM = response.parameters["skinsFEM"];
                    if (skinsStringFEM.Contains("|"))
                    {
                        string[] indexList = skinsStringFEM.Split('|');
                        foreach (var index in indexList)
                            skinsFEM.Enqueue(int.Parse(index));
                    }
                    else
                        skinsFEM.Enqueue(0);

                    string skinsStringMAL = response.parameters["skinsMAL"];
                    if (skinsStringMAL.Contains("|"))
                    {
                        string[] indexList = skinsStringMAL.Split('|');
                        foreach (var index in indexList)
                            skinsMAL.Enqueue(int.Parse(index));
                    }
                    else
                        skinsMAL.Enqueue(0);
                    break;
                case "Reconnection":
                    this.playerId = response.parameters["playerID"];
                    string skinS = response.parameters["skin"];
                    string[] list = skinS.Split('|');

                    if (int.Parse(list[0]) != -1)
                    {
                        SendNewDay();
                        isTheFirstAcess = false;

                        firstInPN = response.parameters["pnTutorial"].ToLower() == "true" ? true : false;
                        firstInHover = response.parameters["hoverTutorial"].ToLower() == "true" ? true : false;
                        firstInForte = response.parameters["forteTutorial"].ToLower() == "true" ? true : false;

                        modeTutorialOn = response.parameters["modeTutorial"].ToLower() == "true" ? true : false;
                        modeWeatherOn = response.parameters["modeWeather"].ToLower() == "true" ? true : false;

                        pointingNormalMode.Enqueue(response.parameters["pointingNormalMode"]);
                        pointingZombieMode.Enqueue(response.parameters["pointingZombieMode"]);
                        potycoins.Enqueue(int.Parse(response.parameters["potycoins"]));

                        string ticketsS = response.parameters["tickets"];
                        string[] ticketList = ticketsS.Split('|');

                        foreach (var ticket in ticketList)
                            tickets.Enqueue(ticket);

                        string sessionsS = response.parameters["sessions"];
                        string[] sessionList = sessionsS.Split('|');

                        foreach (var session in sessionList)
                            sessions.Enqueue(session);

                        skin.Enqueue(response.parameters["skin"]);
                    }
                    break;
                case "RewardCoins":
                    newDay = response.parameters["newDay"].ToLower() == "true" ? true : false;
                    break;
                default:
                    break;
            }
        } catch (Exception e) {
            Debug.Log("Erro ao deserializar: " + e);
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }

    /// <summary>
    /// Envia a posição do jogador para o servidor
    /// </summary>
    /// <param name="position"></param>
    internal void SendPosition(Vector3 position)
    {
        // Antes é necessário criar um objeto Action, que contém o tipo de ação, o ator e os parâmetros
        // esses parâmetros são um dicionário que contem a nova posição do jogador.
        // é necessário usar o InvariantCulture para garantir que o ponto seja usado como separador decimal
        ActionServer action = new ActionServer()
        {
            type = "PositionUpdate",
            actor = this.playerId,
            parameters = new Dictionary<string, string>(){
                {"position_x", position.x.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture)},
                {"position_y", position.y.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture)},
                {"position_z", position.z.ToString("0.000000", System.Globalization.CultureInfo.InvariantCulture)}
            }
        };

        // Enviar a ação para o servidor
        ws.Send(action.ToJson());
    }
    internal void SendNewDay()
    {
        ActionServer action = new ActionServer()
        {
            type = "isNewDay",
            actor = this.playerId,
            parameters = new Dictionary<string, string>(){
               
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendRewardCoins()
    {
        ActionServer action = new ActionServer()
        {
            type = "RewardCoins",
            actor = this.playerId,
            parameters = new Dictionary<string, string>()
            {

            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendModeTutorial(bool mode)
    {
        ActionServer action = new ActionServer()
        {
            type = "UpdateModeTutorial",
            actor = playerId,
            parameters = new Dictionary<string, string>(){
                { "mode", mode.ToString() }
            }
        };
        modeTutorialOn = mode;

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendModeWeather(bool mode)
    {
        ActionServer action = new ActionServer()
        {
            type = "UpdateModeWeather",
            actor = playerId,
            parameters = new Dictionary<string, string>(){
                { "mode", mode.ToString() }
            }
        };

        modeWeatherOn = mode;

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendConnectionSignal(string nickname, string fusoH)
    {
        ActionServer action = new ActionServer()
        {
            type = "Connection",
            actor = nickname,
            parameters = new Dictionary<string, string>(){
                { "serverAddress", serverAddress },
                { "fuso", fusoH }
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendSkin(int gender, int index, int material)
    {
        ActionServer action = new ActionServer()
        {
            type = "UpdateSkin",
            actor = playerId,
            parameters = new Dictionary<string, string>(){
                { "gender", gender.ToString() },
                { "index", index.ToString() },
                { "material", material.ToString() }
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendSkin(int index)
    {
        ActionServer action = new ActionServer()
        {
            type = "NewSkin",
            actor = playerId,
            parameters = new Dictionary<string, string>(){
                { "index", index.ToString() }
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }
    internal void RequestSkins()
    {
        ActionServer action = new ActionServer()
        {
            type = "RequestSkins",
            actor = playerId,
            parameters = new Dictionary<string, string>(){
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendSignalTutorialOK(string typeT)
    {
        ActionServer action = new ActionServer()
        {
            type = typeT,
            actor = playerId,
            parameters = new Dictionary<string, string>()
            {
            }
        };

        if (ws != null)
            // Enviar a ação para o servidor
            ws.Send(action.ToJson());
    }

    internal void SendPontuacionForte(int totalPoints, int mode)
    {
        if (mode == 0)
        {
            ActionServer action = new ActionServer()
            {
                type = "GameForteZ",
                actor = this.playerId,
                parameters = new Dictionary<string, string>()
                {
                    { "nickname", PotyPlayerController.Instance.playerData.name },
                    { "pointing", totalPoints.ToString() }
                }
            };

            if (ws != null)
                // envia a pontuação final no jogo do Forte para o servidor
                ws.Send(action.ToJson());
        }
        else
        {
            ActionServer action = new ActionServer()
            {
                type = "GameForteB",
                actor = this.playerId,
                parameters = new Dictionary<string, string>()
                {
                    { "nickname", PotyPlayerController.Instance.playerData.name },
                    { "pointing", totalPoints.ToString() }
                }
            };

            if (ws != null)
                // envia a pontuação final no jogo do Forte para o servidor
                ws.Send(action.ToJson());
        }
    }

    internal void SendTicket(string id)
    {
        ActionServer action = new ActionServer()
        {
            type = "NewTicket",
            actor = this.playerId,
            parameters = new Dictionary<string, string>()
            {
                { "id", id },
            }
        };
        /*Achievement.Instance.eventos++;
        if(Achievement.Instance.eventos == 1000)
        {
            Achievement.Instance.UnclockAchievement("potyverser");
        }
        Achievement.Instance.SetStat("eventos", Achievement.Instance.eventos);*/

        // solicita a atualização dos tickets para o servidor
        if (ws != null)
            ws.Send(action.ToJson());
    }

    internal void SendSession(string id)
    {
        ActionServer action = new ActionServer()
        {
            type = "NewSession",
            actor = this.playerId,
            parameters = new Dictionary<string, string>()
            {
                { "id", id },
            }
        };

        // solicita a atualização dos tickets para o servidor
        if (ws != null)
            ws.Send(action.ToJson());
    }

    internal void UpdatePotycoins(int potycoins)
    {
        ActionServer action = new ActionServer()
        {
            type = "UpdatePotycoins",
            actor = this.playerId,
            parameters = new Dictionary<string, string>()
            {
                {"potycoins", potycoins.ToString() },
            }
        };

        // envia a pontuação final no jogo do Forte para o servidor
        if(ws != null)
            ws.Send(action.ToJson());
    }

    internal void DeletePerfil()
    {
        ActionServer action = new ActionServer()
        {
            type = "DeletePerfil",
            actor = this.playerId,
            parameters = new Dictionary<string, string>()
            {

            }
        };
        if (ws != null)
        {
            ws.Send(action.ToJson());
            TransitionController.Instance.ExitGame();
        }
    }

    /// <summary>
    /// Atualiza a posição dos jogadores na cena, de acordo com o gameState.
    /// Se um jogador não existir na cena, ele é instanciado.
    /// Se um jogador existir na cena, sua posição é atualizada.
    /// </summary>
    void Update() {
        // A cada frame, verificar se precisa instanciar quantos RemotePlayerPrefab e
        // LocalPlayerPrefab forem necessários
        if (gameState != null) {

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                // Para cada jogador no gameState verificar se o jogador já existe na cena
                // Se não existir, instanciar um novo jogador
                // Se existir, atualizar a posição do jogador
                foreach (var playerId in gameState.players.Keys)
                {

                    // Se o jogador for o jogador local, não fazer nada
                    if (playerId == this.playerId)
                    {
                        //Debug.Log(playerId + " SOU EU!");
                        // é o jogador local
                        //TODO: talvez precisa atualiza a minha posição se isso puder acontecer
                        // com alguma ação do servidor.
                        continue;
                    }

                    // Buscar o jogador na cena pelo playerId
                    GameObject playerObject = GameObject.Find(playerId);


                    // Se o jogador não existir, instanciar um novo jogador
                    if (playerObject == null)
                    {
                        playerObject = Instantiate(RemotePlayerPrefab) as GameObject;
                        playerObject.transform.GetChild(0).GetComponent<SetSkin>().SetSkinAvatar(gameState.players[playerId].skin.gender, 
                            gameState.players[playerId].skin.index, gameState.players[playerId].skin.material);
                        playerObject.name = playerId;
                    }

                    // Atualizar a posição do jogador
                    // TODO: implementar interpolação de movimento
                    playerObject.transform.position = new Vector3(
                        gameState.players[playerId].position_x,
                        gameState.players[playerId].position_y,
                        gameState.players[playerId].position_z
                    );
                }
            }

            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                if (!rankingZ.Equals(""))
                {
                    FindObjectOfType<RankingController>().UpdateRanking(rankingZ, 0);
                    FindFirstObjectByType<PotyPlayerController>().SetPositionRanking(posRankingZ, 0);
                    posRankingZ = "";
                    rankingZ = "";
                }

                if (!rankingN.Equals(""))
                {
                    FindObjectOfType<RankingController>().UpdateRanking(rankingN, 1);
                    FindFirstObjectByType<PotyPlayerController>().SetPositionRanking(posRankingN, 1);
                    posRankingN = "";
                    rankingN = "";
                }
            }

            while (potycoins.TryDequeue(out int potycoin))
            {
                FindFirstObjectByType<PotyPlayerController>().GetPotycoinsOfTheServer(potycoin);
            }

            while (skin.TryDequeue(out string skinString))
            {
                string[] list = skinString.Split('|');
                int bodyIndex = int.Parse(list[0]);
                int skinIndex = int.Parse(list[1]);
                int variant = int.Parse(list[2]);

                FindFirstObjectByType<PotyPlayerController>().SetSkin(bodyIndex, skinIndex, variant);
            }

            while (skinsFEM.TryDequeue(out int skinF))
            {
                FindFirstObjectByType<PotyPlayerController>().AddSkin(skinF);
            }

            while (skinsMAL.TryDequeue(out int skinM))
            {
                FindFirstObjectByType<PotyPlayerController>().AddSkin(skinM);
            }

            while (tickets.TryDequeue(out string ticket))
            {
                FindFirstObjectByType<PotyPlayerController>().AddTicket(ticket);
            }

            while (sessions.TryDequeue(out string session))
            {
                FindFirstObjectByType<PotyPlayerController>().AddSession(session);
            }

            while (pointingNormalMode.TryDequeue(out string pointingNM))
            {
                FindFirstObjectByType<PotyPlayerController>().SetScoreNormalMode(pointingNM);
            }

            while (pointingZombieMode.TryDequeue(out string pointingZM))
            {
                FindFirstObjectByType<PotyPlayerController>().SetScoreZombieMode(pointingZM);
            }

            if (SceneManager.GetActiveScene().buildIndex == 0)
                TransitionController.Instance.UpdateMainMenu(isTheFirstAcess);
        }
    }
}
