using UnityEngine;

public abstract class GameShapeProxyBase : CoreMonoBehaviour
{
    protected GameShape PrimaryShape;

    public virtual GameShape GetPrimaryShape()
    {
        return this.PrimaryShape;
    }

    public virtual bool IsProxyVisible()
    {
        return false;
    }

    public virtual void HideProxy(bool hidden)
    {
        Debug.LogWarning((object)"Virtual HideProxy called");
    }

    public abstract bool RegisterPrimaryShape(GameShape inPrimaryShape);

    public abstract void UnRegisterPrimaryShape();

    public virtual void NotifyPreRemoveFromBoard()
    {
    }

    public virtual void ShapeMovedToNewAnchorPosition(BoardPosition newAnchorPosition)
    {
        if (this.PrimaryShape == null)
            return;
        this.MyTransform.position = HACK_BoardLayoutManager.GetLayoutManager().GetWorldCenterPositionForBoardPositionWithWeird(newAnchorPosition);
    }

    public virtual void HighlightForCombo()
    {
    }

    public virtual void SetNewWorldPositionForBoardRepositionShift(Vector3 newWorldPosition)
    {
        this.MyTransform.position = newWorldPosition;
    }

    public virtual void SetupInterpForShapeDrop(Vector3 shapeStart, Vector3 shapeEnd, float moveTime)
    {
        Debug.LogWarning((object)"Virtual shape interp called");
    }

    public virtual void SetupInterpForShapeWithHidePause(Vector3 shapeStart, Vector3 shapeEnd, float delayBeforeMove, bool hideDuringDelay, float moveTime)
    {
        Debug.LogWarning((object)"Virtual shape interp hide called");
    }

    public virtual Vector3 GetPositionOrTargetPosition()
    {
        return this.MyTransform.position;
    }
}
