using UnityEngine;

public class Position
{
    private int x;
    private int y;

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void setPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int getX()
    {
        return this.x;
    }

    public int getY()
    {
        return this.y;
    }

    public Position copy()
    {
        return new Position(this.x, this.y);
    }

    public bool equals(Position p)
    {
        if (this.x == p.getX() && this.y == p.getY())
        {
            return true;
        }
        return false;
    }

    public int distanceX(Position p)
    {
        return Mathf.Abs(this.x - p.getX());
    }

    public int distanceY(Position p)
    {
        return Mathf.Abs(this.y - p.getY());
    }
}