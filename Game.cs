using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public ChessBoard chessBoard = new ChessBoard();

    void Start()
    {
        Piece piece;
        // Remplissage du plateau
        /*
            7| ♖  ♘  ♗  ♕  ♔  ♗  ♘  ♖  Black
            6| ♙  ♙  ♙  ♙  ♙  ♙  ♙  ♙
            5| ◼  ◻  ◼  ◻  ◼  ◻  ◼  ◻
            4| ◻  ◼  ◻  ◼  ◻  ◼  ◻  ◼
            3| ◼  ◻  ◼  ◻  ◼  ◻  ◼  ◻
            2| ◻  ◼  ◻  ◼  ◻  ◼  ◻  ◼
            1| ♟  ♟  ♟  ♟  ♟  ♟  ♟  ♟
            0| ♜  ♞  ♝  ♛  ♚  ♝  ♞  ♜  White
              -----------------------
               0  1  2  3  4  5  6  7 
        */
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (y == 1)
                {
                    piece = new Piece("Pawn", "White", x, y);
                }
                else if (y == 6)
                {
                    piece = new Piece("Pawn", "Black", x, y);
                }
                else if (y == 0)
                {
                    piece = new Piece(getPieceName(x), "White", x, y);
                }
                else if (y == 7)
                {
                    piece = new Piece(getPieceName(x), "Black", x, y);
                }
                else
                {
                    continue;
                }

                try {
                    chessBoard.addPiece(piece);
                } catch (Exception e) {
                    print(e);
                }
            }
        }
        chessBoard.print();
        // faire un test de mouvement ///////////////////////////////
        Position p = new Position(4, 1); // position du pion blanc devant le roi (e2)
        Move move_test = new Move(chessBoard.getPiece(p), p);
    }

    private string getPieceName(int columnIndex)
    {
        switch (columnIndex)
        {
            case 0: return "Rook";
            case 1: return "Knight";
            case 2: return "Bishop";
            case 3: return "Queen";
            case 4: return "King";
            case 5: return "Bishop";
            case 6: return "Knight";
            case 7: return "Rook";
            default: return "Error";
        }
    }

    void Update()
    {

    }
}
