using System;
using System.Collections.Generic;

// Estado do jogo
public class GameState
{
    // Dicionário de jogadores
    public Dictionary<string, Player> players { get; set; }

    public GameState() {
        players = new Dictionary<string, Player>();
    }

    // FAZER O TOSTRING MOSTRANDO TODOS OS JOGADORES (Debug)
    public override string ToString()
    {
        string str = "";
        foreach (var playerKey in players.Keys)
        {
            Player player = players[playerKey];
            str += player.id + "\n";
        }
        return str;
    }
}

