using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PosData", order = 1)]

public class PosData : ScriptableObject
{
    public float[] XAxis;
    public float[] YAxis;
}