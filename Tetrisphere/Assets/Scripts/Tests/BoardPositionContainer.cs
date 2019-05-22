using System;

[Serializable]
public class BoardPositionContainer
{
    public int X;
    public int Y;
    public int Z;

    public BoardPositionContainer()
    {
        this.X = 0;
        this.Y = 0;
        this.Z = 0;
    }

    public BoardPositionContainer(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public BoardPositionContainer(BoardPosition bp)
    {
        this.X = bp.X;
        this.Y = bp.Y;
        this.Z = bp.Z;
    }

    public BoardPositionContainer(BoardPositionContainer bpc)
    {
        this.X = bpc.X;
        this.Y = bpc.Y;
        this.Z = bpc.Z;
    }

    public int GetX()
    {
        return this.X;
    }

    public int GetY()
    {
        return this.Y;
    }

    public int GetZ()
    {
        return this.Z;
    }

    public void Set(BoardPosition bp)
    {
        this.X = bp.X;
        this.Y = bp.Y;
        this.Z = bp.Z;
    }

    public void Set(int inX, int inY, int inZ)
    {
        this.X = inX;
        this.Y = inY;
        this.Z = inZ;
    }

    public void Set(BoardPositionContainer bpc)
    {
        this.X = bpc.X;
        this.Y = bpc.Y;
        this.Z = bpc.Z;
    }

    public BoardPosition ToBoardPosition()
    {
        return new BoardPosition(this.X, this.Y, this.Z);
    }

    public override string ToString()
    {
        return string.Format("BoardPositionContainer({0}, {1}, {2})", (object)this.X, (object)this.Y, (object)this.Z);
    }
}
