using UnityEngine;

public struct BoardPosition
{
    public int X;
    public int Y;
    public int Z;

    public BoardPosition(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public static BoardPosition operator +(BoardPosition bp1, BoardPosition bp2)
    {
        return new BoardPosition(bp1.X + bp2.X, bp1.Y + bp2.Y, bp1.Z + bp2.Z);
    }

    public static BoardPosition operator -(BoardPosition bp1, BoardPosition bp2)
    {
        return new BoardPosition(bp1.X - bp2.X, bp1.Y - bp2.Y, bp1.Z - bp2.Z);
    }

    public static bool operator ==(BoardPosition bp1, BoardPosition bp2)
    {
        if (bp1.X == bp2.X && bp1.Y == bp2.Y)
            return bp1.Z == bp2.Z;
        else
            return false;
    }

    public static bool operator !=(BoardPosition bp1, BoardPosition bp2)
    {
        return !(bp1 == bp2);
    }

    public static BoardPosition operator -(BoardPosition bp)
    {
        return new BoardPosition(-bp.X, -bp.Y, -bp.Z);
    }

    public override string ToString()
    {
        return string.Format("BoardPosition ({0},{1},{2})", (object)this.X, (object)this.Y, (object)this.Z);
    }

    public string ToString(bool IncludePrependedInfo)
    {
        if (IncludePrependedInfo)
            return this.ToString();
        else
            return string.Format("({0},{1},{2})", (object)this.X, (object)this.Y, (object)this.Z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3((float)this.X, (float)this.Y, (float)this.Z);
    }

    public bool IsZero()
    {
        bool flag = false;
        if (this.X == 0 && this.Y == 0 && this.Z == 0)
            flag = true;
        return flag;
    }

    public override bool Equals(object obj)
    {
        if (obj is BoardPosition)
            return this == (BoardPosition)obj;
        else
            return false;
    }

    public bool Equals(BoardPosition bp)
    {
        return this == bp;
    }

    public override int GetHashCode()
    {
        return this.X ^ this.Y ^ this.Z;
    }

    public static BoardPosition AddBoardPositions(BoardPositionContainer bpc, BoardPosition pos)
    {
        return new BoardPosition(bpc.X + pos.X, bpc.Y + pos.Y, bpc.Z + pos.Z);
    }

    public static bool EqualsXY(BoardPosition bp1, BoardPosition bp2)
    {
        if (bp1.X == bp2.X)
            return bp1.Y == bp2.Y;
        else
            return false;
    }

    public static bool EqualsXZ(BoardPosition bp1, BoardPosition bp2)
    {
        if (bp1.X == bp2.X)
            return bp1.Z == bp2.Z;
        else
            return false;
    }

    public static bool EqualsYZ(BoardPosition bp1, BoardPosition bp2)
    {
        if (bp1.Y == bp2.Y)
            return bp1.Z == bp2.Z;
        else
            return false;
    }
}
