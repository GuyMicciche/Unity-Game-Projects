using UnityEngine;

public class BoardLayoutManager : CoreMonoBehaviour
{
    public MinMaxRange WorldXBoardRange = new MinMaxRange();
    public MinMaxRange WorldYBoardRange = new MinMaxRange();
    public MinMaxRange WorldZBoardRange = new MinMaxRange();
    public MinMaxRange WorldXCameraRange = new MinMaxRange();
    public MinMaxRange WorldYCameraRange = new MinMaxRange();
    protected float BoardXSpanInWorldX;
    protected float BoardYSpanInWorldY;
    protected float BoardZSpanInWorldZ;
    private float[] BoardXWorldXArray;
    private float[] BoardYWorldYArray;
    private float[] BoardZWorldZArray;
    private bool NotifyBoardCreatedSetup;
    private GameBoard ActiveBoard;

    protected override void Awake()
    {
        base.Awake();
        HACK_BoardLayoutManager.instance = this;
    }

    private void OnDestroy()
    {
        if (!((Object)HACK_BoardLayoutManager.instance == (Object)this))
            return;
        HACK_BoardLayoutManager.instance = (BoardLayoutManager)null;
    }

    public virtual void NotifyBoardCreated(GameBoard newBoard, BoardPosition cameraStartBoardPos)
    {
        this.ActiveBoard = newBoard;
        this.BoardXSpanInWorldX = (float)newBoard.BoardXSize * this.GetBoardSlotWorldSizeX();
        this.BoardYSpanInWorldY = (float)newBoard.BoardYSize * this.GetBoardSlotWorldSizeY();
        this.BoardZSpanInWorldZ = (float)newBoard.BoardZSize * this.GetBoardSlotWorldSizeZ();
        this.BoardXWorldXArray = new float[newBoard.BoardXSize];
        this.BoardYWorldYArray = new float[newBoard.BoardYSize];
        this.BoardZWorldZArray = new float[newBoard.BoardZSize];
        for (int boardPosX = 0; boardPosX < this.BoardXWorldXArray.Length; ++boardPosX)
            this.BoardXWorldXArray[boardPosX] = this.GetWorldCenterPositionForBoardPositionX(boardPosX);
        for (int boardPosY = 0; boardPosY < this.BoardYWorldYArray.Length; ++boardPosY)
            this.BoardYWorldYArray[boardPosY] = this.GetWorldCenterPositionForBoardPositionY(boardPosY);
        for (int boardPosZ = 0; boardPosZ < this.BoardZWorldZArray.Length; ++boardPosZ)
            this.BoardZWorldZArray[boardPosZ] = this.GetWorldCenterPositionForBoardPositionZ(boardPosZ);
        this.DoWorldCameraLocationSetup(newBoard, cameraStartBoardPos);
        this.NotifyBoardCreatedSetup = true;
    }

    public virtual void NotifyBoardRemoved(GameBoard removedBoard)
    {
        this.ActiveBoard = (GameBoard)null;
    }

    public float GetBoardSlotWorldSizeX()
    {
        return 1f;
    }

    public float GetBoardSlotWorldSizeY()
    {
        return 1f;
    }

    public float GetBoardSlotWorldSizeZ()
    {
        return 2f;
    }

    public bool GetBoardPositionForWorldPositionXYWithWeird(Vector3 cameraWorldPos, out BoardPosition slotOverPos)
    {
        if (!this.NotifyBoardCreatedSetup)
        {
            slotOverPos = new BoardPosition();
            return false;
        }
        else
        {
            bool flag;
            if (this.WorldXBoardRange.IsInRangeInclusive(cameraWorldPos.x) && this.WorldYBoardRange.IsInRangeInclusive(cameraWorldPos.y))
            {
                slotOverPos = new BoardPosition();
                float num1 = this.GetBoardSlotWorldSizeX() * 0.5f;
                for (int index = 0; index < this.BoardXWorldXArray.Length; ++index)
                {
                    if ((double)Mathf.Abs(cameraWorldPos.x - this.BoardXWorldXArray[index]) <= (double)num1)
                    {
                        slotOverPos.X = index;
                        break;
                    }
                }
                float num2 = this.GetBoardSlotWorldSizeY() * 0.5f;
                for (int index = 0; index < this.BoardYWorldYArray.Length; ++index)
                {
                    if ((double)Mathf.Abs(cameraWorldPos.y - this.BoardYWorldYArray[index]) <= (double)num2)
                    {
                        slotOverPos.Y = index;
                        break;
                    }
                }
                flag = true;
            }
            else
            {
                slotOverPos = new BoardPosition();
                flag = false;
            }
            return flag;
        }
    }

    public Vector3 GetWorldCenterPositionForBoardPositionWithWeird(BoardPosition pos)
    {
        if (!this.NotifyBoardCreatedSetup)
            return this.GetWorldCenterPositionForBoardPosition(pos);
        Vector3 zero = Vector3.zero;
        zero.x = this.BoardXWorldXArray[pos.X];
        zero.y = this.BoardYWorldYArray[pos.Y];
        zero.z = this.BoardZWorldZArray[pos.Z];
        return zero;
    }

    public float GetWorldCenterPositionForBoardPositionXWithWeird(int boardPosX)
    {
        if (!this.NotifyBoardCreatedSetup)
            return this.GetWorldCenterPositionForBoardPositionX(boardPosX);
        else
            return this.BoardXWorldXArray[boardPosX];
    }

    public float GetWorldCenterPositionForBoardPositionYWithWeird(int boardPosY)
    {
        if (!this.NotifyBoardCreatedSetup)
            return this.GetWorldCenterPositionForBoardPositionY(boardPosY);
        else
            return this.BoardYWorldYArray[boardPosY];
    }

    public float GetWorldCenterPositionForBoardPositionZWithWeird(int boardPosZ)
    {
        if (!this.NotifyBoardCreatedSetup)
            return this.GetWorldCenterPositionForBoardPositionZ(boardPosZ);
        else
            return this.BoardZWorldZArray[boardPosZ];
    }

    public Vector3 GetWorldCenterPositionForBoardPosition(BoardPosition pos)
    {
        Vector3 vector3 = new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
        vector3.x *= this.GetBoardSlotWorldSizeX();
        vector3.y *= this.GetBoardSlotWorldSizeY();
        vector3.z *= this.GetBoardSlotWorldSizeZ();
        vector3.x += this.GetBoardSlotWorldSizeX() * 0.5f;
        vector3.y += this.GetBoardSlotWorldSizeY() * 0.5f;
        vector3.z += this.GetBoardSlotWorldSizeZ() * 0.5f;
        return vector3;
    }

    public float GetWorldCenterPositionForBoardPositionX(int boardPosX)
    {
        return (float)boardPosX * this.GetBoardSlotWorldSizeX() + this.GetBoardSlotWorldSizeX() * 0.5f;
    }

    public float GetWorldCenterPositionForBoardPositionY(int boardPosY)
    {
        return (float)boardPosY * this.GetBoardSlotWorldSizeY() + this.GetBoardSlotWorldSizeY() * 0.5f;
    }

    public float GetWorldCenterPositionForBoardPositionZ(int boardPosZ)
    {
        return (float)boardPosZ * this.GetBoardSlotWorldSizeZ() + this.GetBoardSlotWorldSizeZ() * 0.5f;
    }

    public void NotifyCameraMovedToFixupLocation(BoardPosition NOTUSED_newCameraBoardPosUnclamped, Vector3 newCameraWorldPos)
    {
        this.DoStuffForCameraMove(NOTUSED_newCameraBoardPosUnclamped, newCameraWorldPos);
    }

    private void DoWorldCameraLocationSetup(GameBoard newBoard, BoardPosition cameraStartBoardPos)
    {
        int num1 = 11;
        int num2 = newBoard.BoardXSize - 2 * num1;
        int num3 = newBoard.BoardYSize - 2 * num1;
        int num4 = num2 / 2;
        int num5 = num3 / 2;
        this.WorldXCameraRange.Min = this.GetWorldCenterPositionForBoardPositionX(cameraStartBoardPos.X - num4);
        this.WorldXCameraRange.Max = this.GetWorldCenterPositionForBoardPositionX(cameraStartBoardPos.X + num4);
        this.WorldYCameraRange.Min = this.GetWorldCenterPositionForBoardPositionY(cameraStartBoardPos.Y - num5);
        this.WorldYCameraRange.Max = this.GetWorldCenterPositionForBoardPositionY(cameraStartBoardPos.Y + num5);
        int boardPosX = newBoard.BoardXSize - 1;
        int boardPosY = newBoard.BoardYSize - 1;
        int boardPosZ = newBoard.BoardZSize - 1;
        this.WorldXBoardRange.Min = this.GetWorldCenterPositionForBoardPositionX(0);
        this.WorldXBoardRange.Max = this.GetWorldCenterPositionForBoardPositionX(boardPosX);
        this.WorldYBoardRange.Min = this.GetWorldCenterPositionForBoardPositionY(0);
        this.WorldYBoardRange.Max = this.GetWorldCenterPositionForBoardPositionY(boardPosY);
        this.WorldZBoardRange.Min = this.GetWorldCenterPositionForBoardPositionZ(0);
        this.WorldZBoardRange.Max = this.GetWorldCenterPositionForBoardPositionZ(boardPosZ);
    }

    public void DoStuffForCameraMoveBigRandomOffsetXY(BoardPosition newCameraBoardPos)
    {
        int num1 = 11;
        int num2 = this.ActiveBoard.BoardXSize - 2 * num1;
        int num3 = this.ActiveBoard.BoardYSize - 2 * num1;
        int persideslotrange1 = num2 / 2;
        int persideslotrange2 = num3 / 2;
        BoardPosition outPosition;
        if (!this.ActiveBoard.CreateSafeBoardPosition(newCameraBoardPos, out outPosition))
            return;
        Vector3 positionWithWeird1 = this.GetWorldCenterPositionForBoardPositionWithWeird(outPosition);
        int num4 = (this.ActiveBoard.BoardXSize - 1) / 2;
        int num5 = this.ActiveBoard.BoardXSize - 1 - num4;
        for (int index = 1; index <= num4; ++index)
        {
            int num6 = outPosition.X - index;
            BoardPosition pos = outPosition;
            pos.X = num6;
            this.ActiveBoard.CreateSafeBoardPosition(ref pos);
            this.BoardXWorldXArray[pos.X] = positionWithWeird1.x - (float)index * this.GetBoardSlotWorldSizeX();
        }
        for (int index = 1; index <= num5; ++index)
        {
            int num6 = outPosition.X + index;
            BoardPosition pos = outPosition;
            pos.X = num6;
            this.ActiveBoard.CreateSafeBoardPosition(ref pos);
            this.BoardXWorldXArray[pos.X] = positionWithWeird1.x + (float)index * this.GetBoardSlotWorldSizeX();
        }
        this.WorldXBoardRange.Min = positionWithWeird1.x - (float)num4 * this.GetBoardSlotWorldSizeX();
        this.WorldXBoardRange.Max = positionWithWeird1.x + (float)num5 * this.GetBoardSlotWorldSizeX();
        this.UpdateWorldXCameraRange(outPosition, persideslotrange1);
        int num7 = (this.ActiveBoard.BoardYSize - 1) / 2;
        int num8 = this.ActiveBoard.BoardYSize - 1 - num7;
        for (int index = 1; index <= num7; ++index)
        {
            int num6 = outPosition.Y - index;
            BoardPosition pos = outPosition;
            pos.Y = num6;
            this.ActiveBoard.CreateSafeBoardPosition(ref pos);
            this.BoardYWorldYArray[pos.Y] = positionWithWeird1.y - (float)index * this.GetBoardSlotWorldSizeY();
        }
        for (int index = 1; index <= num8; ++index)
        {
            int num6 = outPosition.Y + index;
            BoardPosition pos = outPosition;
            pos.Y = num6;
            this.ActiveBoard.CreateSafeBoardPosition(ref pos);
            this.BoardYWorldYArray[pos.Y] = positionWithWeird1.y + (float)index * this.GetBoardSlotWorldSizeY();
        }
        this.WorldYBoardRange.Min = positionWithWeird1.y - (float)num7 * this.GetBoardSlotWorldSizeY();
        this.WorldYBoardRange.Max = positionWithWeird1.y + (float)num8 * this.GetBoardSlotWorldSizeY();
        this.UpdateWorldYCameraRange(outPosition, persideslotrange2);
        int shapesOnBoardCount = this.ActiveBoard.GetShapesOnBoardCount();
        for (int index = 0; index < shapesOnBoardCount; ++index)
        {
            GameShape gameShape = this.ActiveBoard.ShapeList[index];
            Vector3 positionWithWeird2 = this.GetWorldCenterPositionForBoardPositionWithWeird(gameShape.GetAnchorBoardPosition());
            gameShape.GetRenderProxy().SetNewWorldPositionForBoardRepositionShift(positionWithWeird2);
        }
    }

    private void UpdateWorldXCameraRange(BoardPosition newCenterBoardPos, int persideslotrange)
    {
        BoardPosition pos1 = newCenterBoardPos + new BoardPosition(-persideslotrange, 0, 0);
        BoardPosition pos2 = newCenterBoardPos + new BoardPosition(persideslotrange, 0, 0);
        this.ActiveBoard.CreateSafeBoardPosition(ref pos1);
        this.ActiveBoard.CreateSafeBoardPosition(ref pos2);
        this.WorldXCameraRange.Min = this.GetWorldCenterPositionForBoardPositionXWithWeird(pos1.X);
        this.WorldXCameraRange.Max = this.GetWorldCenterPositionForBoardPositionXWithWeird(pos2.X);
    }

    private void UpdateWorldYCameraRange(BoardPosition newCenterBoardPos, int persideslotrange)
    {
        BoardPosition pos1 = newCenterBoardPos + new BoardPosition(0, -persideslotrange, 0);
        BoardPosition pos2 = newCenterBoardPos + new BoardPosition(0, persideslotrange, 0);
        this.ActiveBoard.CreateSafeBoardPosition(ref pos1);
        this.ActiveBoard.CreateSafeBoardPosition(ref pos2);
        this.WorldYCameraRange.Min = this.GetWorldCenterPositionForBoardPositionYWithWeird(pos1.Y);
        this.WorldYCameraRange.Max = this.GetWorldCenterPositionForBoardPositionYWithWeird(pos2.Y);
    }

    private void DoStuffForCameraMove(BoardPosition NOTUSED_newCameraPos, Vector3 newCameraWorldPos)
    {
        int num1 = 11;
        int num2 = this.ActiveBoard.BoardXSize - 2 * num1;
        int num3 = this.ActiveBoard.BoardYSize - 2 * num1;
        int num4 = num2 / 2;
        int num5 = num3 / 2;
        if ((double)newCameraWorldPos.x < (double)this.WorldXCameraRange.Min)
        {
            float num6 = (float)num4 * this.GetBoardSlotWorldSizeX();
            this.WorldXCameraRange.ShiftRange(-num6);
            this.WorldXBoardRange.ShiftRange(-num6);
            for (int index = 0; index < this.BoardXWorldXArray.Length; ++index)
            {
                if ((double)this.BoardXWorldXArray[index] > (double)this.WorldXBoardRange.Max)
                    this.BoardXWorldXArray[index] -= this.BoardXSpanInWorldX;
            }
            int shapesOnBoardCount = this.ActiveBoard.GetShapesOnBoardCount();
            for (int index = 0; index < shapesOnBoardCount; ++index)
            {
                GameShapeProxyBase renderProxy = this.ActiveBoard.ShapeList[index].GetRenderProxy();
                Vector3 orTargetPosition = renderProxy.GetPositionOrTargetPosition();
                if ((double)orTargetPosition.x > (double)this.WorldXBoardRange.Max)
                {
                    Vector3 newWorldPosition = orTargetPosition;
                    newWorldPosition.x -= this.BoardXSpanInWorldX;
                    renderProxy.SetNewWorldPositionForBoardRepositionShift(newWorldPosition);
                }
            }
        }
        else if ((double)newCameraWorldPos.x > (double)this.WorldXCameraRange.Max)
        {
            float shiftAmount = (float)num4 * this.GetBoardSlotWorldSizeX();
            this.WorldXCameraRange.ShiftRange(shiftAmount);
            this.WorldXBoardRange.ShiftRange(shiftAmount);
            for (int index = 0; index < this.BoardXWorldXArray.Length; ++index)
            {
                if ((double)this.BoardXWorldXArray[index] < (double)this.WorldXBoardRange.Min)
                    this.BoardXWorldXArray[index] += this.BoardXSpanInWorldX;
            }
            int shapesOnBoardCount = this.ActiveBoard.GetShapesOnBoardCount();
            for (int index = 0; index < shapesOnBoardCount; ++index)
            {
                GameShapeProxyBase renderProxy = this.ActiveBoard.ShapeList[index].GetRenderProxy();
                Vector3 orTargetPosition = renderProxy.GetPositionOrTargetPosition();
                if ((double)orTargetPosition.x < (double)this.WorldXBoardRange.Min)
                {
                    Vector3 newWorldPosition = orTargetPosition;
                    newWorldPosition.x += this.BoardXSpanInWorldX;
                    renderProxy.SetNewWorldPositionForBoardRepositionShift(newWorldPosition);
                }
            }
        }
        if ((double)newCameraWorldPos.y < (double)this.WorldYCameraRange.Min)
        {
            float num6 = (float)num5 * this.GetBoardSlotWorldSizeY();
            this.WorldYCameraRange.ShiftRange(-num6);
            this.WorldYBoardRange.ShiftRange(-num6);
            for (int index = 0; index < this.BoardYWorldYArray.Length; ++index)
            {
                if ((double)this.BoardYWorldYArray[index] > (double)this.WorldYBoardRange.Max)
                    this.BoardYWorldYArray[index] -= this.BoardYSpanInWorldY;
            }
            int shapesOnBoardCount = this.ActiveBoard.GetShapesOnBoardCount();
            for (int index = 0; index < shapesOnBoardCount; ++index)
            {
                GameShapeProxyBase renderProxy = this.ActiveBoard.ShapeList[index].GetRenderProxy();
                Vector3 orTargetPosition = renderProxy.GetPositionOrTargetPosition();
                if ((double)orTargetPosition.y > (double)this.WorldYBoardRange.Max)
                {
                    Vector3 newWorldPosition = orTargetPosition;
                    newWorldPosition.y -= this.BoardYSpanInWorldY;
                    renderProxy.SetNewWorldPositionForBoardRepositionShift(newWorldPosition);
                }
            }
        }
        else
        {
            if ((double)newCameraWorldPos.y <= (double)this.WorldYCameraRange.Max)
                return;
            float shiftAmount = (float)num5 * this.GetBoardSlotWorldSizeY();
            this.WorldYCameraRange.ShiftRange(shiftAmount);
            this.WorldYBoardRange.ShiftRange(shiftAmount);
            for (int index = 0; index < this.BoardYWorldYArray.Length; ++index)
            {
                if ((double)this.BoardYWorldYArray[index] < (double)this.WorldYBoardRange.Min)
                    this.BoardYWorldYArray[index] += this.BoardYSpanInWorldY;
            }
            int shapesOnBoardCount = this.ActiveBoard.GetShapesOnBoardCount();
            for (int index = 0; index < shapesOnBoardCount; ++index)
            {
                GameShapeProxyBase renderProxy = this.ActiveBoard.ShapeList[index].GetRenderProxy();
                Vector3 orTargetPosition = renderProxy.GetPositionOrTargetPosition();
                if ((double)orTargetPosition.y < (double)this.WorldYBoardRange.Min)
                {
                    Vector3 newWorldPosition = orTargetPosition;
                    newWorldPosition.y += this.BoardYSpanInWorldY;
                    renderProxy.SetNewWorldPositionForBoardRepositionShift(newWorldPosition);
                }
            }
        }
    }
}