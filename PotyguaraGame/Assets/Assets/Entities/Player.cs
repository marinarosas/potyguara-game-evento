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

    public List<int> skinsMASC = new List<int>();
    public List<int> skinsFEM = new List<int>();
    public List<string> tickets = new List<string>();
    public List<string> sessions = new List<string>();

    public SkinUser skin { get; set; } = new SkinUser();

    public float position_x { get; set; }
    public float position_y { get; set; }
    public float position_z { get; set; }
}
