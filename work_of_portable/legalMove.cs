using System.Data.Common;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class legalMove
{
    // rajouter une declaration de chessBoard (tableau de structure) dans le start de l'objet chessBoard
    public static bool movePiece(structure.s_move move, structure.s_square[,] chessBoard)
    {
        // on créer une structure position
        structure.s_position targetPosition = new structure.s_position(move.targetLine, move.targetColumn)

        bool isLegalMove = false;
        if (isInChessBoard(targetPosition) && isWayClear(move, chessBoard))
        {
            // si le mouvement reste dans la surface du plateau et qu'il n'y a pas de pieces sur le passage
            if (move.pieceName == "Empty" || move.pieceColor == "Empty")
            {
                return false;
            }
            else if (move.pieceName == "Queen" && isQueenLegalMove(move))
            {
                isLegalMove = true;
            }
            else if (move.pieceName == "King" && isKingLegalMove(move, chessBoard))
            {
                isLegalMove = true;
            }
            else if (move.pieceName == "Knight" && isKnightLegalMove(move))
            {
                isLegalMove = true;
            }
            else if (move.pieceName == "Bishop" && isBishopLegalMove(move))
            {
                isLegalMove = true;
            }
            else if (move.pieceName == "Rook" && isRookLegalMove(move))
            {
                isLegalMove = true;
            }
            else if (move.pieceName == "Pawn" && isPawnLegalMove(move, chess.lastMove, chessBoard))
            {
                isLegalMove = true;
            }
        }
        // si la case est occupée par une pièce adverse
        if (chessBoard[move.targetLine, move.targetColumn].isEmpty == false
            && move.pieceColor != chessBoard[move.targetLine, move.targetColumn].pieceColor)
        {
            // capture
            chessBoard[move.targetLine, move.targetColumn].pieceName = "Empty";
            chessBoard[move.targetLine, move.targetColumn].pieceColor = "Empty";
        }
        // si le coup est légal, on déplace la piece.
        if (isLegalMove)
        {
            chessBoard[move.targetLine, move.targetColumn].pieceName = move.pieceName;
            chessBoard[move.targetLine, move.targetColumn].pieceColor = move.pieceColor;
            chessBoard[move.posLine, move.posColumn].pieceName = "Empty";
            chessBoard[move.posLine, move.posColumn].pieceColor = "Empty";
            chess.lastMove = move;
        }
        return isLegalMove;
    }

    // is in chessboard
    public static bool isInChessBoard(structure.s_position position)
    {
        // le mouvement reste sur le plateau
        if (position.line <= 7 && position.line >= 0 && position.column <= 7 && position.column >= 0)
        {
            return true;
        }
        return false;
    }

    // is way clear
    public static bool isWayClear(structure.s_position target, structure.s_move move, structure.s_square[,] chessBoard)
    {
        string direction;
        // on cherche la direction du mouvement pour vérifier qu'il n'y a pas de pièce adverse sur la route
        if (target.line == move.posLine && target.column < move.posColumn)
        {
            direction = "Left"
        }
        else if (target.line == move.posLine && target.column > move.posColumn)
        {
            direction = "Right"
        }
        else if (target.line < move.posLine && target.column == move.posColumn)
        {
            direction = "Top"
        }
        else if (target.line > move.posLine && target.column == move.posColumn)
        {
            direction = "Bottom"
        }
        else if (Mathf.Abs(move.posLine - target.line) == Mathf.Abs(move.posColumn - target.column)
            && target.line < move.posLine && target.column > move.posColumn)
        {
            direction = "TopRightCorner"
        }
        else if (Mathf.Abs(move.posLine - target.line) == Mathf.Abs(move.posColumn - target.column)
            && target.line < move.posLine && target.column < move.posColumn)
        {
            direction = "TopLeftCorner"
        }
        else if (Mathf.Abs(move.posLine - target.line) == Mathf.Abs(move.posColumn - target.column)
            && target.line > move.posLine && target.column < move.posColumn)
        {
            direction = "BottomLeftCorner"
        }
        else if (Mathf.Abs(move.posLine - target.line) == Mathf.Abs(move.posColumn - target.column)
            && target.line > move.posLine && target.column > move.posColumn)
        {
            direction = "BottomRightCorner"
        }
        else
        {
            // n'arrive normalement jamais sauf s'il y a une erreur
            return false;
        }
        scan.toThe(direction, target, chessBoard);
        if (compareDistance(move, target, direction))
        {
            //////////////////////////
            // il faut comparer la distance entre la position et la target et entre la position et la case retournée par le scan
        }
        return false;
    }

    public static bool compareDistance(structure.s_move move, structure.s_position target, string direction)
    {
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
            // si la direction est en ligne droite, on initialise la distance pièce rencontrée-position initiale
            // à |posX - pieceX + posY - pieceY|
            // et la distance à parcourir pour effectuer le mouvement voulu par le joueur
            // à |posX - targetX + posY - targetY|
            distance_pos_piece = Mathf.Abs(move.posLine - target.line + move.posColumn - target.column);
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
    }

    public static bool isAttacked(s_move move, structure.s_square[,] chessBoard)
    {
        // on créer une structure position
        structure.s_position pos = new structure.s_position(move.targetLine, move.targetColumn)

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
                if (isInChessBoard(pos)
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
    // créer wayIsClear
}