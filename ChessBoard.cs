using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.Collections;
using Unity.VisualScripting;

public class ChessBoard : MonoBehaviour
{
    public List<Piece> pieces;
    private GameObject Pieces;
    private Piece[,] chessBoard;
    private readonly string[,] squareNames = {
        {"8a", "8b", "8c", "8d", "8e", "8f", "8g", "8h"},
        {"7a", "7b", "7c", "7d", "7e", "7f", "7g", "7h"},
        {"6a", "6b", "6c", "6d", "6e", "6f", "6g", "6h"},
        {"5a", "5b", "5c", "5d", "5e", "5f", "5g", "5h"},
        {"4a", "4b", "4c", "4d", "4e", "4f", "4g", "4h"},
        {"3a", "3b", "3c", "3d", "3e", "3f", "3g", "3h"},
        {"2a", "2b", "2c", "2d", "2e", "2f", "2g", "2h"},
        {"1a", "1b", "1c", "1d", "1e", "1f", "1g", "1h"}
    };
    private List<Move> movesHistory;
    private List<Piece> capturedBlackPieces;
    private List<Piece> capturedWhitePieces;

    void Awake()
    {
        this.movesHistory = new List<Move>();
        this.capturedBlackPieces = new List<Piece>();
        this.capturedWhitePieces = new List<Piece>();
        Piece tempPiece;

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
        Pieces = GameObject.Find("Pieces");
        foreach (Piece child in Pieces.transform) {
            pieces.Add(child);
            Debug.Log("add " + child.name);
        }
        this.chessBoard = new Piece[8, 8];
        // on rempli d'abord de cases vides
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                this.chessBoard[y, x] = null;
            }
        }
        // puis on rajoute les pièces
        foreach (Piece piece in pieces)
        {
            int x = (int)piece.transform.localPosition.x;
            int y = (int)piece.transform.localPosition.y;
            this.chessBoard[-y, x] = piece;
        }
        // on remplie les attributs des pièces
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                tempPiece = this.chessBoard[y, x];
                if (y == 1)
                {
                    tempPiece.setAttributes("Pawn", "White", x, y);
                }
                else if (y == 6)
                {
                    tempPiece.setAttributes("Pawn", "Black", x, y);
                }
                else if (y == 0)
                {
                    tempPiece.setAttributes(getPieceName(x), "White", x, y);
                }
                else if (y == 7)
                {
                    tempPiece.setAttributes(getPieceName(x), "Black", x, y);
                }
                else
                {
                    continue;
                }
            }
        }
        print();
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
    /* public ChessBoard()
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
    } */

    public void addPiece(Piece piece)
    {
        int x = piece.getX();
        int y = piece.getY();
        this.chessBoard[y, x] = piece;
        piece.setChessBoard(this);
    }

    /*
        Fonction qui déplace une pièce donnée à une position données 
        dans le tableau chessBoard

        et modifie le champ position de la pièce
    */
    // précondition : le coup doit etre valide
    public void movePieceInChessBoard(Position p, Piece piece)
    {
        // on ajoute la position avant mouvement à la liste des dernieres positions
        piece.setLastPosition(piece.getPosition());

        // on récupère la pièce située à la position donnée
        Piece pieceInP = this.getPiece(p);

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
            this.capturedBlackPieces.Add(p);
            // this.capturedBlackPieces.sort();
        }
        else
        {
            this.capturedWhitePieces.Add(p);
            // this.capturedWhitePieces.sort();
        }
    }

    public bool isNotOut(Position p)
    {
        if (p.getX() >= 0 && p.getX() <= 7 && p.getY() <= 7 && p.getY() >= 0)
        {
            return true;
        }
        print("position out of board");
        return false;
    }


    public Piece getPiece(Position p)
    {
        if (isNotOut(p))
        {
            print($"getPiece({p})");
            return this.chessBoard[p.getY(), p.getX()];
        }
        return null;
    }


    public Piece getKing(string color)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece p = this.chessBoard[j, i];
                if (p.getName() == "King" && p.getColor() == color)
                {
                    return p;
                }
            }
        }
        return null;
    }

    public void addMoveToHistory(Move m)
    {
        this.movesHistory.Add(m);
    }

    /*
    public Move getMoveFromHistory(int index)
    {
        return this.movesHistory[index];
    }
    */

    public Move getLastMoveFromHistory()
    {
        return this.movesHistory.Last();
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
        // ou atteindre la position target du mouvement étudié
        while (isNotOut(position))
        {
            posX = position.getX();
            posY = position.getY();
            if (this.chessBoard[posY, posX] == null)
            {
                position.setPosition(posX + stepX, posY + stepY);
            }
        }

        // je sais plus pourquoi j'ai mis ca...
        // on rajoute la direction de là ou on viens à la position de la case sur laquelle on est tombé
        // position.directionFromColumn = stepX;
        // position.directionFromLine = stepY;
    }

    public string getSquareName(Position pos)
    {
        return this.squareNames[7 - pos.getX(), pos.getY()];
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
