using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameShapeProxy : GameShapeProxyBase
{
    protected Color ComboColor = Color.white;
    protected float ComboColorMultiplier = 3f;
    protected GameShapeProxy.EApplyComboColorMultipler ComboColorMultiplierTarget = GameShapeProxy.EApplyComboColorMultipler.ComboColorRGBA;
    private Vector3 ShapeDropInterpStartPosition = Vector3.zero;
    private Vector3 ShapeDropInterpTargetPosition = Vector3.zero;
    private Renderer MyRenderer;
    public GameObject GenericDestructionParticlePrefab;
    public float ParticleLifeTime;
    public ShapeAttributesContainer ShapeAttributes;

    public bool HighlightedForCombo { get; protected set; }

    public bool ShapeDropInterpActive { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        this.MyRenderer = this.GetComponent<Renderer>();
    }

    public override bool RegisterPrimaryShape(GameShape inPrimaryShape)
    {
        this.PrimaryShape = inPrimaryShape;
        this.ShapeAttributes = this.PrimaryShape.GetShapeAttributes();
        return true;
    }

    public override bool IsProxyVisible()
    {
        if ((Object)this.MyRenderer != (Object)null)
            return this.MyRenderer.isVisible;
        else
            return false;
    }

    public bool IsProxyRendererEnabled()
    {
        if ((Object)this.MyRenderer != (Object)null)
            return this.MyRenderer.enabled;
        else
            return false;
    }

    public override void HideProxy(bool hide)
    {
        if (hide)
            this.MyRenderer.enabled = false;
        else
            this.MyRenderer.enabled = true;
    }

    public bool IsOccluding(bool checkIfRendererActive, bool checkIfInterp)
    {
        return !this.PrimaryShape.IsPendingDestroy() && ((!checkIfInterp || checkIfInterp && !this.ShapeDropInterpActive) && (!checkIfRendererActive || this.MyRenderer.enabled));
    }

    public bool VisibilityCanBeControlledByOcclusionCulling()
    {
        bool flag = true;
        if (this.ShapeDropInterpActive)
            flag = false;
        return flag;
    }

    public bool EvaluateProxyVisibility(Dictionary<GameShape, bool> occluderMap)
    {
        if (!this.VisibilityCanBeControlledByOcclusionCulling())
            return this.MyRenderer.enabled;
        if (this.PrimaryShape.HasExposedFacesUpAndSides(occluderMap))
            this.HideProxy(false);
        else
            this.HideProxy(true);
        return this.MyRenderer.enabled;
    }

    public bool EvaluateProxyVisibility()
    {
        if (!this.VisibilityCanBeControlledByOcclusionCulling())
            return this.MyRenderer.enabled;
        if (this.PrimaryShape.HasExposedFacesUpAndSides())
            this.HideProxy(false);
        else
            this.HideProxy(true);
        return this.MyRenderer.enabled;
    }

    public override void NotifyPreRemoveFromBoard()
    {
        //this.CreateDestructionParticle();
    }

    public override void UnRegisterPrimaryShape()
    {
        this.PrimaryShape = (GameShape)null;
        this.ShapeAttributes = (ShapeAttributesContainer)null;
        Object.Destroy((Object)this.MyGameObject);
    }

    //protected void CreateDestructionParticle()
    //{
    //    if ((Object)this.GenericDestructionParticlePrefab == (Object)null)
    //        this.GenericDestructionParticlePrefab = HACK_ShapeParticleFactory.Instance.GetParticlePrefabForShapeDestruction(this.PrimaryShape.GetShapeClassType(), this.PrimaryShape.GetShapeLayoutType());
    //    if ((double)this.ParticleLifeTime <= 0.0)
    //        this.ParticleLifeTime = 0.75f;
    //    if (!((Object)this.GenericDestructionParticlePrefab != (Object)null))
    //        return;GameObject gameObject = Object.Instantiate((Object)this.GenericDestructionParticlePrefab, HACK_BoardViewer.instance.ConvertToSexyTimeOffsetPosition(this.MyTransform.position), Quaternion.identity) as GameObject;
    //    if (true)
    //    {
    //        ColorHolder component1 = this.GetComponent<ColorHolder>();
    //        if ((Object)component1 != (Object)null)
    //        {
    //            Color color = component1.CachedColor;
    //            ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
    //            if ((Object)component2 != (Object)null)
    //                component2.startColor = color;
    //        }
    //    }
    //    Object.Destroy((Object)gameObject, this.ParticleLifeTime);
    //}

    public override void HighlightForCombo()
    {
        if (this.HighlightedForCombo || !((Object)this.MyRenderer != (Object)null))
            return;
        Color color1 = Color.white;
        switch (this.ComboColorMultiplierTarget)
        {
            case GameShapeProxy.EApplyComboColorMultipler.CurrentColorRGBA:
                color1 = this.MyRenderer.material.color * this.ComboColorMultiplier;
                break;
            case GameShapeProxy.EApplyComboColorMultipler.CurrentColorA:
                color1 = this.MyRenderer.material.color;
                color1.a *= this.ComboColorMultiplier;
                break;
            case GameShapeProxy.EApplyComboColorMultipler.ComboColorRGBA:
                color1 = this.ComboColor * this.ComboColorMultiplier;
                break;
            case GameShapeProxy.EApplyComboColorMultipler.CurrentColorRBAComboColorA:
                Color color2 = this.MyRenderer.material.color;
                color2.a = this.ComboColor.a;
                color1 = color2 * this.ComboColorMultiplier;
                break;
        }
        this.MyRenderer.material.color = color1;
        this.HighlightedForCombo = true;
    }

    public override void ShapeMovedToNewAnchorPosition(BoardPosition newAnchorPosition)
    {
        if (this.PrimaryShape == null)
            return;
        Vector3 positionWithWeird = HACK_BoardLayoutManager.GetLayoutManager().GetWorldCenterPositionForBoardPositionWithWeird(newAnchorPosition);
        if (this.ShapeDropInterpActive)
            this.ShapeDropInterpTargetPosition = positionWithWeird;
        else
            this.MyTransform.position = positionWithWeird;
    }

    public override void SetNewWorldPositionForBoardRepositionShift(Vector3 newWorldPosition)
    {
        if (this.ShapeDropInterpActive)
        {
            Vector3 vector3 = newWorldPosition - this.ShapeDropInterpTargetPosition;
            vector3.z = 0.0f;
            this.ShapeDropInterpTargetPosition = newWorldPosition;
            this.ShapeDropInterpStartPosition += vector3;
        }
        else
            this.MyTransform.position = newWorldPosition;
    }

    public override void SetupInterpForShapeDrop(Vector3 shapeStart, Vector3 shapeEnd, float moveTime)
    {
        this.ShapeDropInterpStartPosition = shapeStart;
        this.ShapeDropInterpTargetPosition = shapeEnd;
        this.ShapeDropInterpActive = true;
        //this.StartCoroutine(this.InterpForShapeDrop(moveTime));
    }

    //[DebuggerHidden]
    //private IEnumerator InterpForShapeDrop(float moveTime)
    //{
    //  // ISSUE: object of a compiler-generated type is created
    //  return (IEnumerator) new GameShapeProxy.\u003CInterpForShapeDrop\u003Ec__Iterator11()
    //  {
    //    moveTime = moveTime,
    //    \u003C\u0024\u003EmoveTime = moveTime,
    //    \u003C\u003Ef__this = this
    //  };
    //}

    public override Vector3 GetPositionOrTargetPosition()
    {
        if (this.ShapeDropInterpActive)
            return this.ShapeDropInterpTargetPosition;
        else
            return this.MyTransform.position;
    }

    public override void SetupInterpForShapeWithHidePause(Vector3 shapeStart, Vector3 shapeEnd, float delayBeforeMove, bool hideDuringDelay, float moveTime)
    {
        this.ShapeDropInterpStartPosition = shapeStart;
        this.ShapeDropInterpTargetPosition = shapeEnd;
        if (hideDuringDelay)
            this.HideProxy(true);
        this.ShapeDropInterpActive = true;
        //this.StartCoroutine(this.InterpForShapeDrop(delayBeforeMove, moveTime));
    }

    //[DebuggerHidden]
    //private IEnumerator InterpForShapeDrop(float delayBeforeMove, float moveTime)
    //{
    //  // ISSUE: object of a compiler-generated type is created
    //  return (IEnumerator) new GameShapeProxy.\u003CInterpForShapeDrop\u003Ec__Iterator12()
    //  {
    //    delayBeforeMove = delayBeforeMove,
    //    moveTime = moveTime,
    //    \u003C\u0024\u003EdelayBeforeMove = delayBeforeMove,
    //    \u003C\u0024\u003EmoveTime = moveTime,
    //    \u003C\u003Ef__this = this
    //  };
    //}

    public enum EApplyComboColorMultipler
    {
        CurrentColorRGBA,
        CurrentColorA,
        ComboColorRGBA,
        CurrentColorRBAComboColorA,
    }
}

public class ColorHolder : CoreMonoBehaviour
{
    public Color CachedColor = Color.white;
}

public static class HACK_BoardViewer
{
    public static BoardViewerBase instance;

    public static BoardViewerBase GetViewer()
    {
        if ((Object)HACK_BoardViewer.instance == (Object)null)
        {
            //Debug.LogError((object)"HACK_BoardViewer.instance isn't set");
        }
        return HACK_BoardViewer.instance;
    }
}

public static class HACK_ShapeParticleFactory
{
    //public static ParticleFactory Instance;
}
