using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : ScriptableObject
{
    public float growth = 1;
    public int max = 50;
    public int split = 30;
    public float commitFactor = 0.5f;
    public float transfer = 0.2f;
    public int attack = 10;
    public float attackFac = 1;
    public float attackCommit = 0.1f;
    public float defendFactor = 1;
}
