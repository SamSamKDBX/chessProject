public class Move
{
    private Piece piece;
    private Position posTarget;

    public Move(Piece piece, int x, int y)
    {
        this.piece = piece;
        this.posTarget = new Position(x, y);
    }

    public Piece getPiece()
    {
        return this.piece;
    }

    public Position getPosition()
    {
        return this.posTarget;
    }
}