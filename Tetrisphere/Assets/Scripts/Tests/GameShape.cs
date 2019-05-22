using System;
using System.Collections.Generic;
using UnityEngine;

public enum EShapeLayoutType
{
    None,
    ShapeL,
    ShapeHorizontalColumn,
    ShapeVerticalColumn,
    ShapeSquare,
    ShapeT,
    ShapeZ,
    ShapeSingleBlock,
}

public enum EShapeClassType
{
    None,
    Normal,
    Breakable,
    Indestructible,
    PlayerImmovable,
}


public abstract class GameShape
{
    protected List<BoardSlot> OccupiedSlots = new List<BoardSlot>();
    protected BoardPositionContainer AnchorPosition = new BoardPositionContainer();
    protected GameBoard ParentGameBoard;
    protected GameShapeProxyBase RenderProxy;
    protected ShapeLayoutBase ShapeLayoutContainer;
    public EShapeLayoutType ShapeLayoutType;
    public EShapeClassType ShapeClassType;
    protected ShapeAttributesContainer ShapeAttributes;
    protected bool InShatterCombo;
    protected int ShatterOwnerID;
    protected bool PendingDestroy;

    protected GameShape(EShapeLayoutType layoutType, ShapeLayoutBase layoutContainer)
    {
        this.ShapeLayoutType = layoutType;
        this.ShapeLayoutContainer = layoutContainer;
    }

    public bool LinkupRenderProxy(GameShapeProxyBase newProxy)
    {
        if ((newProxy != null) && (this.RenderProxy == null))
        {
            this.SetRenderProxy(newProxy);
            newProxy.RegisterPrimaryShape(this);
            return true;
        }
        Debug.LogError(string.Concat(new object[] { "Error newProxy ", newProxy != null, " RenderProxy ", this.RenderProxy != null }));
        return false;
    }

    protected void UpdateAnchorPositionForRenderProxy(BoardPosition newAnchorPos)
    {
        if (!((UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null))
            return;
        this.RenderProxy.ShapeMovedToNewAnchorPosition(newAnchorPos);
    }

    protected void ReleaseRenderProxy()
    {
        if (!((UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null))
            return;
        this.RenderProxy.UnRegisterPrimaryShape();
        this.RenderProxy = (GameShapeProxyBase)null;
    }

    protected void SetRenderProxy(GameShapeProxyBase inProxy)
    {
        this.RenderProxy = inProxy;
    }

    public GameShapeProxyBase GetRenderProxy()
    {
        return this.RenderProxy;
    }

    public bool HasRenderProxy()
    {
        return (UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null;
    }

    public List<BoardPosition> GetShapeLayoutFromAnchor()
    {
        return this.ShapeLayoutContainer.ShapeLayoutFromAnchor;
    }

    public IList<BoardPosition> GetShapeLayoutFromAnchorAsReadOnly()
    {
        return (IList<BoardPosition>)this.ShapeLayoutContainer.ShapeLayoutFromAnchor.AsReadOnly();
    }

    public BoardPosition GetAnchorBoardPosition()
    {
        return this.AnchorPosition.ToBoardPosition();
    }

    public BoardSlot GetAnchorSlot()
    {
        return this.OccupiedSlots[0];
    }

    public bool IsOnGameBoard()
    {
        return this.ParentGameBoard != null;
    }

    public ShapeAttributesContainer GetShapeAttributes()
    {
        return this.ShapeAttributes;
    }

    public EShapeLayoutType GetShapeLayoutType()
    {
        return this.ShapeLayoutType;
    }

    public EShapeClassType GetShapeClassType()
    {
        return this.ShapeClassType;
    }

    public int GetNumSlotsThisShapeCanFill()
    {
        return this.ShapeLayoutContainer.ShapeLayoutFromAnchor.Count;
    }

    public void AddOccupiedSlot(BoardSlot slot, int forslotlayoutindex)
    {
        this.OccupiedSlots.Add(slot);
    }

    public void GetOccupiedSlotsCopy(List<BoardSlot> inOccupiedSlots)
    {
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            BoardSlot boardSlot = this.OccupiedSlots[index];
            if (boardSlot != null && !inOccupiedSlots.Contains(boardSlot))
                inOccupiedSlots.Add(boardSlot);
        }
    }

    public void UnregisterFromOccupiedSlots()
    {
        for (int index = this.OccupiedSlots.Count - 1; index >= 0; --index)
        {
            if (this.OccupiedSlots[index] != null)
                this.OccupiedSlots[index].ClearFilledShape();
            this.OccupiedSlots.RemoveAt(index);
        }
    }

    public bool WillBlockShapeAtBoardSlot(GameShape encroachingShape, BoardSlot slot)
    {
        return encroachingShape != this && (this.ShapeClassType != EShapeClassType.Breakable || encroachingShape.ShapeClassType == EShapeClassType.Breakable || encroachingShape.ShapeClassType == EShapeClassType.None);
    }

    public void NotifyAddedAtAnchorPosition(BoardPosition newAnchorPos, GameBoard newboard)
    {
        this.ParentGameBoard = newboard;
        this.AnchorPosition.Set(newAnchorPos);
        if ((UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null)
            ;
    }

    public void NotifyMovedToNewAnchorPosition(BoardPosition newAnchorPos)
    {
        this.AnchorPosition.Set(newAnchorPos);
        this.UpdateAnchorPositionForRenderProxy(newAnchorPos);
    }

    public void NotifyPreRemoveFromBoard()
    {
        this.PendingDestroy = true;
        if (!((UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null))
            return;
        this.RenderProxy.NotifyPreRemoveFromBoard();
    }

    public void NotifyRemovedFromBoard()
    {
        this.ReleaseRenderProxy();
        this.ParentGameBoard = (GameBoard)null;
    }

    public void GetTouchingShapes(List<GameShape> ListToPopulate)
    {
        int count = this.OccupiedSlots.Count;
        for (int index = 0; index < count; ++index)
            this.OccupiedSlots[index].GetNeighborShapes(ListToPopulate);
    }

    public void GetTouchingShapes(SortedList<GameShape, int> sortedListToPopulate)
    {
        int count = this.OccupiedSlots.Count;
        for (int index = 0; index < count; ++index)
            this.OccupiedSlots[index].GetNeighborShapes(sortedListToPopulate);
    }

    public void GetTouchingShapesAndCounts(Dictionary<GameShape, int> DictToPopulate)
    {
        int count = this.OccupiedSlots.Count;
        for (int index = 0; index < count; ++index)
            this.OccupiedSlots[index].GetNeighborShapes(DictToPopulate);
    }

    public void GetTouchingShapesOfLayoutType(List<GameShape> ListToPopulate, EShapeLayoutType layoutType)
    {
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
            this.OccupiedSlots[index].GetNeighborShapes(ListToPopulate);
        for (int index = ListToPopulate.Count - 1; index >= 0; --index)
        {
            if (ListToPopulate[index].ShapeLayoutType != layoutType)
                ListToPopulate.RemoveAt(index);
        }
    }

    public void GetTouchingShapesOfShapeType(List<GameShape> ListToPopulate, EShapeClassType shapeType)
    {
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
            this.OccupiedSlots[index].GetNeighborShapes(ListToPopulate);
        for (int index = ListToPopulate.Count - 1; index >= 0; --index)
        {
            if (ListToPopulate[index].ShapeClassType != shapeType)
                ListToPopulate.RemoveAt(index);
        }
    }

    public int GetTouchingShapesInDirection(BoardPosition direction, List<GameShape> shapes)
    {
        int num = 0;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape shapeInDirection = this.OccupiedSlots[index].GetNeighborShapeInDirection(direction);
            if (shapeInDirection != null && shapeInDirection != this)
            {
                ++num;
                if (!shapes.Contains(shapeInDirection))
                    shapes.Add(shapeInDirection);
            }
        }
        return num;
    }

    public bool GetExposedSlotCountInDirection(BoardPosition direction, out int numExposed, out int numFacingDirection)
    {
        numExposed = 0;
        numFacingDirection = 0;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape shapeInDirection = this.OccupiedSlots[index].GetNeighborShapeInDirection(direction);
            if (shapeInDirection == null)
            {
                numExposed = numExposed + 1;
                numFacingDirection = numFacingDirection + 1;
            }
            else if (shapeInDirection != this)
                numFacingDirection = numFacingDirection + 1;
        }
        return true;
    }

    public bool HasExposedFacesUpAndSides(Dictionary<GameShape, bool> occludermap)
    {
        int numExposed = 0;
        int numOuterFacesInDirections = 0;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape xUp;
            GameShape xDown;
            GameShape yUp;
            GameShape yDown;
            GameShape zUp;
            GameShape zDown;
            this.OccupiedSlots[index].GetNeighborShapesInAllDirections(out xUp, out xDown, out yUp, out yDown, out zUp, out zDown);
            if (xUp != null && xDown != null && (yUp != null && yDown != null) && zDown != null)
            {
                this.InternalDoLogicForShapeSlotResult(xUp, ref numExposed, ref numOuterFacesInDirections, occludermap);
                if (numExposed <= 0)
                {
                    this.InternalDoLogicForShapeSlotResult(xDown, ref numExposed, ref numOuterFacesInDirections, occludermap);
                    if (numExposed <= 0)
                    {
                        this.InternalDoLogicForShapeSlotResult(yUp, ref numExposed, ref numOuterFacesInDirections, occludermap);
                        if (numExposed <= 0)
                        {
                            this.InternalDoLogicForShapeSlotResult(yDown, ref numExposed, ref numOuterFacesInDirections, occludermap);
                            if (numExposed <= 0)
                            {
                                this.InternalDoLogicForShapeSlotResult(zDown, ref numExposed, ref numOuterFacesInDirections, occludermap);
                                if (numExposed > 0)
                                    break;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                else
                    break;
            }
            else
            {
                ++numExposed;
                break;
            }
        }
        return numExposed != 0;
    }

    public bool HasExposedFacesUpAndSides()
    {
        int numExposed = 0;
        int numOuterFacesInDirections = 0;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape xUp;
            GameShape xDown;
            GameShape yUp;
            GameShape yDown;
            GameShape zUp;
            GameShape zDown;
            this.OccupiedSlots[index].GetNeighborShapesInAllDirections(out xUp, out xDown, out yUp, out yDown, out zUp, out zDown);
            if (xUp != null && xDown != null && (yUp != null && yDown != null) && zDown != null)
            {
                this.InternalDoLogicForShapeSlotResult(xUp, ref numExposed, ref numOuterFacesInDirections);
                if (numExposed <= 0)
                {
                    this.InternalDoLogicForShapeSlotResult(xDown, ref numExposed, ref numOuterFacesInDirections);
                    if (numExposed <= 0)
                    {
                        this.InternalDoLogicForShapeSlotResult(yUp, ref numExposed, ref numOuterFacesInDirections);
                        if (numExposed <= 0)
                        {
                            this.InternalDoLogicForShapeSlotResult(yDown, ref numExposed, ref numOuterFacesInDirections);
                            if (numExposed <= 0)
                            {
                                this.InternalDoLogicForShapeSlotResult(zDown, ref numExposed, ref numOuterFacesInDirections);
                                if (numExposed > 0)
                                    break;
                            }
                            else
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                else
                    break;
            }
            else
            {
                ++numExposed;
                break;
            }
        }
        return numExposed != 0;
    }

    private void InternalDoLogicForShapeSlotResult(GameShape shape, ref int numExposed, ref int numOuterFacesInDirections, Dictionary<GameShape, bool> occludermap)
    {
        if (shape != null)
        {
            if (shape == this)
                return;
            numOuterFacesInDirections = numOuterFacesInDirections + 1;
            bool flag;
            if (occludermap.TryGetValue(shape, out flag))
            {
                if (flag)
                    return;
                numExposed = numExposed + 1;
            }
            else
            {
                GameShapeProxy gameShapeProxy = shape.GetRenderProxy() as GameShapeProxy;
                if ((UnityEngine.Object)gameShapeProxy != (UnityEngine.Object)null && gameShapeProxy.IsOccluding(false, true))
                    return;
                numExposed = numExposed + 1;
            }
        }
        else
        {
            numExposed = numExposed + 1;
            numOuterFacesInDirections = numOuterFacesInDirections + 1;
        }
    }

    private void InternalDoLogicForShapeSlotResult(GameShape shape, ref int numExposed, ref int numOuterFacesInDirections)
    {
        if (shape != null)
        {
            if (shape == this)
                return;
            numOuterFacesInDirections = numOuterFacesInDirections + 1;
            GameShapeProxy gameShapeProxy = shape.GetRenderProxy() as GameShapeProxy;
            if ((UnityEngine.Object)gameShapeProxy != (UnityEngine.Object)null && gameShapeProxy.IsOccluding(false, true))
                return;
            numExposed = numExposed + 1;
        }
        else
        {
            numExposed = numExposed + 1;
            numOuterFacesInDirections = numOuterFacesInDirections + 1;
        }
    }

    public bool IsSupportedInGravityDirection(BoardPosition gravity, List<GameShape> IgnoreList)
    {
        bool flag1 = false;
        bool flag2 = false;
        if (IgnoreList != null && IgnoreList.Count > 0)
            flag2 = true;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape shapeInDirection = this.OccupiedSlots[index].GetNeighborShapeInDirection(gravity);
            if (shapeInDirection != null && shapeInDirection != this && (!flag2 || flag2 && !IgnoreList.Contains(shapeInDirection)))
            {
                flag1 = true;
                break;
            }
        }
        return flag1;
    }

    public bool IsSupportedInGravityDirection(BoardPosition gravity)
    {
        bool flag = false;
        for (int index = 0; index < this.OccupiedSlots.Count; ++index)
        {
            GameShape shapeInDirection = this.OccupiedSlots[index].GetNeighborShapeInDirection(gravity);
            if (shapeInDirection != null && shapeInDirection != this)
            {
                flag = true;
                break;
            }
        }
        return flag;
    }

    public void TEMP_MarkForDestroy()
    {
        this.PendingDestroy = true;
    }

    public bool SetShatterComboID(int inShatterID, bool notifyProxyToHighlight)
    {
        this.InShatterCombo = true;
        this.ShatterOwnerID = inShatterID;
        this.PendingDestroy = true;
        if (notifyProxyToHighlight && (UnityEngine.Object)this.RenderProxy != (UnityEngine.Object)null)
            this.RenderProxy.HighlightForCombo();
        return true;
    }

    public int GetShatterOwnerID()
    {
        return this.ShatterOwnerID;
    }

    public bool CanGravityDrop()
    {
        return this.ShapeAttributes.IsAffectedByGravity;
    }

    public bool CanMoveByPlayerGrab()
    {
        return this.ShapeAttributes.IsMovableOnBoardByPlayerGrab;
    }

    public bool CanShatterComboAsDirectShape()
    {
        return this.ShapeAttributes.IsBreakableOnBoardByShatterCombo;
    }

    public bool IsInShatterCombo()
    {
        return this.InShatterCombo;
    }

    public bool CanJoinShatterCombo()
    {
        bool flag = false;
        if (this.CanShatterComboAsDirectShape() && !this.IsInShatterCombo() && !this.IsPendingDestroy())
            flag = true;
        return flag;
    }

    public bool BreaksOnNeighborShatterComboBreak()
    {
        return this.ShapeClassType == EShapeClassType.Breakable;
    }

    public bool IsPendingDestroy()
    {
        return this.PendingDestroy;
    }

    public bool AllowsEncroachment()
    {
        return this.ShapeClassType == EShapeClassType.Breakable;
    }

    public int GetBasePointValue()
    {
        return this.ShapeAttributes.ShapeBasePointValue;
    }

    public float GetComboScoreMultiplierFactor()
    {
        return this.ShapeAttributes.ComboScoreAdditiveMultiplier;
    }

    //public static bool LoadShape(SavedShapeInfo loadData, out GameShape newShape, out BoardPosition newPos)
    //{
    //    bool flag = false;
    //    if (loadData != null)
    //    {
    //        SavedShapeInfo savedShapeInfo = SavedShapeInfo.ProcessLoadingShapeInfo(loadData);
    //        if (savedShapeInfo != null)
    //        {
    //            GameShape shape = ShapeFactory.CreateShape(savedShapeInfo.ShapeLayoutType, savedShapeInfo.ShapeClassType);
    //            if (shape != null)
    //            {
    //                newShape = shape;
    //                newPos = savedShapeInfo.ShapeAnchorPosition.ToBoardPosition();
    //                flag = true;
    //            }
    //            else
    //            {
    //                newShape = (GameShape)null;
    //                newPos = new BoardPosition(-1, -1, -1);
    //                Debug.LogWarning((object)"Unable to create shape from save data");
    //            }
    //        }
    //        else
    //        {
    //            newShape = (GameShape)null;
    //            newPos = new BoardPosition(-1, -1, -1);
    //            Debug.LogWarning((object)"Tried to load shape with non-fixed up save data");
    //        }
    //    }
    //    else
    //    {
    //        newShape = (GameShape)null;
    //        newPos = new BoardPosition(-1, -1, -1);
    //        Debug.LogWarning((object)"Tried to load shape with null saved data");
    //    }
    //    return flag;
    //}
}