using System;

[Serializable]
public class ShapeAttributesContainer
{
    public bool IsMovableOnBoardByPlayerGrab;
    public bool IsBreakableOnBoardByShatterCombo;
    public bool IsAffectedByGravity;
    public int ShapeBasePointValue;
    public float ComboScoreAdditiveMultiplier;

    public ShapeAttributesContainer()
    {
    }

    public ShapeAttributesContainer(ShapeAttributesContainer CopyTemplate)
    {
        this.IsMovableOnBoardByPlayerGrab = CopyTemplate.IsMovableOnBoardByPlayerGrab;
        this.IsBreakableOnBoardByShatterCombo = CopyTemplate.IsBreakableOnBoardByShatterCombo;
        this.IsAffectedByGravity = CopyTemplate.IsAffectedByGravity;
        this.ShapeBasePointValue = CopyTemplate.ShapeBasePointValue;
        this.ComboScoreAdditiveMultiplier = CopyTemplate.ComboScoreAdditiveMultiplier;
    }

    public void HACK_SetDefaultProperties()
    {
        this.IsBreakableOnBoardByShatterCombo = true;
        this.IsAffectedByGravity = true;
    }

    public void HACK_SetDefaultProperties_NormalShape()
    {
        this.HACK_SetDefaultProperties();
        this.IsMovableOnBoardByPlayerGrab = true;
        this.ShapeBasePointValue = 10;
    }

    public void HACK_SetDefaultProperties_BreakableShape()
    {
        this.HACK_SetDefaultProperties();
        this.IsBreakableOnBoardByShatterCombo = false;
        this.IsMovableOnBoardByPlayerGrab = false;
        this.ShapeBasePointValue = 5;
    }

    public void HACK_SetDefaultProperties_ImmovableShape()
    {
        this.HACK_SetDefaultProperties();
        this.IsMovableOnBoardByPlayerGrab = false;
        this.ShapeBasePointValue = 15;
    }

    public void HACK_SetDefaultProperties_Indestructible()
    {
        this.IsBreakableOnBoardByShatterCombo = false;
        this.IsAffectedByGravity = true;
        this.IsMovableOnBoardByPlayerGrab = true;
        this.ShapeBasePointValue = 0;
    }
}