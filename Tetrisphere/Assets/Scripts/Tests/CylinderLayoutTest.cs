// Decompiled with JetBrains decompiler
// Type: CylinderLayoutTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ADDEA9C9-AEE1-4DC1-B840-76EBFDF8AEDE
// Assembly location: C:\Users\Guy\Desktop\sc\assets\bin\Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CylinderLayoutTest : BoardControllerBase
{
    private Dictionary<BoardSlot, GameObject> BoardSlotToVisualMap = new Dictionary<BoardSlot, GameObject>();
    public float BaseDistFromCenterAxis = 5f;
    public float MoveTransitionTime = 0.5f;
    private float MoveTransitionStartTime = -1f;
    private float MoveTransitionStartAngle = -1f;
    private float MoveTransitionEndAngle = -1f;
    public int BoardSizeX;
    public int BoardSizeY;
    public int BoardSizeZ;
    public GameBoard board;
    public GameObject ShapeSpawner;
    public GameObject CylinderCenterGO;
    public float ForwardAngle;
    private bool bInMoveTransition;

    protected override void Start()
    {
        base.Start();
        this.CreateCylinderCenterGO();
        this.BoardSizeX = this.BoardSizeX <= 0 ? 8 : this.BoardSizeX;
        this.BoardSizeY = this.BoardSizeY <= 0 ? 8 : this.BoardSizeY;
        this.BoardSizeZ = this.BoardSizeZ <= 0 ? 8 : this.BoardSizeZ;
        this.board = new GameBoard();
        this.board.InitializeGameBoard(this.BoardSizeX, this.BoardSizeY, this.BoardSizeZ);
        this.board.SetupGravity(new BoardPosition(0, 0, 1), true);
        this.board.WrapXAxis = true;
        this.board.WrapYAxis = true;
        if ((Object)this.BoardViewer != (Object)null)
            this.BoardViewer.NotifyBoardCreated(this.board);
        this.AddShapeToAllSlots();
    }

    private void CreateCylinderCenterGO()
    {
        this.CylinderCenterGO = new GameObject("CylinderCenter");
        this.CylinderCenterGO.transform.position = Vector3.zero;
        this.CylinderCenterGO.transform.rotation = Quaternion.identity;
    }

    private void AddShapeToAllSlots()
    {
        if (this.board == null || (Object)this.ShapeSpawner == (Object)null)
            return;
        BoardSlot[, ,] boardSlotArray = this.board.TheGameBoard;
        int length1 = boardSlotArray.GetLength(0);
        int length2 = boardSlotArray.GetLength(1);
        int length3 = boardSlotArray.GetLength(2);
        for (int index1 = 0; index1 < length1; ++index1)
        {
            for (int index2 = 0; index2 < length2; ++index2)
            {
                for (int index3 = 0; index3 < length3; ++index3)
                {
                    BoardSlot boardSlot = boardSlotArray[index1, index2, index3];
                    Vector3 zero = Vector3.zero;
                    Quaternion identity = Quaternion.identity;
                    this.GetLayoutStuff(boardSlot, ref zero, ref identity);
                    GameObject gameObject = Object.Instantiate((Object)this.ShapeSpawner, zero, identity) as GameObject;
                    this.BoardSlotToVisualMap.Add(boardSlot, gameObject);
                }
            }
        }
    }

    private void UpdateSlotVisuals()
    {
        if (this.board == null || (Object)this.ShapeSpawner == (Object)null)
            return;
        using (Dictionary<BoardSlot, GameObject>.Enumerator enumerator = this.BoardSlotToVisualMap.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                KeyValuePair<BoardSlot, GameObject> current = enumerator.Current;
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                this.GetLayoutStuff(current.Key, ref zero, ref identity);
                current.Value.transform.position = zero;
                current.Value.transform.rotation = identity;
            }
        }
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
        return 1f;
    }

    public void GetLayoutStuff(BoardSlot slot, ref Vector3 newPosition, ref Quaternion FacingDirection)
    {
        BoardPosition boardPos = slot.BoardPos;
        float num1 = 360f / (float)this.board.BoardXSize;
        BoardPosition pos = boardPos;
        this.board.CreateSafeBoardPosition(ref pos);
        Quaternion quaternion = Quaternion.Euler(0.0f, this.ForwardAngle + (float)pos.X * num1, 0.0f);
        float num2 = this.BaseDistFromCenterAxis + this.GetCylinderDepthOffset(slot);
        newPosition = quaternion * Vector3.forward * num2;
        newPosition.y = this.GetCylinderPositionY(slot);
        FacingDirection = quaternion;
    }

    private void Update()
    {
        if (this.bInMoveTransition)
        {
            float num = Time.time - this.MoveTransitionStartTime;
            if ((double)num >= (double)this.MoveTransitionTime)
            {
                this.ForwardAngle = this.MoveTransitionEndAngle;
                this.UpdateSlotVisuals();
                this.bInMoveTransition = false;
            }
            else
            {
                //this.ForwardAngle = InterpHelpers.Smerp(this.MoveTransitionStartAngle, this.MoveTransitionEndAngle, num / this.MoveTransitionTime);
                this.UpdateSlotVisuals();
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            float num = 360f / (float)this.board.BoardXSize;
            this.bInMoveTransition = true;
            this.MoveTransitionStartTime = Time.time;
            this.MoveTransitionStartAngle = this.ForwardAngle;
            this.MoveTransitionEndAngle = this.ForwardAngle + num;
        }
        else
        {
            if (!Input.GetKeyDown(KeyCode.RightArrow))
                return;
            float num = 360f / (float)this.board.BoardXSize;
            this.bInMoveTransition = true;
            this.MoveTransitionStartTime = Time.time;
            this.MoveTransitionStartAngle = this.ForwardAngle;
            this.MoveTransitionEndAngle = this.ForwardAngle - num;
        }
    }

    private float GetCylinderDepthOffset(BoardSlot slot)
    {
        return 2f * (float)(this.board.BoardZSize - slot.BoardPos.Z);
    }

    private float GetCylinderPositionY(BoardSlot slot)
    {
        float num1 = 2f;
        float num2;
        if (this.board.BoardYSize % 2 == 0)
        {
            int num3 = this.board.BoardYSize / 2;
            num2 = slot.BoardPos.Y >= num3 ? (float)((double)(slot.BoardPos.Y - num3) * (double)num1 + 0.5 * (double)num1) : (float)-((double)(num3 - 1 - slot.BoardPos.Y) * (double)num1 + 0.5 * (double)num1);
        }
        else
        {
            int num3 = this.board.BoardYSize / 2;
            num2 = slot.BoardPos.Y >= num3 ? (slot.BoardPos.Y <= num3 ? 0.0f : (float)(slot.BoardPos.Y - num3) * num1) : (float)-(double)(num3 - slot.BoardPos.Y) * num1;
        }
        return num2;
    }

    public Vector3 LayoutGetWorldCenterPositionForBoardPosition(BoardPosition pos)
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
}