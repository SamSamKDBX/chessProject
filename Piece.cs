using UnityEngine;

public class Piece : MonoBehaviour
{
    private string name;
    private string color;
    private Position position;
    private bool hasNeverMoved;
    private ChessBoard chessBoard;

    public Piece(string name, string color, int x, int y)
    {
        this.name = name;
        this.color = color;
        this.position = new Position(x, y);
        this.hasNeverMoved = true;
        this.chessBoard = null;
    }

    public string getName()
    {
        return this.name;
    }

    public string getColor()
    {
        return this.color;
    }

    public Position getPosition()
    {
        return this.position;
    }

    public int getX()
    {
        return this.position.getX();
    }

    public int getY()
    {
        return this.position.getY();
    }

    public void setChessBoard(ChessBoard cb)
    {
        this.chessBoard = cb;
    }

    public bool neverMadeMove()
    {
        return this.hasNeverMoved;
    }

    public bool isLegalMove(Move move)
    {
        switch (this.name)
        {
            case "King": return this.isKingLegalMove(move);
            case "Queen": return this.isQueenLegalMove(move);
            case "Bishop": return this.isBishopLegalMove(move);
            case "Knight": return this.isKnightLegalMove(move);
            case "Rook": return this.isRookLegalMove(move);
            case "Pawn": return this.isPawnLegalMove(move);
            default: return false;
        }
    }

    // King
    private bool isKingLegalMove(Move move)
    {
        // on ne se deplace que d'une case
        if (move.getPosition().distanceX(this.position) <= 1
            && move.getPosition().distanceY(this.position) <= 1
            && !isCheck(move, chessBoard))
        {
            // faire le roque et isCheck ////////////////////////////////////////////////////////////////////
            return true;
        }
        return false;
    }

    // Queen
    private bool isQueenLegalMove(Move move)
    {
        // ne se deplace qu'horizontalement, verticalement ou en diagonale
        if (isRookLegalMove(move) || isBishopLegalMove(move))
        {
            return true;
        }
        return false;
    }

    // Rook
    private bool isRookLegalMove(Move move)
    {
        // ne se deplace que horizontalement ou verticalement 
        if (move.getPosition().getY() == this.position.getY() || move.getPosition().getX() == this.position.getX())
        {
            return true;
        }
        return false;
    }

    // Bishop
    private bool isBishopLegalMove(Move move)
    {
        // ne se deplace que horizontalement ou verticalement 
        if (move.getPosition().distanceY(this.position) == move.getPosition().distanceX(this.position))
        {
            return true;
        }
        return false;
    }

    // Knight
    private bool isKnightLegalMove(Move move)
    {
        // ne se deplace que en L 
        if (move.getPosition().distanceX(this.position) == 2
            && move.getPosition().distanceY(this.position) == 1
            || move.getPosition().distanceX(this.position) == 1
            && move.getPosition().distanceY(this.position) == 2)
        {
            return true;
        }
        return false;
    }

    // Pawn
    private bool isPawnLegalMove(Move move)
    {
        // si le pion est blanc il devra aller vers le haut sinon vers le bas
        int direction = this.color == "White" ? 1 : -1;
        // S'il est blanc sa ligne de départ est la 1 sinon la 6
        int startLine = this.color == "White" ? 1 : 6;

        Position target = move.getPosition();
        int targetX = target.getX();
        int targetY = target.getY();

        int posX = this.getX();
        int posY = this.getY();

        // si le pion n'a pas bougé et qu'il avance de deux cases verticalement
        // ou
        // si le pion avance d'une case verticalement
        if ((targetY == posY + 2 * direction && posY == startLine && targetX == posX)
            || (targetY == posY + direction && targetX == posX))
        {
            return true;
        }
        else if (targetY == posY + direction && target.distanceX(this.position) == 1)
        {
            Piece lastMovedPiece = move.getLastMove().getPiece();

            // si le pion se déplace d'une case en diagonale sur une case occuppée par un pion adverse
            if (this.chessBoard.getPiece(target) != null
                && this.color != this.chessBoard.getPiece(this.position).getColor())
            {
                return true;
            }
            // si le pion fait une prise en passant
            else if (lastMovedPiece.getName() == "Pawn"
                && lastMovedPiece.getColor() != this.color
                && target.getX() == lastMovedPiece.getPosition().getX()
                && targetY == 2 && posY == 3 || targetY == 5 && posY == 4
                && lastMovedPiece.getPosition().distanceX(this.position) == 1)
            {
                return true;
            }
        }
        return false;
    }

    public void promote()
    {
        string choosedPiece;
        // choosedPiece = saisie parmi {"Queen", "Bishop", "Rook", "Knight"}
        // this.name = choosedPiece;
    }
}