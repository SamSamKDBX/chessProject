using UnityEngine;

public class Move
{
    private Piece piece;
    private Position posTarget;
    private Move lastMove;

    public Move(Piece piece, int x, int y)
    {
        this.piece = piece;
        this.posTarget = new Position(x, y);
        this.lastMove = null;
    }

    public Piece getPiece()
    {
        return this.piece;
    }

    public Position getPosition()
    {
        return this.posTarget;
    }

    public Move getLastMove()
    {
        return this.lastMove;
    }

    public void setLastMove(Move move)
    {
        this.lastMove = move;
    }
}