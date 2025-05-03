using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ChessBoard : MonoBehaviour
{
    private List<Piece> pieces;
    public GameObject Pieces;
    private Piece[,] chessBoard;
    private readonly string[,] squareNames = {
        {"a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8"},
        {"a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7"},
        {"a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6"},
        {"a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5"},
        {"a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4"},
        {"a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3"},
        {"a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2"},
        {"a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1"}
    };
    private List<Move> movesHistory;
    private List<Piece> capturedBlackPieces;
    private List<Piece> capturedWhitePieces;

    void Awake()
    {
        this.pieces = new List<Piece>();
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
        foreach (Transform child in Pieces.transform)
        {
            pieces.Add(child.GetComponent<Piece>());
            // Debug.Log("add " + pieces.Last().name);
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
        // print();
        // puis on rajoute les pièces
        foreach (Piece piece in pieces)
        {
            // print("for piece : " + piece.name);
            int x = (int)piece.transform.position.x;
            int y = (int)piece.transform.position.y;
            // print($"try to add piece in [{-y}, {x}]");
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
                    tempPiece.setAttributes("Pawn", "White", x, y, this);
                }
                else if (y == 6)
                {
                    tempPiece.setAttributes("Pawn", "Black", x, y, this);
                }
                else if (y == 0)
                {
                    tempPiece.setAttributes(getPieceName(x), "White", x, y, this);
                }
                else if (y == 7)
                {
                    tempPiece.setAttributes(getPieceName(x), "Black", x, y, this);
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
    public void movePieceInVirtualChessBoard(Position p, Piece piece, bool isVirtualMove)
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

        // si le coup n'est pas virtuel (pour les tests en cas d'échec apres le déplacement d'une pièce)
        if (!isVirtualMove)
        {
            // on bouge la pièce dans la vue du jeu
            piece.transform.position = new Vector3(piece.getX(), -piece.getY(), -1); // TODO //////////////////////////////////
            // on indique à la pièce qu'elle a bougée
            piece.madeMove();
            // debug affichage
            print($"{piece.name} moved to ({piece.getX()}, {piece.getY()})");
        }
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
        // print("position out of board");
        return false;
    }


    public Piece getPiece(Position p)
    {
        if (isNotOut(p))
        {
            // print($"getPiece({p.getX()}, {p.getY()}) = {this.chessBoard[p.getY(), p.getX()]}");
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
                if (p != null && p.getName() == "King" && p.getColor() == color)
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

    // scan dans la direction donnée depuis la position donnée jusqu'à tomber sur une pièce ou sortir du plateau
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
            if (this.chessBoard[posY, posX] == null || posX == position.getX() && posY == position.getY())
            {
                position.incrementXY(stepX, stepY);
            }
            else
            {
                break;
            }
        }
        if (this.getPiece(position) != null)
        {
            print($"piece found at ({position.getX()}, {position.getY()}) : {this.getPiece(position).name}");
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
        Position p = new Position(0, 0);
        Piece piece;
        string str = "";
        for (int y = 0; y < 8; y++)
        {
            str += 8 - y + "|";
            for (int x = 0; x < 8; x++)
            {
                p.setPosition(x, y);
                piece = this.getPiece(p);
                if (piece == null)
                {
                    str += " ◻ ";
                    continue;
                }
                if (piece.getColor() == "Black")
                {
                    switch (piece.getName())
                    {
                        case "King":
                            str += " ♔ ";
                            break;
                        case "Queen":
                            str += " ♕ ";
                            break;
                        case "Bishop":
                            str += " ♗ ";
                            break;
                        case "Knight":
                            str += " ♘ ";
                            break;
                        case "Rook":
                            str += " ♖ ";
                            break;
                        case "Pawn":
                            str += " ♙ ";
                            break;
                    }
                }
                else
                {
                    switch (piece.getName())
                    {
                        case "King":
                            str += " ♚ ";
                            break;
                        case "Queen":
                            str += " ♛ ";
                            break;
                        case "Bishop":
                            str += " ♝ ";
                            break;
                        case "Knight":
                            str += " ♞ ";
                            break;
                        case "Rook":
                            str += " ♜ ";
                            break;
                        case "Pawn":
                            str += " ♟ ";
                            break;
                    }
                }
            }
            str += "\n";
        }
        str += "    -----------------------\n     a   b   c   d   e   f   g   h";
        return str;
    }
}
