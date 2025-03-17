using UnityEngine;

public class King : MonoBehaviour
{
    public string color;
    public Transform pieces;
    void Start()
    {
        //kingMove(0.5f, -2.5f);
        //kingMove(-0.5f, -3.5f);
        structure.s_move move = new structure.s_move("King", color, 7, 4, 6, 4);
        legalMove.movePiece(move, chess.board);
    }
    // pour deplacer transform.localPosition = new Vector3(targetX, targetY);
}

