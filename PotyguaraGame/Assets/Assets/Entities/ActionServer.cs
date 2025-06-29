using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Classe que representa uma ação que pode ser executada por um jogador
/// E enviada para o servidor
/// </summary>
public class ActionServer
{
    // Tipo da ação
    public string type;

    // Id do jogador que executou a ação
    public string actor;

    // Id do jogador(ou outra coisa) que é o alvo da ação
    public string target;

    // Parâmetros da ação (pode ser qualquer coisa armazenada em um dicionário)
    public Dictionary<string, string> parameters;

    /// <summary>
    /// Conversão de um json para um objeto Action
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static ActionServer FromJson(string json) {
        ActionServer action = JsonUtility.FromJson<ActionServer>(json);
        return action;
    }

    /// <summary>
    /// Conversão de um objeto Action para um json
    /// </summary>
    /// <returns></returns>
    public string ToJson() {
        return JsonConvert.SerializeObject(this);
        // return JsonUtility.ToJson(this);
    }
}
