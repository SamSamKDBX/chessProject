using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    private string Name;
    private string color;
    private Position position;
    private List<Position> latestPositions;
    private bool hasNeverMoved;
    private ChessBoard chessBoard;
    private readonly string[] directions = {
            "Bottom",
            "Right",
            "Top",
            "Left",
            "TopRightCorner",
            "TopLeftCorner",
            "BottomRightCorner",
            "BottomLeftCorner"
        };

    public void setAttributes(string Name, string color, int x, int y, ChessBoard chessBoard)
    {
        this.Name = Name;
        this.color = color;
        this.position = new Position(x, y);
        this.hasNeverMoved = true;
        this.chessBoard = chessBoard;
        this.latestPositions = new List<Position>();
    }

    public string getName()
    {
        return this.Name;
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

    public void setLastPosition(Position pos)
    {
        this.latestPositions.Add(pos);
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

    public void madeMove()
    {
        this.hasNeverMoved = false;
    }

    public bool moveTo(Move move, ChessBoard chessBoard, bool isVirtualMove)
    {
        Position target = move.getPosition();

        // si les conditions :
        // - la pièce se déplace au moins d'une case
        // - le mouvement reste dans la surface du plateau
        // - il n'y a pas de pieces sur le passage
        // - le coup est légal pour la pièce à déplacer
        // sont réunies, on déplace la pièce
        if (isLegalMove(move))
        {
            // on bouge dans le tableau chessBoard
            chessBoard.movePieceInVirtualChessBoard(target, this, isVirtualMove);
            // si le roi est en échec après le mouvement, on reviens à la position initiale
            if (chessBoard.getKing(this.color).isCheck(move, chessBoard))
            {
                chessBoard.movePieceInVirtualChessBoard(this.latestPositions.Last(), this, isVirtualMove);
                this.latestPositions.RemoveAt(this.latestPositions.Count - 1);
                Debug.Log("You cannot put the king in check");
                return false;
            }
            chessBoard.addMoveToHistory(move);
            return true;
        }
        print("move is not legal");
        return false;
    }

    public bool isLegalMove(Move move)
    {
        if (move.getPosition().equals(this.position)
            || !chessBoard.isNotOut(move.getPosition())
            || !this.isWayClear(move, chessBoard))
        {
            return false;
        }
        switch (this.Name)
        {
            case "King": return this.isKingLegalMove(move);
            case "Queen": return this.isQueenLegalMove(move);
            case "Bishop": return this.isBishopLegalMove(move);
            case "Knight": return this.isKnightLegalMove(move);
            case "Rook": return this.isRookLegalMove(move);
            case "Pawn": return this.isPawnLegalMove(move);
            default:
                print("move isn't legal");
                return false;
        }
    }

    // pas encore vérifié
    private bool isWayClear(Move move, ChessBoard chessBoard)
    {
        if (this.Name == "Knight" && this.color != move.getPiece().getColor())
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
        // (targetY > posY && targetX == posX) || (targetY > posY && targetX < posX) || (targetY > posY && targetX > posX)
        stepY = targetY > posY ? 1 : -1;

        // Si la direction est "Right", "BottomRightCorner" ou "TopRightCorner",
        // on se déplace vers la droite (colonne positive : +1), sinon vers la gauche (-1).
        // (targetY == posY && targetX > posX) || (targetY < posY && targetX > posX) || (targetY > posY && targetX > posX)
        stepX = targetX > posX ? 1 : -1;

        // Si la direction est strictement horizontale ("Left" ou "Right"),
        // il n'y a pas de mouvement vertical, donc on met stepY à 0.
        if (targetY == posY)
        {
            stepY = 0;
        }
        // Si la direction est strictement verticale ("Bottom" ou "Top"),
        // il n'y a pas de mouvement horizontal, donc on met stepX à 0.
        else if (targetX == posX)
        {
            stepX = 0;
        }

        // on initialise une position (qui va changer) pour la prochaine pièce trouvée dans la direction du mouvement
        // et qui démarre à la position de la pièce actuelle (this)
        scanPos = this.position.copy();

        // puis on déplace la position dans la direction donnée jusqu'à trouver une case non vide
        // ou atteindre la position target du mouvement étudié
        while (chessBoard.isNotOut(scanPos)
            && !scanPos.equals(target))
        {
            scanPos.incrementXY(stepX, stepY);
            if (chessBoard.getPiece(scanPos) != null)
            {
                print($"scan stoped at ({scanPos.getX()}, {scanPos.getY()}) with piece : {chessBoard.getPiece(scanPos).getName()}");
                break;
            }
            else if (scanPos.equals(target))
            {
                print($"scan stoped at ({scanPos.getX()}, {scanPos.getY()}) because the target is here");
            }
        }

        // Si la case sur laquelle on s'est arrêté est différente de la target du move
        // ou que la case comporte une pièce de même couleur, alors le move n'est pas valide (passage à travers une pièce)
        Piece foundPiece = chessBoard.getPiece(scanPos);
        if (!scanPos.equals(target) || foundPiece != null && foundPiece.getColor() == this.color)
        {
            print("Way is not clear");
            print("scanPos.equals(target) = " + scanPos.equals(target));
            print("foundPiece null ? " + foundPiece == null);
            return false;
        }

        // on ne passe jamais au dessus d'une pièce et on ne s'arrête pas sur une case remplie par une pièce
        // de la même couleur, donc tout va bien
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
        print("move is not king legal move");
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
        print("move is not queen legal move");
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
        print("move is not rook legal move");
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
        print("move is not bishop legal move");
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
        print("move is not knight legal move");
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
            Piece lastMovedPiece = this.chessBoard.getLastMoveFromHistory().getPiece();

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
        print("move is not pawn legal move");
        return false;
    }

    // vérifie si la case move.position est attaquée
    public bool isCheck(Move move, ChessBoard chessBoard)
    {
        // on créer une position temporaire 
        Position pos = new Position(move.getPosition().getX(), move.getPosition().getY());
        // une piece temporaire
        Piece tempPiece;

        // on regarde si un cavalier, le roi ou la reine adverse est a proximité
        // dans un carré de 16 cases autour de la case à vérifier
        for (int i = -2; i < 3; i++)
        {
            for (int j = -2; j < 3; j++)
            {
                pos.setPosition(pos.getX() + i, pos.getY() + j);
                tempPiece = chessBoard.getPiece(pos);
                if (tempPiece != null && tempPiece.getColor() != this.color)
                {
                    // on vérifie les cavaliers adverse à ces cases là :
                    /*
                        . X . X .
                        X . . . X
                        . . O . .
                        X . . . X
                        . X . X .
                    */
                    if ((Mathf.Abs(i) == 1 && Mathf.Abs(j) == 2 || Mathf.Abs(i) == 2 && Mathf.Abs(j) == 1)
                        && (tempPiece.getName() == "Knight"))
                    {
                        print($"Check with kinght at ({tempPiece.getX()}, {tempPiece.getY()})");
                        return true;
                    }
                    else if (Mathf.Abs(i) == 1 && Mathf.Abs(j) == 1
                        && (tempPiece.getName() == "King"
                        || tempPiece.getName() == "Queen"))
                    {
                        print($"Check with {tempPiece.name} at ({tempPiece.getX()}, {tempPiece.getY()})");
                        return true;
                    }
                }
            }
        }

        // une autre pièce temporaire et son nom
        Piece pieceFound;
        string pieceFoundName;

        // on trace une ligne dans chaque direction pour vérifier si une pièce n'attaque pas la case
        for (int i = 0; i < 8; i++)
        {
            // on réinitialise la position temporaire à la position de la pièce étudiée
            pos.setPosition(this.getX(), this.getY());

            // on regarde la permière piece touvée dans une direction donnée
            chessBoard.findNextPiece(directions[i], pos);
            pieceFound = chessBoard.getPiece(pos);
            // si c'est une pièce adverse,
            if (pieceFound != null && pieceFound.getColor() != this.color)
            {
                pieceFoundName = pieceFound.getName();
                // s'il y a une reine,
                if (pieceFoundName == "Queen")
                {
                    print($"Check with {pieceFound.name} at ({pieceFound.getX()}, {pieceFound.getY()})");
                    return true;
                }
                // une tour adverse qui attaque la case en ligne droite,
                else if (i < 4 && pieceFoundName == "Rook")
                {
                    print($"Check with {pieceFound.name} at ({pieceFound.getX()}, {pieceFound.getY()})");
                    return true;
                }
                else if (i >= 4)
                {
                    // un fou en diagonale
                    if (pieceFoundName == "Bishop")
                    {
                        print($"Check with {pieceFound.name} at ({pieceFound.getX()}, {pieceFound.getY()})");
                        return true;
                    }
                    // ou un pion proche en diagonale
                    else if (pieceFoundName == "Pawn"
                        && (pieceFound.getColor() == "Black"
                        && pieceFound.getY() == this.getY() + 1
                        || pieceFound.getColor() == "White"
                        && pieceFound.getY() == this.getY() - 1)
                        && Mathf.Abs(move.getPosition().getX() - this.getX()) == 1)
                    {
                        print($"Check with {pieceFound.name} at ({pieceFound.getX()}, {pieceFound.getY()})");
                        return true;
                    }
                }
            }
        }

        // sinon c'est que la case n'est pas attaquée
        return false;
    }

    /* public void promote()
    {
        string choosedPiece;
        // choosedPiece = saisie parmi {"Queen", "Bishop", "Rook", "Knight"}
        // this.Name = choosedPiece;
    } */

    private void kingPossibleMoves(List<Move> moves)
    {
        // opstimisation possible
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Move move = new Move(this, this.getX() + i, this.getY() + j);
                if (this.moveTo(move, this.chessBoard, true))
                {
                    moves.Add(move);
                }
            }
        }
    }
    private void rookPossibleMoves(List<Move> moves)
    {
        // optimisation possible
        for (int i = 0; i < 8; i++)
        {
            Move move = new Move(this, i, this.getY());
            if (this.moveTo(move, this.chessBoard, true))
            {
                moves.Add(move);
            }
        }
        for (int i = 0; i < 8; i++)
        {
            Move move = new Move(this, this.getX(), i);
            if (this.moveTo(move, this.chessBoard, true))
            {
                moves.Add(move);
            }
        }
    }

    private void bishopPossibleMoves(List<Move> moves)
    {
        // optimisation possible
        Position pos;
        int stepX;
        int stepY;
        for (int i = 0; i < 4; i++)
        {
            // chaque case possible pour un fou
            switch (i)
            {
                case 0: stepX = 1; stepY = 1; break;
                case 1: stepX = 1; stepY = -1; break;
                case 2: stepX = -1; stepY = 1; break;
                case 3: stepX = -1; stepY = -1; break;
                default: return;
            }
            pos = new Position(this.getX(), this.getY());
            while (chessBoard.isNotOut(pos))
            {
                Move move = new Move(this, pos.getX(), pos.getY());
                if (this.moveTo(move, this.chessBoard, true))
                {
                    moves.Add(move);
                }
                pos.incrementXY(stepX, stepY);
            }
        }
    }

    private void knightPossibleMoves(List<Move> moves)
    {
        // optimisation possible
        int x;
        int y;
        for (int i = 0; i < 8; i++)
        {
            // chaque case possible pour un cavalier
            switch (i)
            {
                case 0: x = this.getX() + 2; y = this.getY() - 1; break;
                case 1: x = this.getX() + 2; y = this.getY() + 1; break;
                case 2: x = this.getX() + 1; y = this.getY() - 2; break;
                case 3: x = this.getX() + 1; y = this.getY() + 2; break;
                case 4: x = this.getX() - 2; y = this.getY() + 2; break;
                case 5: x = this.getX() - 2; y = this.getY() - 2; break;
                case 6: x = this.getX() - 1; y = this.getY() + 2; break;
                case 7: x = this.getX() - 1; y = this.getY() - 2; break;
                default: return;
            }
            Move move = new Move(this, x, y);
            if (this.moveTo(move, this.chessBoard, true))
            {
                moves.Add(move);
            }
        }
    }

    private void pawnPossibleMoves(List<Move> moves)
    {
        // optimisation possible
        int x;
        int y;
        for (int i = 0; i < 4; i++)
        {
            // chaque case possible pour un pion
            switch (i)
            {
                case 0: x = this.getX(); y = this.getY() + 1; break;
                case 1: x = this.getX(); y = this.getY() + 2; break;
                case 2: x = this.getX() + 1; y = this.getY() + 1; break;
                case 3: x = this.getX() - 1; y = this.getY() + 1; break;
                default: return;
            }
            Move move = new Move(this, x, y);
            if (this.moveTo(move, this.chessBoard, true))
            {
                moves.Add(move);
            }
        }
    }

    private void printPossibleMoves()
    {
        List<Move> possibleMoves = new List<Move>();
        switch (this.Name)
        {
            case "King":
                this.kingPossibleMoves(possibleMoves);
                break;
            case "Queen":
                this.rookPossibleMoves(possibleMoves);
                this.bishopPossibleMoves(possibleMoves);
                break;
            case "Bishop":
                this.bishopPossibleMoves(possibleMoves);
                break;
            case "Knight":
                this.knightPossibleMoves(possibleMoves);
                break;
            case "Rook":
                this.rookPossibleMoves(possibleMoves);
                break;
            case "Pawn":
                this.pawnPossibleMoves(possibleMoves);
                break;
        }
        foreach (Move move in possibleMoves)
        {
            // TODO
            // afficher au joueur les coups contenus dans la liste

        }
    }

    private bool isClicked()
    {
        // Vérifie si le bouton gauche de la souris est enfoncé
        if (Input.GetMouseButtonDown(0))
        {
            // Crée un rayon depuis la position de la souris
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            // Vérifie si un objet a été touché par le rayon
            if (hit.collider != null)
            {
                // Si l'objet cliqué est celui-ci
                if (hit.collider.gameObject == this.gameObject)
                {
                    // Code à exécuter lorsqu'on clique sur l'objet
                    // attendre que le joueur clique sur une autre case

                    Debug.Log("L'objet " + this.name + " a été cliqué");
                    return true;
                }
            }
        }
        return false;
    }

    void Update()
    {
        if (this.isClicked())
        {
            this.printPossibleMoves();
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                // Vérifie si un objet a été touché par le rayon
                if (hit.collider != null)
                {
                    // TODO
                    // boucle foreach pour les coups possible et vérification si la case cliquée est l'un d'eux


                    
                    // Si l'objet cliqué est celui-ci
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        // Code à exécuter lorsqu'on clique sur l'objet
                        // attendre que le joueur clique sur une autre case

                    }
                }
            }
        }
    }
}