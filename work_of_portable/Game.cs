using UnityEngine;

public class Game : MonoBehaviour
{
    public ChessBoard chessBoard = new ChessBoard();

    void Start()
    {
        Piece piece;
        // Remplissage du plateau
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                // Exemple de logique pour remplir le tableau
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
                    piece = new Piece(getPieceName(x), "White");
                }
                else if (y == 7)
                {
                    piece = new Piece(getPieceName(x), "Black");
                }
                else
                {
                    continue;
                }

                try
                {
                    chessBoard.addPiece(piece);
                }
                catch (exception e)
                {
                    print(e);
                }
            }
        }
        chessBoard.print();
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
        }
    }

    void Update()
    {

    }
}
