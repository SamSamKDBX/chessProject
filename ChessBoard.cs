using UnityEngine;

public class ChessBoard
{
    private Piece[,] chessBoard;

    public ChessBoard()
    {
        this.chessBoard = new Piece[8, 8];
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                this.chessBoard[y, x] = null;
            }
        }
    }

    public void addPiece(Piece piece)
    {
        int x = piece.getX();
        int y = piece.getY();

        if (x < 0 || x > 7 || y < 0 || y > 7)
        {
            throw new System.Exception("Erreur. Conditions : (0 < x < 8) && (0 < y < 8)");
        }

        this.chessBoard[y, x] = piece;
        piece.setChessBoard(this);
    }

    public Piece getPiece(Position p)
    {
        return this.chessBoard[p.getX(), p.getY()];
    }

    public void print()
    {
        Debug.Log(this.toString());
    }

    public string toString()
    {
        Position p = new Position(0, 0);
        string str = "";
        for (int y = 0; y < 8; y++)
        {
            str += "----------------------------------\n";
            for (int x = 0; x < 8; x++)
            {
                p.setPosition(x, y);
                str += "|";
                switch (this.getPiece(p).getName())
                {
                    case "King":
                        str += "K";
                        break;
                    case "Queen":
                        str += "Q";
                        break;
                    case "Bishop":
                        str += "b";
                        break;
                    case "Knight":
                        str += "k";
                        break;
                    case "Rook":
                        str += "r";
                        break;
                    case "Pawn":
                        str += "p";
                        break;
                    case null:
                        str += "  ";
                        break;
                }
                str += "|\n";
            }
        }
        str += "----------------------------------\n";
        return str;
    }
}
