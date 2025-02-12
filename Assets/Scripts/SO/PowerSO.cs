using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Power")]
public class PowerSO : TileSO
{
    public List<Vector2Int> BlastDirections;
    public int BlastRadius;
}