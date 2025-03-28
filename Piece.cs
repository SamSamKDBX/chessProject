using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    private string name;
    private string color;
    private Position position;
    private List<Position> latestPositions;
    private bool hasNeverMoved;
    private ChessBoard chessBoard;

    public Piece(string name, string color, int x, int y)
    {
        this.name = name;
        this.color = color;
        this.position = new Position(x, y);
        this.hasNeverMoved = true;
        this.chessBoard = null;
        this.latestPositions = new List<Position>();
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

    public Position getLastPosition()
    {
        return this.latestPositions.Last();
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
        // - le coup est légal pour la pièce à déplacer
        // sont réunies, on déplace la pièce
        if (!target.equals(move.getPosition())
            && chessBoard.isNotOut(target)
            && (this.isWayClear(move, chessBoard) || this.name == "Knight")
            && this.isLegalMove(move))
        {
            // on bouge dans le tableau chessBoard
            chessBoard.movePieceInChessBoard(target, this);
            // si le roi est en échec après le mouvement, on reviens à la position initiale
            if (chessBoard.getKing(this.color).isCheck()) // a faire ///////////////////////////
            {
                chessBoard.movePieceInChessBoard(this.latestPositions.Last(), this);
                this.latestPositions.RemoveAt(this.latestPositions.Count - 1);
                print("You cannot put the king in check");
                return false;
            }
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
        if (this.name == "Knight" && this.color != move.getPiece().getColor())
        {
            return true;
        }

        // on récupère la position target du mouvement et ses coordonnées
        Position target = move.getPosition();
        int targetX = target.getX();
        int targetY = target.getY();

        // les coordonnées de la pièce actuelle (this)
        int posX = this.getX();
        int posY = this.getY();

        int stepX; // direction du pas en x
        int stepY; // direction du pas en Y

        Position scanPos;

        // Si la direction est "Bottom", "BottomRightCorner" ou "BottomLeftCorner", 
        // on se déplace vers le bas (ligne négative : -1), sinon vers le haut (+1).
        stepY = (targetY > posY && targetX == posX) || (targetY > posY && targetX < posX) || (targetY > posY && targetX > posX) ? -1 : 1;

        // Si la direction est "Right", "BottomRightCorner" ou "TopRightCorner",
        // on se déplace vers la droite (colonne positive : +1), sinon vers la gauche (-1).
        stepX = (targetY == posY && targetX > posX) || (targetY < posY && targetX > posX) || (targetY > posY && targetX > posX) ? 1 : -1;

        // Si la direction est strictement horizontale ("Left" ou "Right"),
        // il n'y a pas de mouvement vertical, donc on met stepY à 0.
        if ((targetY == posY && targetX < posX) || (targetY == posY && targetX > posX))
        {
            stepY = 0;
        }
        // Si la direction est strictement verticale ("Bottom" ou "Top"),
        // il n'y a pas de mouvement horizontal, donc on met stepX à 0.
        else if ((targetY < posY && targetX == posX) || (targetY > posY && targetX == posX))
        {
            stepX = 0;
        }

        // on initialise une position (qui va changer) pour la prochaine pièce trouvée dans la direction du mouvement
        // et qui démarre à la position de la pièce actuelle (this)
        scanPos = this.position.copy();

        // puis on déplace la position dans la direction donnée jusqu'à trouver une case non vide
        // ou atteindre la position target du mouvement étudié
        while (chessBoard.isNotOut(scanPos)
            && !scanPos.equals(target)
            && chessBoard.getPiece(scanPos) == null)
        {
            scanPos.incrementXY(stepX, stepY);
        }

        // Si la case sur laquelle on s'est arrêté est différente de la target du move
        // ou que la case comporte une pièce de même couleur, alors le move n'est pas valide (passage à travers une pièce)
        if (!scanPos.equals(target) || chessBoard.getPiece(scanPos).getColor() == this.color)
        {
            return false;
        }

        // on ne passe jamais au dessus d'une pièce et on ne s'arrête pas sur une case remplie par une pièce
        // de la même couleur
        return true;
    }

    // King
    private bool isKingLegalMove(Move move)
    {
        // on ne se deplace que d'une case
        if (move.getPosition().distanceX(this.position) <= 1
            && move.getPosition().distanceY(this.position) <= 1
            && !this.isCheck(move, chessBoard))
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

    public static bool isAttacked(Move move, ChessBoard chessBoard)
    {
        // on créer une structure position
        Position pos = new Position(move.getPosition().getX(), move.targetColumn);

        string[] directions = {
            "Bottom",
            "Right",
            "Top",
            "Left",
            "TopRightCorner",
            "TopLeftCorner",
            "BottomRightCorner",
            "BottomLeftCorner"
        };

        // on regarde si le roi ou la reine adverse est a proximité
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                pos.line = move.targetLine + i;
                pos.column = move.targetColumn + j;
                if (isInChessBoard(pos)
                    && chessBoard[pos.line, pos.column].pieceColor != move.pieceColor
                    && (chessBoard[pos.line, pos.column].pieceName == "King"
                    || chessBoard[pos.line, pos.column].pieceName == "Queen"))
                {
                    return true;
                }
            }
        }

        // on trace une ligne dans chaque direction pour vérifier si une pièce n'attaque pas la case
        for (int i = 0; i < 8; i++)
        {
            scan.toThe(directions[i], pos, chessBoard)
            if (chessBoard[pos.line, pos.column].pieceColor != move.pieceColor)
            {
                if (i < 4 && (chessBoard[pos.line, pos.column].pieceName == "Rook"
                    || chessBoard[pos.line, pos.column].pieceName == "Queen"))
                {
                    // si il y a une reine ou une tour adverse qui attaque la case en ligne droite
                    return true;
                }
                else if (i >= 4 && (chessBoard[pos.line, pos.column].pieceName == "Bishop"
                    || chessBoard[pos.line, pos.column].pieceName == "Queen"
                    || chessBoard[pos.line, pos.column].pieceName == "Pawn"
                    && pos.line == move.targetLine + 1
                    && Mathf.Abs(move.targetColumn - pos.column) == 1))
                {
                    // si il y a un fou, la reine qui attaque en diagonale ou un pion proche
                    return true;
                }
            }
            pos.line = move.targetLine;
            pos.column = move.targetColumn;
        }

        // on vérifie les cavaliers adverse à ces cases là :
        /*
            . X . X .
            X . . . X
            . . O . .
            X . . . X
            . X . X .
        */
        int i = -2;
        int j = -2;
        while (i < 3)
        {
            while (j < 3)
            {
                pos.line = move.targetLine + i;
                pos.column = move.targetColumn + j;
                if (chessBoard.isNotOut(pos)
                    && (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 2 || Mathf.Abs(i) == 2 && Mathf.Abs(j) == 1)
                    && chessBoard[pos.line, pos.column].pieceColor != move.pieceColor
                    && (chessBoard[pos.line, pos.column].pieceName == "Knight"))
                {
                    return true;
                }
                j++;
            }
            i++;
        }

        // si on arrive là c'est que la case n'est pas attaquée
        return false;
    }

    public void promote()
    {
        string choosedPiece;
        // choosedPiece = saisie parmi {"Queen", "Bishop", "Rook", "Knight"}
        // this.name = choosedPiece;
    }
}