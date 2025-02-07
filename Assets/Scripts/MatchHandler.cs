using UnityEngine;

public class MatchHandler : MonoBehaviour
{
    private Board _board;

    void Awake()
    {
        _board = GetComponent<Board>();
    }
    
    void OnEnable()
    {
        Tile.OnTileMoved += CheckMatch;
    }

    void OnDisable()
    {
        Tile.OnTileMoved -= CheckMatch;
    }
    
    private void CheckMatch(object sender, Vector2Int position)
    {
        
    }
}
