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

    public void setPosition(Position p)
    {
        this.position = p;
    }

    public bool neverMadeMove()
    {
        return this.hasNeverMoved;
    }

    // pas encore vérifiée
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
            && this.isWayClear(move, chessBoard)
            && this.isLegalMove(move)) // vérifier iswayclear
        {
            chessBoard.movePiece(target, this);
            chessBoard.addMoveToHistory(move);
            return true;
        }
        return false;
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

    // pas encore vérifié
    private bool isWayClear(Move move, ChessBoard chessBoard)
    {
        string direction = "";

        // on récupère la position target du mouvement et ses coordonnées
        Position target = move.getPosition();
        int targetX = target.getX();
        int targetY = target.getY();

        // les coordonnées de la pièce actuelle (this)
        int posX = this.getX();
        int posY = this.getY();

        int stepX; // direction du pas en x
        int stepY; // direction du pas en Y

        Position nextPieceFoundPosition;

        // on cherche la direction du mouvement pour vérifier qu'il n'y a pas de pièce adverse sur la route
        // changer pour mettre directement les step ici /////////////////////////////////////////////////
        if (targetY == posY && targetX < posX) { direction = "Left"; stepX = -1; stepY = 0 } // Left

        else if (targetY == posY && targetX > posX) { direction = "Right"; stepX = 1; stepY = 0 } // Right

        else if (targetY < posY && targetX == posX) { direction = "Top"; stepX = 0; stepY = 1 } // Top

        else if (targetY > posY && targetX == posX) { direction = "Bottom"; stepX = 0; stepY = -1 } // Bottom

        else if (target.distanceY(this.getPosition()) == target.distanceX(this.getPosition()))
        {
            if (targetY < posY && targetX > posX) { direction = "TopRightCorner"; stepX = 1; stepY = 1 } // TopRightCorner

            else if (targetY < posY && targetX < posX) { direction = "TopLeftCorner"; stepX = -1; stepY = 1 } // TopLeftCorner

            else if (targetY > posY && targetX < posX) { direction = "BottomLeftCorner"; stepX = 1; stepY = -1 } // BottomLeftCorner

            else if (targetY > posY && targetX > posX) { direction = "BottomRightCorner"; stepX = -1; stepY = -1 } // BottomRightCorner
        }

        // on déclare une position (qui va changer) pour la prochaine pièce trouvée dans la direction du mouvement
        nextPieceFoundPosition = this.position.copy();

        // puis on déplace la position dans la direction donnée jusqu'à trouver une case non vide
        // ou atteindre la position target du mouvement étudié
        while (isNotOut(nextPieceFoundPosition) 
            && !nextPieceFoundPosition.equals(target) 
            && chessBoard.getPiece(nextPieceFoundPosition) == null)
        {
            nextPieceFoundPosition.incrementXY(stepX, stepY);
        }

        // if (!nextPieceFoundPosition.equals(target))
        
        return false;
    }

    /*
        compare la distance entre piece de this-p1 et this-p2 et retourne le résultat
        en gros : return |this-p2 - this-p1|
    */
    public bool compareDistance(Piece p1, Piece p2, string direction)
    {
        int p1X = p1.getX();
        int p1Y = p1.getY();
        int p2X = p2.getX();
        int p2Y = p2.getY();
        // à faire ///////////////////////////////////////////////////
        int distance_pos_piece;
        int distance_pos_target;
        // comparer |x1 - x2 + y1 - y2| et |x3 - x4 + y3 - y4| quand ligne droite
        // si diagonale comparer que avec x ou y
        if (direction == "Right"
            || direction == "Left"
            || direction == "Top"
            || direction == "Bottom")
        {
            // si la direction est en ligne droite, on initialise la distance [pièce rencontré]-[position initiale]
            // à |posX - pieceX + posY - pieceY|
            // et la distance à parcourir pour effectuer le mouvement voulu par le joueur
            // à |posX - targetX + posY - targetY|
            distance_pos_piece = Mathf.Abs(move.getPosition().getX() - target.line + move.posColumn - target.column);
            distance_pos_target = Mathf.Abs(move.posLine - move.targetLine + move.posColumn - move.targetColumn);
        }
        else
        {
            // sinon on initialise en ne faisant la différence que de x ou y car si on fait l'addition,
            // le calcul donnera toujours 0.
            distance_pos_piece = Mathf.Abs(move.posLine - target.line);
            distance_pos_target = Mathf.Abs(move.poLine - move.targetLine);
        }
        // comparer les résultats et retourner
        return false; // a changer
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