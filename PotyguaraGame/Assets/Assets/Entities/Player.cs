using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string id { get; set; }
    public string name { get; set; }

    public int potycoins { get; set; }
    public int pointingNormalMode { get; set; }
    public int pointingZombieMode { get; set; }

    public bool modeTutorialOn { get; set; }
    public bool modeWeatherOn { get; set; }
    public bool pnFirstTutorial { get; set; }
    public bool forteFirstTutorial { get; set; }
    public bool hoverFirstTutorial { get; set; }

    public List<SkinUser> skinsMASC = new List<SkinUser>();
    public List<SkinUser> skinsFEM = new List<SkinUser>();
    public List<string> tickets = new List<string>();
    public List<string> sessions = new List<string>();

    public SkinUser skin { get; set; } = new SkinUser();

    public float position_x { get; set; }
    public float position_y { get; set; }
    public float position_z { get; set; }
}
