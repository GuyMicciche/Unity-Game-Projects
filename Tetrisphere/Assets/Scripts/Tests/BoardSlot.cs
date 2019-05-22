using System.Collections.Generic;
using UnityEngine;

public class BoardSlot
{
    public static BoardPosition xUpBoardPos = new BoardPosition(1, 0, 0);
    public static BoardPosition xDownBoardPos = new BoardPosition(-1, 0, 0);
    public static BoardPosition yUpBoardPos = new BoardPosition(0, 1, 0);
    public static BoardPosition yDownBoardPos = new BoardPosition(0, -1, 0);
    public static BoardPosition zUpBoardPos = new BoardPosition(0, 0, 1);
    public static BoardPosition zDownBoardPos = new BoardPosition(0, 0, -1);
    protected BoardPositionContainer boardPos = new BoardPositionContainer();
    protected BoardPosition CachedBoardPos;
    protected GameBoard ParentGameBoard;
    protected GameShape FilledShape;
    protected BoardSlot xUpBoardSlot;
    protected BoardSlot xDownBoardSlot;
    protected BoardSlot yUpBoardSlot;
    protected BoardSlot yDownBoardSlot;
    protected BoardSlot zUpBoardSlot;
    protected BoardSlot zDownBoardSlot;

    public BoardPosition BoardPos
    {
        get
        {
            return this.CachedBoardPos;
        }
    }

    public BoardSlot(int xPos, int yPos, int zPos, GameBoard owner)
    {
        this.SetupPosition(xPos, yPos, zPos);
        this.ParentGameBoard = owner;
    }

    protected void SetupPosition(int xPos, int yPos, int zPos)
    {
        this.boardPos.Set(xPos, yPos, zPos);
        this.CachedBoardPos = this.boardPos.ToBoardPosition();
    }

    public void SetupDirectNeighborPointers()
    {
        this.xUpBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.xUpBoardPos, false, false, false, false);
        this.xDownBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.xDownBoardPos, false, false, false, false);
        this.yUpBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.yUpBoardPos, false, false, false, false);
        this.yDownBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.yDownBoardPos, false, false, false, false);
        this.zUpBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.zUpBoardPos, false, false, false, false);
        this.zDownBoardSlot = this.ParentGameBoard.GetBoardSlotForPositionOffset(this, BoardSlot.zDownBoardPos, false, false, false, false);
    }

    public bool CanBlock()
    {
        bool flag = false;
        if (this.FilledShape != null)
            flag = true;
        return flag;
    }

    public bool WillBlockShape(GameShape incoming)
    {
        bool flag = false;
        if (this.FilledShape != null)
            flag = this.FilledShape.WillBlockShapeAtBoardSlot(incoming, this);
        return flag;
    }

    public bool CommitFilledShape(GameShape newshape)
    {
        bool flag = false;
        if (newshape != null)
        {
            if (this.FilledShape == null)
            {
                this.FilledShape = newshape;
                this.ParentGameBoard.IncrementVerticalColumnShapeTracker(this.BoardPos);
                flag = true;
            }
            else
                Debug.LogError((object)"Tried to CommitFilledShape when FilledShape already set");
        }
        else
            Debug.LogWarning((object)"Tried to CommitFilledShape with a null newshape");
        return flag;
    }

    public void ClearFilledShape()
    {
        if (this.FilledShape != null)
            this.ParentGameBoard.DecrementVerticalColumnShapeTracker(this.BoardPos);
        this.FilledShape = (GameShape)null;
    }

    public GameShape GetFilledShape()
    {
        return this.FilledShape;
    }

    public bool HasFilledShape()
    {
        return this.FilledShape != null;
    }

    public GameShape GetNeighborShapeInDirection(BoardPosition inDirection)
    {
        GameShape gameShape = (GameShape)null;
        BoardPosition pos = this.BoardPos + new BoardPosition()
        {
            X = Mathf.Clamp(inDirection.X, -1, 1),
            Y = Mathf.Clamp(inDirection.Y, -1, 1),
            Z = Mathf.Clamp(inDirection.Z, -1, 1)
        };
        if (this.ParentGameBoard.CreateSafeBoardPosition(ref pos))
        {
            BoardSlot boardSlotForPosition = this.ParentGameBoard.GetBoardSlotForPosition(pos);
            if (boardSlotForPosition != null)
                gameShape = boardSlotForPosition.FilledShape;
        }
        return gameShape;
    }

    public void GetNeighborShapesInAllDirections(out GameShape xUp, out GameShape xDown, out GameShape yUp, out GameShape yDown, out GameShape zUp, out GameShape zDown)
    {
        xUp = this.xUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xUpBoardPos) : this.xUpBoardSlot.GetFilledShape();
        xDown = this.xDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xDownBoardPos) : this.xDownBoardSlot.GetFilledShape();
        yUp = this.yUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yUpBoardPos) : this.yUpBoardSlot.GetFilledShape();
        yDown = this.yDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yDownBoardPos) : this.yDownBoardSlot.GetFilledShape();
        zUp = this.zUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.zUpBoardPos) : this.zUpBoardSlot.GetFilledShape();
        if (this.zDownBoardSlot != null)
            zDown = this.zDownBoardSlot.GetFilledShape();
        else
            zDown = this.GetShapeInBoardSlotAtOffset(BoardSlot.zDownBoardPos);
    }

    public void GetNeighborShapes(List<GameShape> neighbors)
    {
        bool bIgnoreSameFilledShape = true;
        GameShape boardSlotAtOffset1 = this.GetShapeInBoardSlotAtOffset(BoardSlot.xUpBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset1 != null && !neighbors.Contains(boardSlotAtOffset1))
            neighbors.Add(boardSlotAtOffset1);
        GameShape boardSlotAtOffset2 = this.GetShapeInBoardSlotAtOffset(BoardSlot.xDownBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset2 != null && !neighbors.Contains(boardSlotAtOffset2))
            neighbors.Add(boardSlotAtOffset2);
        GameShape boardSlotAtOffset3 = this.GetShapeInBoardSlotAtOffset(BoardSlot.yUpBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset3 != null && !neighbors.Contains(boardSlotAtOffset3))
            neighbors.Add(boardSlotAtOffset3);
        GameShape boardSlotAtOffset4 = this.GetShapeInBoardSlotAtOffset(BoardSlot.yDownBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset4 != null && !neighbors.Contains(boardSlotAtOffset4))
            neighbors.Add(boardSlotAtOffset4);
        GameShape boardSlotAtOffset5 = this.GetShapeInBoardSlotAtOffset(BoardSlot.zUpBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset5 != null && !neighbors.Contains(boardSlotAtOffset5))
            neighbors.Add(boardSlotAtOffset5);
        GameShape boardSlotAtOffset6 = this.GetShapeInBoardSlotAtOffset(BoardSlot.zDownBoardPos, bIgnoreSameFilledShape);
        if (boardSlotAtOffset6 == null || neighbors.Contains(boardSlotAtOffset6))
            return;
        neighbors.Add(boardSlotAtOffset6);
    }

    public void GetNeighborShapes(SortedList<GameShape, int> neighbors)
    {
        bool bIgnoreSameFilledShape = true;
        GameShape other1 = this.xUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xUpBoardPos, bIgnoreSameFilledShape) : this.xUpBoardSlot.GetFilledShape();
        if (other1 != null)
            BoardSlot.AddNewToSortedList(neighbors, other1);
        GameShape other2 = this.xDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xDownBoardPos, bIgnoreSameFilledShape) : this.xDownBoardSlot.GetFilledShape();
        if (other2 != null)
            BoardSlot.AddNewToSortedList(neighbors, other2);
        GameShape other3 = this.yUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yUpBoardPos, bIgnoreSameFilledShape) : this.yUpBoardSlot.GetFilledShape();
        if (other3 != null)
            BoardSlot.AddNewToSortedList(neighbors, other3);
        GameShape other4 = this.yDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yDownBoardPos, bIgnoreSameFilledShape) : this.yDownBoardSlot.GetFilledShape();
        if (other4 != null)
            BoardSlot.AddNewToSortedList(neighbors, other4);
        GameShape other5 = this.zUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.zUpBoardPos, bIgnoreSameFilledShape) : this.zUpBoardSlot.GetFilledShape();
        if (other5 != null)
            BoardSlot.AddNewToSortedList(neighbors, other5);
        GameShape other6 = this.zDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.zDownBoardPos, bIgnoreSameFilledShape) : this.zDownBoardSlot.GetFilledShape();
        if (other6 == null)
            return;
        BoardSlot.AddNewToSortedList(neighbors, other6);
    }

    public void GetNeighborShapes(Dictionary<GameShape, int> neighbors)
    {
        bool bIgnoreSameFilledShape = true;
        GameShape other1 = this.xUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xUpBoardPos, bIgnoreSameFilledShape) : this.xUpBoardSlot.GetFilledShape();
        if (other1 != null)
            BoardSlot.AddOrIncrementDictSet(neighbors, other1);
        GameShape other2 = this.xDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.xDownBoardPos, bIgnoreSameFilledShape) : this.xDownBoardSlot.GetFilledShape();
        if (other2 != null)
            BoardSlot.AddOrIncrementDictSet(neighbors, other2);
        GameShape other3 = this.yUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yUpBoardPos, bIgnoreSameFilledShape) : this.yUpBoardSlot.GetFilledShape();
        if (other3 != null)
            BoardSlot.AddOrIncrementDictSet(neighbors, other3);
        GameShape other4 = this.yDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.yDownBoardPos, bIgnoreSameFilledShape) : this.yDownBoardSlot.GetFilledShape();
        if (other4 != null)
            BoardSlot.AddOrIncrementDictSet(neighbors, other4);
        GameShape other5 = this.zUpBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.zUpBoardPos, bIgnoreSameFilledShape) : this.zUpBoardSlot.GetFilledShape();
        if (other5 != null)
            BoardSlot.AddOrIncrementDictSet(neighbors, other5);
        GameShape other6 = this.zDownBoardSlot == null ? this.GetShapeInBoardSlotAtOffset(BoardSlot.zDownBoardPos, bIgnoreSameFilledShape) : this.zDownBoardSlot.GetFilledShape();
        if (other6 == null)
            return;
        BoardSlot.AddOrIncrementDictSet(neighbors, other6);
    }

    private static void AddNewToSortedList(SortedList<GameShape, int> shapeNeighbors, GameShape other)
    {
        int num;
        if (other == null || shapeNeighbors.TryGetValue(other, out num))
            return;
        shapeNeighbors.Add(other, 1);
    }

    private static void AddOrIncrementDictSet(Dictionary<GameShape, int> shapeNeighbors, GameShape other)
    {
        if (other == null)
            return;
        int num;
        if (shapeNeighbors.TryGetValue(other, out num))
        {
            ++num;
            shapeNeighbors[other] = num;
        }
        else
            shapeNeighbors.Add(other, 1);
    }

    private GameShape GetShapeInBoardSlotAtOffset(BoardPosition offset, bool bIgnoreSameFilledShape)
    {
        GameShape gameShape = (GameShape)null;
        BoardPosition pos = BoardPosition.AddBoardPositions(this.boardPos, offset);
        if (this.ParentGameBoard.CreateSafeBoardPosition(ref pos))
        {
            BoardSlot boardSlotForPosition = this.ParentGameBoard.GetBoardSlotForPosition(pos);
            if (boardSlotForPosition != null && boardSlotForPosition.HasFilledShape() && (!bIgnoreSameFilledShape || bIgnoreSameFilledShape && boardSlotForPosition.FilledShape != this.FilledShape))
                gameShape = boardSlotForPosition.FilledShape;
        }
        return gameShape;
    }

    private GameShape GetShapeInBoardSlotAtOffset(BoardPosition offset)
    {
        GameShape gameShape = (GameShape)null;
        BoardPosition pos = BoardPosition.AddBoardPositions(this.boardPos, offset);
        if (this.ParentGameBoard.CreateSafeBoardPosition(ref pos))
        {
            BoardSlot boardSlotForPosition = this.ParentGameBoard.GetBoardSlotForPosition(pos);
            if (boardSlotForPosition != null)
                gameShape = boardSlotForPosition.FilledShape;
        }
        return gameShape;
    }

    protected enum EFetchDirection
    {
        X_Up,
        X_Down,
        Y_Up,
        Y_Down,
        Z_Up,
        Z_Down,
    }
}
