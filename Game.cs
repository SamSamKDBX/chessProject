using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ChessBoard chessBoard;

    void Start()
    {
        // faire un test de mouvement 
        Position pos = new Position(4, 6); // position du pion blanc devant le roi (e2);
        print("Position créée");
        Piece piece = chessBoard.getPiece(pos); // le pion e2
        print("Piece récupérée");
        Move move_test = new Move(piece, 4, 4); // on créé un nouveau move avec la pièce et la position target
        print("Move créé");
        bool validMove = piece.moveTo(move_test, chessBoard, false); // on essaye de déplacer la pièce
        if (!validMove) {
            print("e4 invalide");
        }
        else {
            chessBoard.print();
        }
    }

    
}
