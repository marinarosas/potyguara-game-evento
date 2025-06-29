using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Classe que representa uma resposta do servidor
/// </summary>
public class ServerResponse
{
    // Tipo da resposta
    public string type { get; set; }

    // Parâmetros da resposta (pode ser qualquer coisa armazenada em um dicionário)
    public Dictionary<string, string> parameters { get; set; }

    // Estado do jogo
    public GameState gameState { get; set; }

    ServerResponse()
    {
        parameters = new Dictionary<string, string>();
    }

    // Tostring mostrando tudo
    public override string ToString()
    {
        string str = "";
        str += "Type: " + type + "\n";
        // str += "GameState: " + gameState.ToString() + "\n";
        // str += "Parameters: " + parameters.ToString() + "\n";
        return str;
    }
}
