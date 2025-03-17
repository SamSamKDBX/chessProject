using UnityEngine;

public static class scan {

    public static void toThe(string direction, structure.s_position position, structure.s_square[,] chessBoard) {
        // Déterminer la direction de mouvement selon les axes lignes et colonnes

        // Si la direction est "Bottom", "BottomRightCorner" ou "BottomLeftCorner", 
        // on se déplace vers le bas (ligne positive : +1), sinon vers le haut (-1).
        int directionLine = direction == "Bottom" || direction == "BottomRightCorner" || direction == "BottomLeftCorner" ? 1 : -1;

        // Si la direction est "Right", "BottomRightCorner" ou "TopRightCorner",
        // on se déplace vers la droite (colonne positive : +1), sinon vers la gauche (-1).
        int directionColumn = direction == "Right" || direction == "BottomRightCorner" || direction == "TopRightCorner" ? 1 : -1;

        // Si la direction est strictement horizontale ("Left" ou "Right"),
        // il n'y a pas de mouvement vertical, donc on met directionLine à 0.
        if (direction == "Left" || direction == "Right") {
            directionLine = 0;
        } 
        // Si la direction est strictement verticale ("Bottom" ou "Top"),
        // il n'y a pas de mouvement horizontal, donc on met directionColumn à 0.
        else if (direction == "Bottom" || direction == "Top") {
            directionColumn = 0;
        }


        // puis on déplace la position dans la direction donnée jusqu'à trouver une case non vide
        while (isInChessBoard(position) && chessBoard[position.line][position.column].isEmpty) {
            position.line += directionLine;
            position.column += directionColumn;
        }
        
        // on rajoute la direction de là ou on viens à la position de la case sur laquelle on est tombé
        position.directionFromColumn = directionColumn;
        position.directionFromLine = directionLine;
    }
}