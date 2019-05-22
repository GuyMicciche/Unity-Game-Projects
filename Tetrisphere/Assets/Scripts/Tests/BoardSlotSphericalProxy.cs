public class BoardSlotSphericalProxy : CoreMonoBehaviour
{
    private BoardPositionContainer boardPos = new BoardPositionContainer(-1, -1, -1);
    public int VisualSlotPositionX;
    public int VisualSlotPositionY;
    public int VisualSlotPositionZ;
    public float AzimuthPos;
    public float AzimuthBreadth;
    public float InclinationPos;
    public float InclinationBreadth;

    public BoardPosition BoardPos
    {
        get
        {
            return this.boardPos.ToBoardPosition();
        }
    }

    public void SetSlot(BoardSlot newSlot)
    {
        this.boardPos.Set(newSlot.BoardPos);
    }

    public void RemoveSlotReference()
    {
        this.boardPos.Set(-1, -1, -1);
    }

    public void SetVisible(bool IsVisible)
    {
        this.gameObject.SetActive(IsVisible);
    }

    public void SetSphericalVisualPositions(int visx, int visy, int visz)
    {
        this.VisualSlotPositionX = visx;
        this.VisualSlotPositionY = visy;
        this.VisualSlotPositionZ = visz;
    }

    public void SetSphericalPositions(float inAzimuthPos, float inAzimuthBreadth, float inInclinationPos, float inInclinationBreadth)
    {
        this.AzimuthPos = inAzimuthPos;
        this.AzimuthBreadth = inAzimuthBreadth;
        this.InclinationPos = inInclinationPos;
        this.InclinationBreadth = inInclinationBreadth;
    }
}