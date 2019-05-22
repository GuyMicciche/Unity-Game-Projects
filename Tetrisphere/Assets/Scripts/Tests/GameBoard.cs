// Decompiled with JetBrains decompiler
// Type: GameBoard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ADDEA9C9-AEE1-4DC1-B840-76EBFDF8AEDE
// Assembly location: C:\Users\Guy\Desktop\sc\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard
{
    public int BoardXSize = -1;
    public int BoardYSize = -1;
    public int BoardZSize = -1;
    public bool WrapXAxis = true;
    public bool WrapYAxis = true;
    public List<GameShape> ShapeList = new List<GameShape>();
    public BoardPosition GravityDirection = new BoardPosition();
    private int TopGravityLayerIndexZ = -1;
    private int LowestGravityLayerIndexZ = -1;
    private Dictionary<EShapeLayoutType, int> ShapeLayoutTypesOnBoard = new Dictionary<EShapeLayoutType, int>();
    public BoardSlot[, ,] TheGameBoard;
    public byte[,] VerticalColumnShapeTracker;
    protected bool BoardInitialized;
    public bool WrapZAxis;
    public bool GravityEnabled;
    private int ReservedShapeLayerIndexZ;
    private int ExtraZPadForLoadedBoard;

    public bool InitializeGameBoard(int xSize, int ySize, int zSize)
    {
        if (!this.IsBoardInitialized())
        {
            if (this.AllocateGameBoard(xSize, ySize, zSize))
            {
                for (int i = 0; i < this.BoardXSize; i++)
                {
                    for (int k = 0; k < this.BoardYSize; k++)
                    {
                        for (int m = 0; m < this.BoardZSize; m++)
                        {
                            this.TheGameBoard.SetValue(new BoardSlot(i, k, m, this), i, k, m);
                        }
                    }
                }
                BoardSlot[, ,] theGameBoard = this.TheGameBoard;
                int length = theGameBoard.GetLength(0);
                int num7 = theGameBoard.GetLength(1);
                int num9 = theGameBoard.GetLength(2);
                for (int j = 0; j < length; j++)
                {
                    for (int n = 0; n < num7; n++)
                    {
                        for (int num8 = 0; num8 < num9; num8++)
                        {
                            theGameBoard[j, n, num8].SetupDirectNeighborPointers();
                        }
                    }
                }
                this.BoardInitialized = true;
                //NakaiDebug.CustomDebugEditorLog(string.Concat(new object[] { "Allocated game board with X:", this.BoardXSize, " Y:", this.BoardYSize, " Z:", this.BoardZSize, " which created array of size ", this.TheGameBoard.Length }));
                return true;
            }
            return false;
        }
        //NakaiDebug.CustomDebugLogError("Tried to call InitializeGameBoard() on a board that is already initialized");
        return false;
    }

    private bool AllocateGameBoard(int xSize, int ySize, int zSize)
    {
        bool flag;
        if (xSize > 0 && ySize > 0 && zSize > 0)
        {
            this.TheGameBoard = new BoardSlot[xSize, ySize, zSize];
            this.BoardXSize = xSize;
            this.BoardYSize = ySize;
            this.BoardZSize = zSize;
            this.VerticalColumnShapeTracker = new byte[xSize, ySize];
            flag = true;
        }
        else
        {
            //NakaiDebug.CustomDebugLogError(string.Format("Tried to allocate a game board with invalid dimensions x:{0} y:{1} z:{2}", (object)xSize, (object)xSize, (object)zSize));
            flag = false;
        }
        return flag;
    }

    public bool IsBoardInitialized()
    {
        return this.BoardInitialized;
    }

    public void IncrementVerticalColumnShapeTracker(BoardPosition clampedPos)
    {
        ++this.VerticalColumnShapeTracker[clampedPos.X, clampedPos.Y];
    }

    public void DecrementVerticalColumnShapeTracker(BoardPosition clampedPos)
    {
        --this.VerticalColumnShapeTracker[clampedPos.X, clampedPos.Y];
    }

    public bool GetNumberFilledBoardSlotsForXY(BoardPosition XYPos, out byte resultValue)
    {
        bool flag = false;
        XYPos.Z = 0;
        BoardPosition outPosition;
        if (this.CreateSafeBoardPosition(XYPos, out outPosition))
        {
            resultValue = this.VerticalColumnShapeTracker[outPosition.X, outPosition.Y];
            flag = true;
        }
        else
            resultValue = (byte)0;
        return flag;
    }

    public BoardPosition GetGameBoardDimensions()
    {
        return new BoardPosition(this.BoardXSize, this.BoardYSize, this.BoardZSize);
    }

    public BoardSlot GetBoardSlotForPosition(BoardPosition pos)
    {
        BoardSlot boardSlot = (BoardSlot)null;
        if (this.IsSafeBoardPosition(pos))
        {
            boardSlot = this.TheGameBoard[pos.X, pos.Y, pos.Z];
            if (boardSlot == null)
            { 
                //NakaiDebug.CustomDebugLogError("Failed to get valid board slot for position " + (object)pos);\
            }
        }
        return boardSlot;
    }

    public bool IsReservedLayerZ(int LayerIndex)
    {
        bool flag = false;
        if (LayerIndex == this.ReservedShapeLayerIndexZ)
            flag = true;
        return flag;
    }

    public void NormalizeBoardPosition(BoardPosition inPosition, out BoardPosition outPosition)
    {
        outPosition = inPosition;
        if (outPosition.X < 0 || outPosition.X >= this.BoardXSize)
            outPosition.X = this.HandleBoardWrap(outPosition.X, 0, this.BoardXSize);
        if (outPosition.Y < 0 || outPosition.Y >= this.BoardYSize)
            outPosition.Y = this.HandleBoardWrap(outPosition.Y, 0, this.BoardYSize);
        if (outPosition.Z >= 0 && outPosition.Z < this.BoardZSize)
            return;
        outPosition.Z = this.HandleBoardWrap(outPosition.Z, 0, this.BoardZSize);
    }

    public bool CreateSafeBoardPosition(ref BoardPosition pos)
    {
        return this.CreateSafeBoardPosition(pos, out pos);
    }

    public bool CreateSafeBoardPosition(BoardPosition inPosition, out BoardPosition outPosition)
    {
        bool flag1 = true;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        outPosition = inPosition;
        if (inPosition.X < 0 || inPosition.X >= this.BoardXSize)
        {
            if (this.WrapXAxis)
                outPosition.X = this.HandleBoardWrap(inPosition.X, 0, this.BoardXSize);
            else
                flag2 = true;
        }
        if (inPosition.Y < 0 || inPosition.Y >= this.BoardYSize)
        {
            if (this.WrapYAxis)
                outPosition.Y = this.HandleBoardWrap(inPosition.Y, 0, this.BoardYSize);
            else
                flag3 = true;
        }
        if (inPosition.Z < 0 || inPosition.Z >= this.BoardZSize)
        {
            if (this.WrapZAxis)
                outPosition.Z = this.HandleBoardWrap(inPosition.Z, 0, this.BoardZSize);
            else
                flag4 = true;
        }
        if (flag2 || flag3 || flag4)
            flag1 = false;
        return flag1;
    }
    
    public BoardSlot GetBoardSlotForPositionOffset(BoardSlot anchor, BoardPosition offset, bool wrapX, bool wrapY, bool wrapZ, bool logWrapErrors)
    {
        BoardSlot slot = null;
        if (anchor == null)
        {
            return slot;
        }
        BoardPosition clampedPos = anchor.BoardPos + offset;
        if ((clampedPos.X < 0) || (clampedPos.X >= this.BoardXSize))
        {
            if (wrapX)
            {
                clampedPos.X = this.HandleBoardWrap(clampedPos.X, 0, this.BoardXSize);
            }
            else if (logWrapErrors)
            {
                //NakaiDebug.CustomDebugLogWarning("Invalid x");
            }
        }
        if ((clampedPos.Y < 0) || (clampedPos.Y >= this.BoardYSize))
        {
            if (wrapY)
            {
                clampedPos.Y = this.HandleBoardWrap(clampedPos.Y, 0, this.BoardYSize);
            }
            else if (logWrapErrors)
            {
                //NakaiDebug.CustomDebugLogWarning("Invalid y");
            }
        }
        if ((clampedPos.Z < 0) || (clampedPos.Z >= this.BoardZSize))
        {
            if (wrapZ)
            {
                clampedPos.Z = this.HandleBoardWrap(clampedPos.Z, 0, this.BoardZSize);
            }
            else if (logWrapErrors)
            {
                //NakaiDebug.CustomDebugLogWarning("Invalid z");
            }
        }
        if (this.IsSafeBoardPosition(clampedPos))
        {
            return this.GetBoardSlotForPosition(clampedPos);
        }
        if (logWrapErrors)
        {
            //NakaiDebug.CustomDebugLogWarning(string.Concat(new object[] { "Failed to create a safe board position for ", clampedPos, ", wrapping may need to be turned on: ", this.WrapXAxis, " ", this.WrapYAxis, " ", this.WrapZAxis }));
        }
        return null;
    }

    public int HandleBoardWrap(int inValue, int lowendExclusive, int highendInclusive)
    {
        int num1;
        if (inValue < lowendExclusive)
        {
            int num2 = Math.Abs(inValue) % highendInclusive;
            num1 = num2 != 0 ? highendInclusive - num2 : 0;
        }
        else
            num1 = inValue % highendInclusive;
        return num1;
    }

    public bool ShapeIsOnBoard(GameShape eval)
    {
        bool flag = false;
        if (this.ShapeList.Contains(eval))
            flag = true;
        return flag;
    }

    public int GetShapesOnBoardCount()
    {
        return this.ShapeList.Count;
    }

    public bool GetLowestUniformEmptyBoardSlotsForLayout(BoardPosition currentBoardPos, GameShape HACK_ShapeForLayout, ref BoardPosition newAnchorPos, List<BoardSlot> Results)
    {
        bool flag1 = false;
        BoardPosition pos = currentBoardPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            BoardPosition PreviewPosition = new BoardPosition();
            if (this.DetermineNewShapePositionForPreviewShape(HACK_ShapeForLayout, pos, ref PreviewPosition, true))
            {
                newAnchorPos = PreviewPosition;
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(PreviewPosition);
                List<BoardPosition> layoutFromAnchor = HACK_ShapeForLayout.GetShapeLayoutFromAnchor();
                bool flag2 = false;
                for (int index = 0; index < layoutFromAnchor.Count; ++index)
                {
                    BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, layoutFromAnchor[index], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
                    if (forPositionOffset != null)
                    {
                        Results.Add(forPositionOffset);
                    }
                    else
                    {
                        //NakaiDebug.CustomDebugLogError("Encountered a null slot for position " + (boardSlotForPosition.BoardPos + layoutFromAnchor[index]).ToString());
                        flag2 = true;
                        break;
                    }
                }
                flag1 = !flag2;
            }
        }
        else
        {
            //NakaiDebug.CustomDebugLogError("Failed to create safe board position for " + (object)currentBoardPos);
        }
        return flag1;
    }

    public bool GetLowestNonUniformEmptyBoardSlotsForLayout(BoardPosition currentBoardPos, GameShape HACK_ShapeForLayout, ref List<BoardSlot> Results)
    {
        bool flag = false;
        BoardPosition pos = currentBoardPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
            List<BoardPosition> layoutFromAnchor = HACK_ShapeForLayout.GetShapeLayoutFromAnchor();
            for (int index = 0; index < layoutFromAnchor.Count; ++index)
            {
                BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, layoutFromAnchor[index], this.WrapXAxis, this.WrapYAxis, false, true);
                BoardSlot slot = (BoardSlot)null;
                this.GetTopEmptyBoardSlotForXY(forPositionOffset.BoardPos, ref slot);
                Results.Add(slot);
            }
        }
        else
        {
            //NakaiDebug.CustomDebugLogError("Failed to create safe board position for " + (object)currentBoardPos);
        }
        return flag;
    }

    public bool GetTopEmptyBoardSlotForXY(BoardPosition XYPos, ref BoardSlot slot)
    {
        bool flag = false;
        BoardPosition pos = XYPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            slot = (BoardSlot)null;
            do
            {
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
                if (!boardSlotForPosition.HasFilledShape())
                {
                    slot = boardSlotForPosition;
                    ++pos.Z;
                }
                else
                    break;
            }
            while (pos.Z < this.BoardZSize);
            flag = true;
        }
        return flag;
    }

    public int GetAllShapesForXY(BoardPosition XYPos, List<GameShape> shapesResultList, bool checkForDuplicatesInResultList)
    {
        int num = 0;
        BoardPosition pos = XYPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            do
            {
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
                if (boardSlotForPosition.HasFilledShape())
                {
                    GameShape filledShape = boardSlotForPosition.GetFilledShape();
                    if (checkForDuplicatesInResultList)
                    {
                        if (!shapesResultList.Contains(filledShape))
                        {
                            shapesResultList.Add(filledShape);
                            ++num;
                        }
                    }
                    else
                    {
                        shapesResultList.Add(filledShape);
                        ++num;
                    }
                }
                ++pos.Z;
            }
            while (pos.Z < this.BoardZSize);
        }
        return num;
    }

    public GameShape GetFirstShapeForXY(BoardPosition XYPos)
    {
        GameShape gameShape = (GameShape)null;
        BoardPosition pos = XYPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            do
            {
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
                if (boardSlotForPosition.HasFilledShape())
                {
                    gameShape = boardSlotForPosition.GetFilledShape();
                    break;
                }
                else
                    ++pos.Z;
            }
            while (pos.Z < this.BoardZSize);
        }
        return gameShape;
    }

    public GameShape GetFirstShapeForXYAnchorAndLayout(BoardPosition XYAnchorPos, EShapeLayoutType layoutType)
    {
        GameShape gameShape = (GameShape)null;
        BoardPosition pos = XYAnchorPos;
        pos.Z = 0;
        if (this.CreateSafeBoardPosition(ref pos))
        {
            do
            {
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
                if (boardSlotForPosition.HasFilledShape())
                {
                    GameShape filledShape = boardSlotForPosition.GetFilledShape();
                    if (filledShape.ShapeLayoutType == layoutType && BoardPosition.EqualsXY(filledShape.GetAnchorBoardPosition(), pos))
                    {
                        gameShape = filledShape;
                        break;
                    }
                }
                ++pos.Z;
            }
            while (pos.Z < this.BoardZSize);
        }
        return gameShape;
    }

    public bool CreateSafeBoardPositionX(int boardPosX, out int safeBoardPosX, bool wrapAxis)
    {
        safeBoardPosX = boardPosX;
        bool flag;
        if (this.IsSafeBoardPositionX(boardPosX))
            flag = true;
        else if (wrapAxis)
        {
            safeBoardPosX = this.HandleBoardWrap(boardPosX, 0, this.BoardXSize);
            flag = true;
        }
        else
            flag = false;
        return flag;
    }

    public bool CreateSafeBoardPositionX(int boardPosX, out int safeBoardPosX)
    {
        return this.CreateSafeBoardPositionX(boardPosX, out safeBoardPosX, this.WrapXAxis);
    }

    public bool CreateSafeBoardPositionY(int boardPosY, out int safeBoardPosY, bool wrapAxis)
    {
        safeBoardPosY = boardPosY;
        bool flag;
        if (this.IsSafeBoardPositionY(boardPosY))
            flag = true;
        else if (wrapAxis)
        {
            safeBoardPosY = this.HandleBoardWrap(boardPosY, 0, this.BoardYSize);
            flag = true;
        }
        else
            flag = false;
        return flag;
    }

    public bool CreateSafeBoardPositionY(int boardPosY, out int safeBoardPosY)
    {
        return this.CreateSafeBoardPositionY(boardPosY, out safeBoardPosY, this.WrapYAxis);
    }

    public bool CreateSafeBoardPositionZ(int boardPosZ, out int safeBoardPosZ, bool wrapAxis)
    {
        safeBoardPosZ = boardPosZ;
        bool flag;
        if (this.IsSafeBoardPositionZ(boardPosZ))
            flag = true;
        else if (wrapAxis)
        {
            safeBoardPosZ = this.HandleBoardWrap(boardPosZ, 0, this.BoardZSize);
            flag = true;
        }
        else
            flag = false;
        return flag;
    }

    public bool CreateSafeBoardPositionZ(int boardPosZ, out int safeBoardPosZ)
    {
        return this.CreateSafeBoardPositionZ(boardPosZ, out safeBoardPosZ, this.WrapZAxis);
    }

    public bool IsSafeBoardPosition(BoardPosition clampedPos)
    {
        bool flag = false;
        if (clampedPos.X >= 0 && clampedPos.X < this.BoardXSize && (clampedPos.Y >= 0 && clampedPos.Y < this.BoardYSize) && (clampedPos.Z >= 0 && clampedPos.Z < this.BoardZSize))
            flag = true;
        return flag;
    }

    public bool IsSafeBoardPositionX(int boardPosX)
    {
        bool flag = false;
        if (boardPosX >= 0 && boardPosX < this.BoardXSize)
            flag = true;
        return flag;
    }

    public bool IsSafeBoardPositionY(int boardPosY)
    {
        bool flag = false;
        if (boardPosY >= 0 && boardPosY < this.BoardYSize)
            flag = true;
        return flag;
    }

    public bool IsSafeBoardPositionZ(int boardPosZ)
    {
        bool flag = false;
        if (boardPosZ >= 0 && boardPosZ < this.BoardZSize)
            flag = true;
        return flag;
    }

    //public bool CanFitShapeLayoutNoEncroachment(EShapeLayoutType layoutType, BoardPosition newPos)
    //{
    //    BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(newPos);
    //    bool flag1;
    //    if (boardSlotForPosition != null)
    //    {
    //        List<BoardPosition> list = ShapeFactory.GetStaticLayoutContainer(layoutType).ShapeLayoutFromAnchor;
    //        if (list != null)
    //        {
    //            bool flag2 = true;
    //            for (int index = 0; index < list.Count; ++index)
    //            {
    //                BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, list[index], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
    //                if (forPositionOffset != null)
    //                {
    //                    if (forPositionOffset.CanBlock())
    //                    {
    //                        flag2 = false;
    //                        break;
    //                    }
    //                }
    //                else
    //                {
    //                    flag2 = false;
    //                    //NakaiDebug.CustomDebugLogError("null testslot found for slot " + (object)(boardSlotForPosition.BoardPos + list[index]));
    //                    break;
    //                }
    //            }
    //            flag1 = flag2;
    //        }
    //        else
    //        {
    //            flag1 = false;
    //            //NakaiDebug.CustomDebugLogWarning("tried to test with a layout without a layout container:" + (object)layoutType);
    //        }
    //    }
    //    else
    //        flag1 = false;
    //    return flag1;
    //}

    public bool CanFitShape(GameShape newshape, BoardPosition newPos)
    {
        bool flag1 = false;
        if (newshape != null)
        {
            BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(newPos);
            if (boardSlotForPosition != null)
            {
                List<BoardPosition> layoutFromAnchor = newshape.GetShapeLayoutFromAnchor();
                bool flag2 = true;
                for (int index = 0; index < layoutFromAnchor.Count; ++index)
                {
                    BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, layoutFromAnchor[index], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
                    if (forPositionOffset != null)
                    {
                        if (forPositionOffset.CanBlock() && forPositionOffset.WillBlockShape(newshape))
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    else
                        flag2 = false;
                }
                flag1 = flag2;
            }
            else
                flag1 = false;
        }
        return flag1;
    }

    public bool HACK_CanFitShapeForGravityDrop(GameShape newshape, BoardPosition newPos)
    {
        bool flag1 = false;
        if (newshape != null)
        {
            BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(newPos);
            if (boardSlotForPosition != null)
            {
                List<BoardPosition> layoutFromAnchor = newshape.GetShapeLayoutFromAnchor();
                bool flag2 = true;
                for (int index = 0; index < layoutFromAnchor.Count; ++index)
                {
                    BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, layoutFromAnchor[index], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
                    if (forPositionOffset != null)
                    {
                        if (forPositionOffset.CanBlock())
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    else
                    {
                        flag2 = false;
                        //NakaiDebug.CustomDebugLogError("null testslot found for slot " + (object)(boardSlotForPosition.BoardPos + layoutFromAnchor[index]));
                        break;
                    }
                }
                flag1 = flag2;
            }
            else
                flag1 = false;
        }
        return flag1;
    }

    private bool MoveShapeOnBoardWithGravity(GameShape ShapeToMove, BoardPosition newAnchorPos)
    {
        bool flag = false;
        if (ShapeToMove != null && this.ShapeIsOnBoard(ShapeToMove))
        {
            ShapeToMove.UnregisterFromOccupiedSlots();
            this.PlaceShapeToAnchorPosition(ShapeToMove, newAnchorPos);
            ShapeToMove.NotifyMovedToNewAnchorPosition(newAnchorPos);
            flag = true;
        }
        return flag;
    }

    private bool MoveShapeOnBoardToNewAnchorPosition(GameShape ShapeToMove, BoardPosition newAnchorPosition, List<BoardSlot> ChangedSlots)
    {
        bool flag = false;
        if (ShapeToMove != null && this.ShapeIsOnBoard(ShapeToMove))
        {
            ShapeToMove.GetOccupiedSlotsCopy(ChangedSlots);
            ShapeToMove.UnregisterFromOccupiedSlots();
            this.PlaceShapeToAnchorPosition(ShapeToMove, newAnchorPosition);
            ShapeToMove.NotifyMovedToNewAnchorPosition(newAnchorPosition);
            flag = true;
        }
        return flag;
    }

    private void PlaceShapeToAnchorPosition(GameShape ShapeToMove, BoardPosition newAnchorPos)
    {
        IList<BoardPosition> list = (IList<BoardPosition>)ShapeToMove.GetShapeLayoutFromAnchor().AsReadOnly();
        BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(newAnchorPos);
        for (int forslotlayoutindex = 0; forslotlayoutindex < list.Count; ++forslotlayoutindex)
        {
            BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, list[forslotlayoutindex], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
            forPositionOffset.CommitFilledShape(ShapeToMove);
            ShapeToMove.AddOccupiedSlot(forPositionOffset, forslotlayoutindex);
        }
    }

    public bool MoveShapeOnBoardToNewAnchorPositionInEditor(GameShape shapeToMove, BoardPosition newAnchorPosition)
    {
        bool flag = false;
        if (shapeToMove != null && this.ShapeIsOnBoard(shapeToMove) && this.CanFitShape(shapeToMove, newAnchorPosition))
        {
            List<BoardSlot> ChangedSlots = new List<BoardSlot>();
            flag = this.MoveShapeOnBoardToNewAnchorPosition(shapeToMove, newAnchorPosition, ChangedSlots);
        }
        return flag;
    }

    public bool MoveShapeOnBoardWithPlayerGrabByOffsetInEditor(GameShape ShapeToMove, BoardPosition newAnchorPosOffset, bool CheckSupportedAtNewPosition)
    {
        bool flag = false;
        if (ShapeToMove != null)
        {
            BoardPosition inNewAnchorPos = ShapeToMove.GetAnchorBoardPosition() + newAnchorPosOffset;
            flag = this.MoveShapeOnBoardWithPlayerGrab(ShapeToMove, inNewAnchorPos, CheckSupportedAtNewPosition, true);
        }
        return flag;
    }

    public bool MoveShapeOnBoardWithPlayerGrabByOffset(GameShape ShapeToMove, BoardPosition newAnchorPosOffset, bool CheckSupportedAtNewPosition)
    {
        bool flag = false;
        if (ShapeToMove != null)
        {
            BoardPosition inNewAnchorPos = ShapeToMove.GetAnchorBoardPosition() + newAnchorPosOffset;
            flag = this.MoveShapeOnBoardWithPlayerGrab(ShapeToMove, inNewAnchorPos, CheckSupportedAtNewPosition, false);
        }
        return flag;
    }

    public bool CanMoveShapeOnBoardWithPlayerGrabByOffset(GameShape shapeToMove, BoardPosition newAnchorPosOffset)
    {
        bool flag = false;
        if (shapeToMove != null && shapeToMove.CanMoveByPlayerGrab() && this.ShapeIsOnBoard(shapeToMove))
        {
            BoardPosition boardPosition = shapeToMove.GetAnchorBoardPosition() + newAnchorPosOffset;
            BoardPosition outPosition;
            if (this.CreateSafeBoardPosition(boardPosition, out outPosition))
                flag = this.CanFitShape(shapeToMove, boardPosition);
        }
        return flag;
    }

    public bool CanMoveShapeOnBoardWithPlayerGrabByOffsetFourWayCheck(GameShape shapeToMove, out bool xDown, out bool xUp, out bool yDown, out bool yUp)
    {
        bool flag = false;
        xDown = false;
        xUp = false;
        yDown = false;
        yUp = false;
        if (shapeToMove != null && shapeToMove.CanMoveByPlayerGrab() && this.ShapeIsOnBoard(shapeToMove))
        {
            BoardPosition boardPosition1 = shapeToMove.GetAnchorBoardPosition() + BoardSlot.xDownBoardPos;
            BoardPosition outPosition1;
            if (this.CreateSafeBoardPosition(boardPosition1, out outPosition1))
                xDown = this.CanFitShape(shapeToMove, boardPosition1);
            BoardPosition boardPosition2 = shapeToMove.GetAnchorBoardPosition() + BoardSlot.xUpBoardPos;
            BoardPosition outPosition2;
            if (this.CreateSafeBoardPosition(boardPosition2, out outPosition2))
                xUp = this.CanFitShape(shapeToMove, boardPosition2);
            BoardPosition boardPosition3 = shapeToMove.GetAnchorBoardPosition() + BoardSlot.yDownBoardPos;
            BoardPosition outPosition3;
            if (this.CreateSafeBoardPosition(boardPosition3, out outPosition3))
                yDown = this.CanFitShape(shapeToMove, boardPosition3);
            BoardPosition boardPosition4 = shapeToMove.GetAnchorBoardPosition() + BoardSlot.yUpBoardPos;
            BoardPosition outPosition4;
            if (this.CreateSafeBoardPosition(boardPosition4, out outPosition4))
                yUp = this.CanFitShape(shapeToMove, boardPosition4);
            flag = true;
        }
        return flag;
    }

    private bool MoveShapeOnBoardWithPlayerGrab(GameShape ShapeToMove, BoardPosition inNewAnchorPos, bool CheckSupportedAtNewPosition, bool editorMode)
    {
        bool flag = false;
        if (ShapeToMove != null && this.ShapeIsOnBoard(ShapeToMove) && (ShapeToMove.CanMoveByPlayerGrab() || editorMode))
        {
            BoardPosition pos = inNewAnchorPos;
            if (this.CreateSafeBoardPosition(ref pos) && this.CanFitShape(ShapeToMove, pos))
            {
                List<BoardSlot> ChangedSlots = new List<BoardSlot>();
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(pos);
                List<BoardPosition> layoutFromAnchor = ShapeToMove.GetShapeLayoutFromAnchor();
                List<GameShape> list1 = (List<GameShape>)null;
                for (int index = 0; index < layoutFromAnchor.Count; ++index)
                {
                    BoardSlot forPositionOffset = this.GetBoardSlotForPositionOffset(boardSlotForPosition, layoutFromAnchor[index], this.WrapXAxis, this.WrapYAxis, this.WrapZAxis, true);
                    if (forPositionOffset.HasFilledShape() && forPositionOffset.GetFilledShape() != ShapeToMove)
                    {
                        GameShape filledShape = forPositionOffset.GetFilledShape();
                        if (list1 == null)
                            list1 = new List<GameShape>();
                        if (!list1.Contains(filledShape))
                            list1.Add(filledShape);
                    }
                }
                if (list1 != null && list1.Count > 0)
                {
                    int points = 0;
                    for (int index = 0; index < list1.Count; ++index)
                    {
                        GameShape ShapeToRemove = list1[index];
                        points += ShapeToRemove.GetBasePointValue();
                        if (!this.RemoveShapeFromBoardByEncroachmentBreak(ShapeToRemove, ChangedSlots))
                        {
                            //NakaiDebug.CustomDebugLogError("Critical failure removing breakshape " + (object)ShapeToRemove);
                        }
                    }
                    //ScoreEventPlayerMoveBreakPoints playerMoveBreakPoints = new ScoreEventPlayerMoveBreakPoints(points);
                    //Engine_NotificationManager.SharedInstance.SendNotification(ScoreEvents.ScoreEvent_PlayerMoveBreak, (Component)null, (NMEventArgs)playerMoveBreakPoints);
                }
                this.MoveShapeOnBoardToNewAnchorPosition(ShapeToMove, pos, ChangedSlots);
                if (CheckSupportedAtNewPosition && this.IsGravityEnabled() && (ShapeToMove.CanGravityDrop() && !ShapeToMove.IsSupportedInGravityDirection(this.GravityDirection)))
                {
                    BoardPosition newAnchorPos = this.DetermineNewShapePositionFromGravityDrop(ShapeToMove);
                    this.MoveShapeOnBoardWithGravity(ShapeToMove, newAnchorPos);
                }
                if (this.IsGravityEnabled())
                {
                    BoardPosition UpDirection = -this.GravityDirection;
                    List<GameShape> list2 = new List<GameShape>();
                    this.GetPotentialDropShapesForChangedSlots(ChangedSlots, UpDirection, list2);
                    if (list2.Count > 0)
                    {
                        SortedDictionary<int, List<GameShape>> ShapesOnPlane = new SortedDictionary<int, List<GameShape>>();
                        this.CalculateShapeGravityDropMapping(list2, ShapesOnPlane);
                        this.ExecuteGravityDropForShapes(ShapesOnPlane);
                    }
                }
                flag = true;
            }
        }
        return flag;
    }

    public void PostShatterGravityDrop(List<BoardSlot> ChangedSlots)
    {
        List<GameShape> list = new List<GameShape>();
        this.GetPotentialGravityDropShapesForChangedSlots(ChangedSlots, list);
        if (list.Count <= 0)
            return;
        SortedDictionary<int, List<GameShape>> ShapesOnPlane = new SortedDictionary<int, List<GameShape>>();
        this.CalculateShapeGravityDropMapping(list, ShapesOnPlane);
        this.ExecuteGravityDropForShapes(ShapesOnPlane);
        //Engine_NotificationManager.SharedInstance.SendNotification(GeneralEvents.Notification_RequestShapeOcclusionRecalcFast, (Component)null, (NMEventArgs)null);
    }

    private void ExecuteGravityDropForShapes(SortedDictionary<int, List<GameShape>> ShapesOnPlane)
    {
        int num = 0;
        using (SortedDictionary<int, List<GameShape>>.Enumerator enumerator = ShapesOnPlane.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                List<GameShape> list = enumerator.Current.Value;
                num += list.Count;
                for (int index = 0; index < list.Count; ++index)
                {
                    GameShape gameShape = list[index];
                    BoardPosition newAnchorPos = this.DetermineNewShapePositionFromGravityDrop(gameShape);
                    this.MoveShapeOnBoardWithGravity(gameShape, newAnchorPos);
                }
            }
        }
    }

    private BoardPosition DetermineNewShapePositionFromGravityDrop(GameShape DroppingShape)
    {
        BoardPosition anchorBoardPosition1 = DroppingShape.GetAnchorBoardPosition();
        BoardPosition anchorBoardPosition2 = DroppingShape.GetAnchorBoardPosition();
        int num = 0;
        BoardPosition boardPosition;
        bool flag;
        do
        {
            boardPosition = anchorBoardPosition2;
            anchorBoardPosition2 += this.GravityDirection;
            flag = this.HACK_CanFitShapeForGravityDrop(DroppingShape, anchorBoardPosition2);
            ++num;
        }
        while (flag && num <= this.BoardZSize);
        if (num > this.BoardZSize)
        {
            boardPosition = anchorBoardPosition1;
            //NakaiDebug.CustomDebugLogWarning("iteration count exceeded");
        }
        return boardPosition;
    }

    public bool DetermineNewShapePositionForPreviewShape(GameShape PreviewShape, BoardPosition StartPosition, ref BoardPosition PreviewPosition, bool HACK_AllowReservedLayerPosition)
    {
        bool flag1 = false;
        if (PreviewShape != null)
        {
            BoardPosition newPos = StartPosition;
            BoardPosition boardPosition = StartPosition;
            int num = 0;
            bool flag2;
            do
            {
                flag2 = this.HACK_CanFitShapeForGravityDrop(PreviewShape, newPos);
                ++num;
                if (flag2)
                {
                    boardPosition = newPos;
                    newPos += this.GravityDirection;
                }
            }
            while (flag2 && num <= this.BoardZSize);
            if (num > 1)
            {
                if (this.IsReservedLayerZ(boardPosition.Z) && !HACK_AllowReservedLayerPosition)
                {
                    flag1 = false;
                }
                else
                {
                    PreviewPosition = boardPosition;
                    flag1 = true;
                }
            }
            else
                flag1 = false;
        }
        else
        {
            //NakaiDebug.CustomDebugLogError("Null PreviewShape pointer given");
        }
        return flag1;
    }

    private void CalculateShapeGravityDropMapping(List<GameShape> inPotentialDroppers, SortedDictionary<int, List<GameShape>> ShapesOnPlane)
    {
        for (int i = 0; i < inPotentialDroppers.Count; i++)
        {
            GameShape dropper = inPotentialDroppers[i];
            if (((dropper != null) && dropper.CanGravityDrop()) && !dropper.IsSupportedInGravityDirection(this.GravityDirection))
            {
                int sortIndex = 0;
                if (this.GetGravitySortIndexForBoardPosition(dropper.GetAnchorBoardPosition(), ref sortIndex))
                {
                    this.AddShapeToSortedDictionary(sortIndex, dropper, ShapesOnPlane);
                }
            }
        }
        List<GameShape> shapes = new List<GameShape>();
        for (int j = 0; j < this.BoardZSize; j++)
        {
            List<GameShape> list2 = null;
            if (ShapesOnPlane.TryGetValue(j, out list2))
            {
                for (int k = 0; k < list2.Count; k++)
                {
                    GameShape shape2 = list2[k];
                    shapes.Clear();
                    if (shape2.GetTouchingShapesInDirection(-this.GravityDirection, shapes) > 0)
                    {
                        for (int m = 0; m < shapes.Count; m++)
                        {
                            GameShape shape3 = shapes[m];
                            if (shape3.CanGravityDrop() && !shape3.IsSupportedInGravityDirection(this.GravityDirection, list2))
                            {
                                int num7 = -1;
                                if (this.GetGravitySortIndexForBoardPosition(shape3.GetAnchorBoardPosition(), ref num7))
                                {
                                    if (num7 != (j + 1))
                                    {
                                        //NakaiDebug.CustomDebugLogWarning(string.Concat(new object[] { "Broken assumption regarding gravity drop logic with shapes only spanning 1 z-plane PotentialDropperZ ", num7, " CurrentZ ", j }));
                                    }
                                    this.AddShapeToSortedDictionary(num7, shape3, ShapesOnPlane);
                                    //NakaiDebug.CustomDebugLog(string.Concat(new object[] { "Found cascaded shape ", shape3.GetType(), " with PotentialDropperZ ", num7 }));
                                }
                                else
                                {
                                    //NakaiDebug.CustomDebugLogError("Something bad happened here, potentially a bad board position " + shape3.GetAnchorBoardPosition());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void AddShapeToSortedDictionary(int SortIndex, GameShape dropper, SortedDictionary<int, List<GameShape>> ShapesOnPlane)
    {
        List<GameShape> list;
        if (ShapesOnPlane.TryGetValue(SortIndex, out list))
        {
            if (!list.Contains(dropper))
            {
                list.Add(dropper);
            }
        }
        else
        {
            list = new List<GameShape> {
            dropper
        };
            ShapesOnPlane.Add(SortIndex, list);
        }
    }

    private void GravitySorting()
    {
        int num = 0;
        if (this.GravityDirection.X > 0)
            ++num;
        else if (this.GravityDirection.X < 0)
            ++num;
        if (this.GravityDirection.Y > 0)
            ++num;
        else if (this.GravityDirection.Y < 0)
            ++num;
        if (this.GravityDirection.Z > 0)
            ++num;
        else if (this.GravityDirection.Z < 0)
            ++num;
        if (num <= 1)
            return;
        //NakaiDebug.CustomDebugLogError("Multi-axis gravity force, cannot handle that.  Gravity is " + (object)this.GravityDirection);
    }

    public bool GetGravitySortIndexForBoardPosition(BoardPosition inPos, ref int SortIndex)
    {
        bool flag = false;
        if (!this.IsSafeBoardPosition(inPos))
        {
            //NakaiDebug.CustomDebugLogError("invalid boardpos passed in for " + (object)inPos);
            return false;
        }
        else
        {
            if (this.GravityDirection.X > 0)
            {
                SortIndex = this.BoardXSize - 1 - inPos.X;
                flag = true;
            }
            else if (this.GravityDirection.X < 0)
            {
                SortIndex = inPos.X;
                flag = true;
            }
            else if (this.GravityDirection.Y > 0)
            {
                SortIndex = this.BoardYSize - 1 - inPos.Y;
                flag = true;
            }
            else if (this.GravityDirection.Y < 0)
            {
                SortIndex = inPos.Y;
                flag = true;
            }
            else if (this.GravityDirection.Z > 0)
            {
                SortIndex = this.BoardZSize - 1 - inPos.Z;
                flag = true;
            }
            else if (this.GravityDirection.Z < 0)
            {
                SortIndex = inPos.Z;
                flag = true;
            }
            return flag;
        }
    }

    public bool GetPotentialGravityDropShapesForChangedSlots(List<BoardSlot> ChangedSlots, List<GameShape> PotentialDroppers)
    {
        bool flag = false;
        if (this.IsGravityEnabled())
        {
            BoardPosition UpDirection = -this.GravityDirection;
            this.GetPotentialDropShapesForChangedSlots(ChangedSlots, UpDirection, PotentialDroppers);
            flag = true;
        }
        return flag;
    }

    public void GetPotentialDropShapesForChangedSlots(List<BoardSlot> ChangedSlots, BoardPosition UpDirection, List<GameShape> PotentialDroppers)
    {
        for (int index = 0; index < ChangedSlots.Count; ++index)
        {
            BoardSlot boardSlot = ChangedSlots[index];
            if (boardSlot != null && !boardSlot.HasFilledShape())
            {
                BoardSlot boardSlotForPosition = this.GetBoardSlotForPosition(boardSlot.BoardPos + UpDirection);
                if (boardSlotForPosition != null && boardSlotForPosition.HasFilledShape())
                {
                    GameShape filledShape = boardSlotForPosition.GetFilledShape();
                    if (!PotentialDroppers.Contains(filledShape))
                        PotentialDroppers.Add(filledShape);
                }
            }
        }
    }

    private void AddOrIncrementShapeLayoutTypeOnBoard(EShapeLayoutType layoutType)
    {
        int num1;
        if (this.ShapeLayoutTypesOnBoard.TryGetValue(layoutType, out num1))
        {
            int num2 = num1 + 1;
            this.ShapeLayoutTypesOnBoard[layoutType] = num2;
        }
        else
            this.ShapeLayoutTypesOnBoard.Add(layoutType, 1);
    }

    private void DecrementShapeLayoutTypeOnBoard(EShapeLayoutType layoutType)
    {
        int num1;
        if (!this.ShapeLayoutTypesOnBoard.TryGetValue(layoutType, out num1))
            return;
        int num2 = num1 - 1;
        this.ShapeLayoutTypesOnBoard[layoutType] = num2;
    }

    public int GetCountForLayoutTypeOnBoard(EShapeLayoutType layoutType)
    {
        int num1 = 0;
        int num2;
        if (this.ShapeLayoutTypesOnBoard.TryGetValue(layoutType, out num2))
            num1 = num2;
        return num1;
    }

    public bool CopyLayoutTypesOnBoardDictionary(Dictionary<EShapeLayoutType, int> copyTarget, ref int TotalShapesOnBoard)
    {
        bool flag = false;
        if (copyTarget != null)
        {
            using (Dictionary<EShapeLayoutType, int>.Enumerator enumerator = this.ShapeLayoutTypesOnBoard.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    KeyValuePair<EShapeLayoutType, int> current = enumerator.Current;
                    if (copyTarget.ContainsKey(current.Key))
                        copyTarget[current.Key] = current.Value;
                    else
                        copyTarget.Add(current.Key, current.Value);
                }
            }
            TotalShapesOnBoard = this.ShapeList.Count;
            flag = true;
        }
        return flag;
    }

    public void PrintLayoutTypeInfo()
    {
        foreach (KeyValuePair<EShapeLayoutType, int> pair in this.ShapeLayoutTypesOnBoard)
        {
            //NakaiDebug.CustomDebugLog(string.Concat(new object[] { "layout type ", pair.Key, " count ", pair.Value }));
        }
    }

    public bool AddShapeToBoard(GameShape newshape, BoardPosition newAnchorPos)
    {
        if ((newshape != null) && !this.ShapeIsOnBoard(newshape))
        {
            if (this.HACK_CanFitShapeForGravityDrop(newshape, newAnchorPos))
            {
                this.PlaceShapeToAnchorPosition(newshape, newAnchorPos);
                this.ShapeList.Add(newshape);
                this.AddOrIncrementShapeLayoutTypeOnBoard(newshape.ShapeLayoutType);
                newshape.NotifyAddedAtAnchorPosition(newAnchorPos, this);
                return true;
            }
            //NakaiDebug.CustomDebugLog(string.Concat(new object[] { "May NOT add shape ", newshape, " at anchor ", newAnchorPos }));
        }
        return false;
    }

    public bool RemoveShapeFromBoardByEncroachmentBreak(GameShape ShapeToRemove, List<BoardSlot> ChangedSlots)
    {
        ShapeToRemove.GetOccupiedSlotsCopy(ChangedSlots);
        return this.RemoveShapeFromBoard(ShapeToRemove);
    }

    public bool RemoveShapefromBoardByShatterComboProximityBreak(GameShape ShapeToRemove)
    {
        return this.RemoveShapeFromBoard(ShapeToRemove);
    }

    public bool RemoveShapeFromBoardByShatterComboBreak(GameShape ShapeToRemove)
    {
        return this.RemoveShapeFromBoard(ShapeToRemove);
    }

    public bool RemoveShapeFromBoardByShapeBreaker(GameShape ShapeToRemove)
    {
        return this.RemoveShapeFromBoard(ShapeToRemove);
    }

    public bool RemoveShapeFromBoard(GameShape ShapeToRemove)
    {
        bool flag = false;
        if ((ShapeToRemove == null) || !this.ShapeIsOnBoard(ShapeToRemove))
        {
            return flag;
        }
        ShapeToRemove.NotifyPreRemoveFromBoard();
        bool flag2 = this.ShapeList.Remove(ShapeToRemove);
        this.DecrementShapeLayoutTypeOnBoard(ShapeToRemove.ShapeLayoutType);
        ShapeToRemove.UnregisterFromOccupiedSlots();
        ShapeToRemove.NotifyRemovedFromBoard();
        if (!flag2)
        {
            //NakaiDebug.CustomDebugLog(string.Concat(new object[] { "goodremove is ", flag2, " for shape ", ShapeToRemove, " at ", ShapeToRemove.GetAnchorBoardPosition() }));
        }
        return flag2;
    }

    public void SetupGravity(BoardPosition newGravity, bool newGravityEnabled)
    {
        if (this.IsBoardInitialized())
        {
            if (newGravity == new BoardPosition(0, 0, 1))
            {
                this.GravityDirection = newGravity;
                this.GravityEnabled = newGravityEnabled;
                this.ReservedShapeLayerIndexZ = 0;
                this.TopGravityLayerIndexZ = 0;
                this.LowestGravityLayerIndexZ = this.BoardZSize - 1;
                //NakaiDebug.CustomDebugLog(string.Concat(new object[] { "TopGravityLayerIndexZ:", this.TopGravityLayerIndexZ, " LowestGravityLayerIndexZ:", this.LowestGravityLayerIndexZ }));
            }
            else
            {
                //NakaiDebug.CustomDebugLogError("Currently only gravity of BoardPosition(0, 0, 1) is supported");
            }
        }
        else
        {
            //NakaiDebug.CustomDebugLogError("The board must be initialized before calling SetupGravity");
        }
    }

    public bool IsGravityEnabled()
    {
        return this.GravityEnabled;
    }

    public BoardPosition GetGravityDirection()
    {
        return this.GravityDirection;
    }

    //public bool LoadGameBoardFromDataStorage(BoardSavedData ds, bool HACK_CreatePreviewLayerNeverUseInEditMode)
    //{
    //    bool flag = false;
    //    if (ds != null)
    //    {
    //        if (!this.IsBoardInitialized())
    //        {
    //            SavedBoardPropertiesInfo info = ds.BoardSettings.LoadFixedup();
    //            if (HACK_CreatePreviewLayerNeverUseInEditMode)
    //            {
    //                this.ExtraZPadForLoadedBoard = 1;
    //            }
    //            int num = 0;
    //            if (this.InitializeGameBoard(info.BoardXSize, info.BoardYSize, info.BoardZSize + this.ExtraZPadForLoadedBoard))
    //            {
    //                for (int i = 0; i < ds.ShapesOnBoard.Count; i++)
    //                {
    //                    GameShape shape;
    //                    SavedShapeInfo loadData = ds.ShapesOnBoard[i];
    //                    BoardPosition newPos = new BoardPosition();
    //                    bool flag2 = false;
    //                    if (GameShape.LoadShape(loadData, out shape, out newPos))
    //                    {
    //                        newPos.Z += this.ExtraZPadForLoadedBoard;
    //                        flag2 = this.AddShapeToBoard(shape, newPos);
    //                    }
    //                    if (!flag2)
    //                    {
    //                        //NakaiDebug.CustomDebugLogError(string.Concat(new object[] { "Failed to load/add shape index:", i, " ", loadData.GetAsString(), " (potentially unreliable) addpos:", newPos.ToString(false) }));
    //                        num++;
    //                    }
    //                }
    //                if (num > 0)
    //                {
    //                    //NakaiDebug.CustomDebugLogError("failure to load board properly, encountered " + num + " shapes we were unable to load on the board because they didn't fit");
    //                    return flag;
    //                }
    //                return true;
    //            }
    //            //NakaiDebug.CustomDebugLogWarning(string.Concat(new object[] { "CreateGameBoard failed, improper dimensions probably given - x ", info.BoardXSize, " y ", info.BoardYSize, " z ", info.BoardZSize }));
    //            return flag;
    //        }
    //        //NakaiDebug.CustomDebugLogWarning("editor already exists, unable to load another");
    //        return flag;
    //    }
    //    //NakaiDebug.CustomDebugLogWarning("BoardSavedData passed in is null");
    //    return flag;
    //}
}
