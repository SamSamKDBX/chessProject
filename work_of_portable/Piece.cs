using UnityEngine;

public class Piece : MonoBehaviour
{
    private string name;
    private string color;
    private Position position;
    private bool isNeverMoved;

    public Piece(string name, string color, int x, int y)
    {
        this.name = name;
        this.color = color;
        this.pos = new Position(x, y);
        this.isNeverMoved = true;
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

    public void setPosition(Position p)
    {
        this.position = p;
    }

    public int getX()
    {
        return this.position.getX()
    }

    public int getY()
    {
        return this.position.getY()
    }

    public bool isNeverMoved()
    {
        return this.isNeverMoved;
    }

    public bool moveTo(Move move, ChessBoard chessBoard)
    {
        Position target = move.getPosition();

        // si les conditions :
        // - la pièce se déplace au moins d'une case
        // - le mouvement reste dans la surface du plateau
        // - il n'y a pas de pieces sur le passage
        // - le coup est légal
        // sont réunies, on déplace la pièce
        if (!target.equals(move.getPosition())
            && chessBoard.isNotOut(target)
            && isWayClear(move, chessBoard)
            && this.isLegalMove(move, chessBoard)) // vérifier iswayclear
        {
            chessBoard.movePiece(target, this);
            chessBoard.addMoveToHistory(move);
            return true;
        }
        return false;
    }

    public bool isLegalMove(Move move, ChessBoard chessBoard)
    {
        switch (this.name)
        {
            case "King": return this.isKingLegalMove(move, chessBoard);
            case "Queen": return this.isQueenLegalMove(move);
            case "Bishop": return this.isBishopLegalMove(move);
            case "Knight": return this.isKnightLegalMove(move);
            case "Rook": return this.isRookLegalMove(move);
            case "Pawn": return this.isPawnLegalMove(move, chessBoard.getLastMoveFromHistory(), chessBoard);
        }
    }

    public static bool isWayClear(Move move, ChessBoard chessBoard)
    {
        string direction;

        Position target = move.getPosition();
        int targetX = target.getX();
        int targetY = target.getY();

        int posX = this.getX();
        int posY = this.getY();

        Position nextPieceFoundPosition;

        // on cherche la direction du mouvement pour vérifier qu'il n'y a pas de pièce adverse sur la route
        if (targetY == posY && targetX < posX) { direction = "Left"; }

        else if (targetY == posY && targetX > posX) { direction = "Right"; }

        else if (targetY < posY && targetX == posX) { direction = "Top"; }

        else if (targetY > posY && targetX == posX) { direction = "Bottom"; }

        else if (target.distanceY(posY) == target.distanceX(posX))
        {
            if (targetY < posY && targetX > posX) { direction = "TopRightCorner"; }

            else if (targetY < posY && targetX < posX) { direction = "TopLeftCorner"; }

            else if (targetY > posY && targetX < posX) { direction = "BottomLeftCorner"; }

            else if (targetY > posY && targetX > posX) { direction = "BottomRightCorner"; }
        }
        else { return false; } // n'arrive normalement jamais sauf s'il y a une erreur

        // on déclare une position (qui va changer) pour la prochaine pièce trouvée dans la direction du mouvement
        nextPieceFoundPosition = this.position.copy();
        // on cherche la prochaine pièce dans la direction (la fonction modifie la position passée en paramètre)
        chessBoard.findNextPiece(direction, nextPieceFoundPosition);
        if (compareDistance(move, target, direction))
        {
            //////////////////////////
            // il faut comparer la distance entre la position et la target et entre la position et la case retournée par le scan
        }
        return false;
    }

    // King
    private bool isKingLegalMove(Move move, ChessBoard chessBoard)
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
    public static bool isRookLegalMove(Move move)
    {
        // ne se deplace que horizontalement ou verticalement 
        if (move.getPosition().getY() == posY || move.getPosition().getX() == posX)
        {
            return true;
        }
        return false;
    }

    // Bishop
    public static bool isBishopLegalMove(Move move)
    {
        // ne se deplace que horizontalement ou verticalement 
        if (move.getPosition().distanceY(this.position) == move.getPosition().distanceX(this.position))
        {
            return true;
        }
        return false;
    }

    // Knight
    public static bool isKnightLegalMove(Move move)
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
    public static bool isPawnLegalMove(Move move, Move lastMove, ChessBoard chessBoard)
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
            Piece lastPieceMoved = lastMove.getPiece();

            // si le pion se déplace d'une case en diagonale sur une case occuppée par un pion adverse
            if (chessBoard.getPiece(target) != null
                && this.color != chessBoard.getPiece(this.position).getColor())
            {
                return true;
            }
            // si le pion fait une prise en passant
            else if (lastPieceMoved.getName() == "Pawn"
                && lastPieceMoved.getColor() != this.color
                && target.getX() == lastPieceMoved.getPosition().getX();
                && (targetY == 2 && posY == 3) || (targetY == 5 posY == 4)
                && lastPieceMoved.getPosition().distanceX(this.position) == 1)
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
        this.name = choosedPiece;
    }
}