using UnityEngine;

public class chessBoard
{
    private Piece[,] chessBoard;
    private List<Move> movesHistory;
    private List<Piece> capturedBlackPieces;
    private List<Piece> capturedWhitePieces;

    public chessBoard()
    {
        this.movesHistory = new List<Move>();
        this.capturedBlackPieces = new List<Piece>();
        this.capturedWhitePieces = new List<Piece>();
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

        this.chessBoard[y, x] = piece;
    }

    public void movePiece(Position p, Piece piece)
    {
        pieceInP = this.getPiece(p);
        // si la case est occupée, on capture la pièce
        if (pieceInP != null)
        {
            this.capturePiece(pieceInP);
        }
        // on ajoute la piece sur la nouvelle position
        this.chessBoard[p.getY(), p.getX()] = piece;
        // on vide l'ancienne position
        this.chessBoard[piece.getY(), piece.getX()] = null;
        // on met à jour la position de la pièce déplacée
        piece.setPosition(p);
    }

    public void capturePiece(Piece p)
    {
        if (p.getColor() == "Black")
        {
            this.capturedBlackPieces.add(p);
            // this.capturedBlackPieces.sort();
        }
        else
        {
            this.capturedWhitePieces.add(p);
            // this.capturedWhitePieces.sort();
        }
    }

    public bool isNotOut(Position p)
    {
        if (p.x >= 0 && p.x <= 7 && p.y <= 7 && p.y >= 0)
        {
            return true;
        }
        return false;
    }

    public Piece getPiece(Position p)
    {
        return this.chessBoard[p.getX(), p.getY()];
    }

    public void addMoveToHistory(Move m)
    {
        this.movesHistory.add(m);
    }

    /*
    public Move getMoveFromHistory(int index)
    {
        return this.movesHistory[index];
    }
    */

    public Move getLastMoveFromHistory()
    {
        return this.movesHistory[this.movesHistory.Count - 1];
    }

    public void findNextPiece(string direction, Position position)
    {
        int posX;
        int posY;
        // Déterminer la direction de mouvement selon les axes lignes et colonnes

        // Si la direction est "Bottom", "BottomRightCorner" ou "BottomLeftCorner", 
        // on se déplace vers le bas (ligne négative : -1), sinon vers le haut (+1).
        int stepY = direction == "Bottom" || direction == "BottomRightCorner" || direction == "BottomLeftCorner" ? -1 : 1;

        // Si la direction est "Right", "BottomRightCorner" ou "TopRightCorner",
        // on se déplace vers la droite (colonne positive : +1), sinon vers la gauche (-1).
        int stepX = direction == "Right" || direction == "BottomRightCorner" || direction == "TopRightCorner" ? 1 : -1;

        // Si la direction est strictement horizontale ("Left" ou "Right"),
        // il n'y a pas de mouvement vertical, donc on met stepY à 0.
        if (direction == "Left" || direction == "Right")
        {
            stepY = 0;
        }
        // Si la direction est strictement verticale ("Bottom" ou "Top"),
        // il n'y a pas de mouvement horizontal, donc on met stepX à 0.
        else if (direction == "Bottom" || direction == "Top")
        {
            stepX = 0;
        }

        // puis on déplace la position dans la direction donnée jusqu'à trouver une case non vide
        while (isNotOut(position))
        {
            posX = position.getX();
            posY = position.getY();
            if (this.chessBoard[posY, posX] == null)
            {
                position.setPosition(posX + stepX, posY + stepY);
            }
        }

        // on rajoute la direction de là ou on viens à la position de la case sur laquelle on est tombé
        // position.directionFromColumn = stepX;
        // position.directionFromLine = stepY;
    }

    public void print()
    {
        print(this.toString());
    }

    public string toString()
    {
        string str = "";
        for (int y = 0; y < 8; y++)
        {
            str += "----------------------------------\n";
            for (int x = 0; x < 8; x++)
            {
                str += "|"
                switch (this.getPiece(x, y).getName)
                {
                    case "King": str += "K";
                    case "Queen": str += "Q";
                    case "Bishop": str += "b";
                    case "Knight": str += "k";
                    case "Rook": str += "r";
                    case "Pawn": str += "p";
                    case null: str += "  ";
                }:
                str += "|\n"
            }
        }
        str += "----------------------------------\n";
    }
}