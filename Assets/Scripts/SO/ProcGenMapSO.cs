using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProcGenRule
{
    public GenLogic GenLogic;
    public float Probability;
}

[CreateAssetMenu(fileName = "ProcGen map")]
public class ProcGenMapSO : ScriptableObject
{
    public List<ProcGenRule> procGenRules;
}