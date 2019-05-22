// Decompiled with JetBrains decompiler
// Type: SphericalLayoutTest
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ADDEA9C9-AEE1-4DC1-B840-76EBFDF8AEDE
// Assembly location: C:\Users\Guy\Desktop\sc\assets\bin\Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SphericalLayoutTest : CoreMonoBehaviour
{
    public int SlotsHeight = 14;
    public int SlotsWidth = 14;
    public float HeightRadiusDeg = 125f;
    public float WidthRadiusDeg = 170f;
    public bool CenterWidth = true;
    private Dictionary<BoardSlot, GameObject> BoardSlotToVisualMap = new Dictionary<BoardSlot, GameObject>();
    private Dictionary<BoardSlot, BoardSlotSphericalProxy> BoardSlotToProxyMap = new Dictionary<BoardSlot, BoardSlotSphericalProxy>();
    private Dictionary<int, SphericalLayoutTest.VerticesInfo> vertlists = new Dictionary<int, SphericalLayoutTest.VerticesInfo>();
    public float LayerDepth = 1.6f;
    public float InitialRadius = 5f;
    public bool NoDrawGizmos;
    public int BoardSizeX;
    public int BoardSizeY;
    public int BoardSizeZ;
    public GameObject ShapeSpawner;
    private GameBoard board;
    private Mesh BaseMesh;
    private Vector3[] BaseMeshVerts;
    public int miscounter;

    private void OnDrawGizmos()
    {
        if (NoDrawGizmos)
        {
            return;
        }
        float radius = InitialRadius;
        Gizmos.DrawSphere(Vector3.zero, 0.25f);
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        float num1 = WidthRadiusDeg / (float)SlotsWidth;
        float num2 = HeightRadiusDeg / (float)SlotsWidth;
        float num3 = num1 * 0.5f;
        float num4 = num2 * 0.5f;
        float num5 = WidthRadiusDeg;
        if (CenterWidth)
        {
            num5 = (float)(180.0 - (180.0 - (double)WidthRadiusDeg) * 0.5);
        }
        for (int index1 = 0; index1 < SlotsHeight; ++index1)
        {
            float elevation = (HeightRadiusDeg / 2f - num4 - (float)index1 * num2) * (float)(Math.PI / 180.0);
            for (int index2 = 0; index2 < SlotsWidth; ++index2)
            {
                float polar = (-num5 + num3 + (float)index2 * num1) * (float)(Math.PI / 180.0);
                Vector3 outCart;
                SphericalCoordinates.SphericalToCartesian(radius, polar, elevation, out outCart);
                Vector3 vector3 = outCart - Vector3.zero;
                vector3.Normalize();
                Gizmos.color = Color.red;
                Gizmos.DrawRay(outCart, vector3 * 2f);
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(outCart, Vector3.one);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        InitEww();
    }

    private void Update()
    {
        DoCubeLayout();
    }

    private void InitEww()
    {
        BoardSizeX = BoardSizeX <= 0 ? 8 : BoardSizeX;
        BoardSizeY = BoardSizeY <= 0 ? 8 : BoardSizeY;
        BoardSizeZ = BoardSizeZ <= 0 ? 8 : BoardSizeZ;
        board = new GameBoard();
        board.InitializeGameBoard(BoardSizeX, BoardSizeY, BoardSizeZ);
        board.SetupGravity(new BoardPosition(0, 0, 1), true);
        board.WrapXAxis = true;
        board.WrapYAxis = true;
        AddShapeToAllSlots();
    }

    private void DoCubeLayout()
    {
        UpdateSlotVisuals();
    }

    private void GetSlotSphericalInfo(int VisSlotX, int VisSlotY, int VisSlotDepth, out float AzimuthToCenterDeg, out float SlotAzimuthBreadthDeg, out float ElevationToCenterDeg, out float SlotElevationBreadthDeg, out float SlotCenterRadius)
    {
        float num1 = WidthRadiusDeg / (float)SlotsWidth;
        float num2 = HeightRadiusDeg / (float)SlotsWidth;
        float num3 = num1 * 0.5f;
        float num4 = num2 * 0.5f;
        float num5 = WidthRadiusDeg;
        float num6 = HeightRadiusDeg / 2f;
        if (CenterWidth)
        {
            num5 = (float)(180.0 - (180.0 - (double)WidthRadiusDeg) * 0.5);
        }
        float num7 = num6 - num4 - (float)VisSlotY * num2;
        float num8 = -num5 + num3 + (float)VisSlotX * num1;
        float num9 = (float)((double)InitialRadius + (double)LayerDepth * 0.5 + (double)LayerDepth * (double)VisSlotDepth);
        AzimuthToCenterDeg = num8;
        SlotAzimuthBreadthDeg = num1;
        ElevationToCenterDeg = num7;
        SlotElevationBreadthDeg = num2;
        SlotCenterRadius = num9;
    }

    private void GetPositionAndNormalForSlotXAndY(int VisSlotX, int VisSlotY, int VisSlotZ, out Vector3 pos, out Vector3 norm)
    {
        float AzimuthToCenterDeg;
        float SlotAzimuthBreadthDeg;
        float ElevationToCenterDeg;
        float SlotElevationBreadthDeg;
        float SlotCenterRadius;
        GetSlotSphericalInfo(VisSlotX, VisSlotY, VisSlotZ, out AzimuthToCenterDeg, out SlotAzimuthBreadthDeg, out ElevationToCenterDeg, out SlotElevationBreadthDeg, out SlotCenterRadius);
        float polar = AzimuthToCenterDeg * (float)(Math.PI / 180.0);
        float elevation = ElevationToCenterDeg * (float)(Math.PI / 180.0);
        SphericalCoordinates.SphericalToCartesian(SlotCenterRadius, polar, elevation, out pos);
        Vector3 zero = Vector3.zero;
        norm = (pos - zero).normalized;
    }

    private void AddShapeToAllSlots()
    {
        if (board == null || (UnityEngine.Object)ShapeSpawner == (UnityEngine.Object)null)
        {
            return;
        }
        BoardSlot[, ,] boardSlotArray = board.TheGameBoard;
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
                    GetLayoutStuff(boardSlot, ref zero, ref identity);

                    // Add object
                    GameObject gameObject = UnityEngine.Object.Instantiate((UnityEngine.Object)ShapeSpawner, zero, identity) as GameObject;
                    BoardSlotToVisualMap.Add(boardSlot, gameObject);
                    BoardSlotSphericalProxy slotproxy = gameObject.GetComponent<BoardSlotSphericalProxy>();
                    if ((UnityEngine.Object)slotproxy == (UnityEngine.Object)null)
                    {
                        slotproxy = gameObject.AddComponent<BoardSlotSphericalProxy>();
                        slotproxy.SetSlot(boardSlot);
                    }
                    BoardSlotToProxyMap.Add(boardSlot, slotproxy);
                    CacheSphericalProxyInfo(boardSlot, slotproxy);
                }
            }
        }
    }

    private void UpdateSlotVisuals()
    {
        if (board == null || (UnityEngine.Object)ShapeSpawner == (UnityEngine.Object)null)
        {
            return;
        }
        using (Dictionary<BoardSlot, GameObject>.Enumerator enumerator = BoardSlotToVisualMap.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                KeyValuePair<BoardSlot, GameObject> current = enumerator.Current;
                Vector3 zero = Vector3.zero;
                Quaternion identity = Quaternion.identity;
                BoardSlot key = current.Key;
                GameObject MeshGO = current.Value;
                GetLayoutStuff(key, ref zero, ref identity);
                MeshGO.transform.position = zero;
                MeshGO.transform.rotation = identity;
                DeformMeshToCoverBoardSlot(key, MeshGO);
            }
        }
    }

    private void DeformMeshToCoverBoardSlot(BoardSlot slot, GameObject MeshGO)
    {
        if ((UnityEngine.Object)BaseMesh == (UnityEngine.Object)null)
        {
            BaseMesh = ShapeSpawner.GetComponent<MeshFilter>().mesh;
            BaseMeshVerts = BaseMesh.vertices;
        }
        int VisualX;
        int VisualY;
        int VisualZ;
        GetVisualPositionsForBoardSlot(slot, out VisualX, out VisualY, out VisualZ);
        int keyForY1 = GetKeyForY(VisualY, VisualZ);
        if (vertlists.ContainsKey(keyForY1))
        {
            Mesh mesh = MeshGO.GetComponent<MeshFilter>().mesh;
            SphericalLayoutTest.VerticesInfo verticesInfo = vertlists[keyForY1];
            mesh.vertices = verticesInfo.verts;
            mesh.normals = verticesInfo.normals;
        }
        else
        {
            float AzimuthToCenterDeg;
            float SlotAzimuthBreadthDeg;
            float ElevationToCenterDeg;
            float SlotElevationBreadthDeg;
            float SlotCenterRadius;
            GetSlotSphericalInfo(VisualX, VisualY, VisualZ, out AzimuthToCenterDeg, out SlotAzimuthBreadthDeg, out ElevationToCenterDeg, out SlotElevationBreadthDeg, out SlotCenterRadius);
            Mesh mesh = MeshGO.GetComponent<MeshFilter>().mesh;
            Vector3[] vector3Array = new Vector3[mesh.vertexCount];
            Transform transform = MeshGO.transform;
            Vector3 localScale = transform.localScale;
            for (int index = 0; index < vector3Array.Length; ++index)
            {
                Vector3 vector3 = BaseMeshVerts[index];
                float polar = (AzimuthToCenterDeg + vector3.x * SlotAzimuthBreadthDeg) * (float)(Math.PI / 180.0);
                float elevation = (ElevationToCenterDeg + vector3.y * SlotElevationBreadthDeg) * (float)(Math.PI / 180.0);
                Vector3 outCart;
                SphericalCoordinates.SphericalToCartesian(SlotCenterRadius - localScale.z * vector3.z, polar, elevation, out outCart);
                outCart = transform.InverseTransformPoint(outCart);
                vector3Array[index] = outCart;
            }
            mesh.vertices = vector3Array;
            mesh.RecalculateNormals();
            int keyForY2 = GetKeyForY(VisualY, VisualZ);
            if (vertlists.ContainsKey(keyForY2))
            {
                return;
            }
            vertlists[keyForY2] = new SphericalLayoutTest.VerticesInfo();
            vertlists[keyForY2].verts = vector3Array;
            vertlists[keyForY2].normals = mesh.normals;
        }
    }

    private int GetKeyForY(int VisY, int VisZ)
    {
        return VisZ * SlotsHeight + VisY;
    }

    private void GetVisualPositionsForBoardSlot(BoardSlot slot, out int VisualX, out int VisualY, out int VisualZ)
    {
        VisualX = slot.BoardPos.X;
        VisualY = BoardSizeY - 1 - slot.BoardPos.Y;
        VisualZ = BoardSizeZ - 1 - slot.BoardPos.Z;
    }

    private void CacheSphericalProxyInfo(BoardSlot slot, BoardSlotSphericalProxy slotproxy)
    {
        int VisualX;
        int VisualY;
        int VisualZ;
        GetVisualPositionsForBoardSlot(slot, out VisualX, out VisualY, out VisualZ);
        float AzimuthToCenterDeg;
        float SlotAzimuthBreadthDeg;
        float ElevationToCenterDeg;
        float SlotElevationBreadthDeg;
        float SlotCenterRadius;
        GetSlotSphericalInfo(VisualX, VisualY, VisualZ, out AzimuthToCenterDeg, out SlotAzimuthBreadthDeg, out ElevationToCenterDeg, out SlotElevationBreadthDeg, out SlotCenterRadius);
        slotproxy.SetSphericalPositions(AzimuthToCenterDeg, SlotAzimuthBreadthDeg, ElevationToCenterDeg, SlotElevationBreadthDeg);
    }

    public void GetLayoutStuff(BoardSlot slot, ref Vector3 newPosition, ref Quaternion FacingDirection)
    {
        int VisualX;
        int VisualY;
        int VisualZ;
        GetVisualPositionsForBoardSlot(slot, out VisualX, out VisualY, out VisualZ);
        Vector3 pos;
        Vector3 norm;
        GetPositionAndNormalForSlotXAndY(VisualX, VisualY, VisualZ, out pos, out norm);
        Quaternion quaternion = Quaternion.LookRotation(norm, Vector3.up);
        newPosition = pos;
        FacingDirection = quaternion;
    }

    public class VerticesInfo
    {
        public Vector3[] verts;
        public Vector3[] normals;
    }
}
