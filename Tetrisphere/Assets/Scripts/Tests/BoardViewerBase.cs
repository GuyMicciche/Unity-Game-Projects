// Decompiled with JetBrains decompiler
// Type: BoardViewerBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: ADDEA9C9-AEE1-4DC1-B840-76EBFDF8AEDE
// Assembly location: C:\Users\Guy\Desktop\sc\assets\bin\Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public abstract class BoardViewerBase : CoreMonoBehaviour
{
    private float InterpDuration = -1f;
    public Camera ViewingCamera;
    public GameBoard ActiveBoard;
    protected BoardControllerBase BoardController;
    private bool InterpActive;
    private float InterpProgressTime;
    private Vector3 NewCameraDest;
    private Vector3 StartCamearDest;

    public Transform MyTransformProp
    {
        get
        {
            return this.MyTransform;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        this.LinkToViewingCamera();
        HACK_BoardViewer.instance = this;
    }

    protected virtual void LinkToViewingCamera()
    {
        if ((Object)this.ViewingCamera == (Object)null)
            this.ViewingCamera = this.GetComponent<Camera>();
        if (!((Object)this.ViewingCamera == (Object)null))
            return;
        Debug.LogError((object)"Unable to link to ViewingCamera");
    }

    public virtual void RegisterBoardController(BoardControllerBase inController)
    {
        this.BoardController = inController;
    }

    public Camera GetViewingCamera()
    {
        return this.ViewingCamera;
    }

    private void OnDestroy()
    {
        if (!((Object)HACK_BoardViewer.instance == (Object)this))
            return;
        HACK_BoardViewer.instance = (BoardViewerBase)null;
    }

    public virtual void NotifyBoardCreated(GameBoard newBoard)
    {
        this.ActiveBoard = newBoard;
    }

    public virtual void NotifyBoardRemoved(GameBoard removedBoard)
    {
        this.ActiveBoard = (GameBoard)null;
    }

    //public virtual T GetGameShapeProxyAtScreenPosition<T>(Vector3 inScreenPosition) where T : GameShapeProxyBase
    //{
    //    T obj = (T)null;
    //    if ((Object)this.ViewingCamera != (Object)null)
    //    {
    //        RaycastHit hitInfo;
    //        if (Physics.Raycast(this.ViewingCamera.ScreenPointToRay(inScreenPosition), out hitInfo, 1000f))
    //        {
    //            GameObject gameObject = hitInfo.collider.gameObject;
    //            T gameShapeProxy = this.FindGameShapeProxy<T>(gameObject);
    //            object[] objArray = new object[5];
    //            int index1 = 0;
    //            string str1 = "Hit (";
    //            objArray[index1] = (object)str1;
    //            int index2 = 1;
    //            string name = gameObject.name;
    //            objArray[index2] = (object)name;
    //            int index3 = 2;
    //            string str2 = ") with parent shapeproxy (";
    //            objArray[index3] = (object)str2;
    //            int index4 = 3;
    //            // ISSUE: variable of a boxed type
    //            __Boxed<T> local = (object)gameShapeProxy;
    //            objArray[index4] = (object)local;
    //            int index5 = 4;
    //            string str3 = ")";
    //            objArray[index5] = (object)str3;
    //            Debug.Log((object)string.Concat(objArray));
    //            obj = gameShapeProxy;
    //        }
    //        else
    //            obj = (T)null;
    //    }
    //    return obj;
    //}

    //protected T FindGameShapeProxy<T>(GameObject inGO) where T : GameShapeProxyBase
    //{
    //    T obj = (T)null;
    //    return GameObjectHelpers.FindParentGameObjectComponent<T>(inGO);
    //}

    public bool GetCameraPositionForBoardSlot(BoardSlot slot, ref Vector3 result)
    {
        bool flag = false;
        result = Vector3.zero;
        if (slot != null)
            flag = this.GetCameraPositionForBoardPosition(slot.BoardPos, ref result);
        return flag;
    }

    public bool GetCameraPositionForBoardPosition(BoardPosition pos, ref Vector3 result)
    {
        bool flag = false;
        result = Vector3.zero;
        if (this.ActiveBoard != null)
        {
            BoardPosition pos1 = pos;
            if (this.ActiveBoard.CreateSafeBoardPosition(ref pos1))
            {
                result = HACK_BoardLayoutManager.GetLayoutManager().GetWorldCenterPositionForBoardPositionWithWeird(pos1);
                flag = true;
            }
        }
        else
            Debug.LogWarning((object)"Tried to GetCameraPositionForBoardPosition() when no ActiveBoard set");
        return flag;
    }

    public bool SetCameraPositionForBoardPosition(BoardPosition inPos, bool preserveZValue)
    {
        bool flag = false;
        Vector3 zero = Vector3.zero;
        if (this.GetCameraPositionForBoardPosition(inPos, ref zero))
        {
            if (preserveZValue)
            {
                this.SetViewingCameraPositionXY(zero);
                flag = true;
            }
            else
                Debug.LogError((object)"SetCameraPositionForBoardPosition isn't currently supported with preserveZValue false");
        }
        return flag;
    }

    public bool TweenCameraPositionToBoardPosition(BoardPosition inPos, bool preserveZValue)
    {
        bool flag = false;
        Vector3 zero = Vector3.zero;
        if (this.GetCameraPositionForBoardPosition(inPos, ref zero))
        {
            if (preserveZValue)
            {
                this.TweenViewingCameraPositionXY(zero);
                flag = true;
            }
            else
                Debug.LogError((object)"SetCameraPositionForBoardPosition isn't currently supported with preserveZValue false");
        }
        return flag;
    }

    public bool TweenCameraPositionToBoardPosition(BoardPosition inPos, bool preserveZValue, float interpDuration)
    {
        bool flag = false;
        Vector3 zero = Vector3.zero;
        if (this.GetCameraPositionForBoardPosition(inPos, ref zero))
        {
            if (preserveZValue)
            {
                this.TweenViewingCameraPositionXY(zero);
                this.InterpDuration = interpDuration;
                flag = true;
            }
            else
                Debug.LogError((object)"SetCameraPositionForBoardPosition isn't currently supported with preserveZValue false");
        }
        return flag;
    }

    public void UpdateCameraPositionForShape(GameShapeProxyBase following)
    {
        if (!((Object)following != (Object)null) || following.GetPrimaryShape() == null)
            return;
        Vector3 zero = Vector3.zero;
        if (!this.GetCameraPositionForBoardSlot(following.GetPrimaryShape().GetAnchorSlot(), ref zero))
            return;
        this.SetViewingCameraPositionXY(zero);
    }

    private void Update()
    {
        this.TickCameraMovement(Time.deltaTime);
    }

    private void TickCameraMovement(float dt)
    {
        if (!this.InterpActive)
            return;
        float num = 0.2f;
        if ((double)this.InterpDuration > 0.0)
            num = this.InterpDuration;
        this.InterpProgressTime += dt;
        if ((double)this.InterpProgressTime > (double)num)
        {
            this.InterpProgressTime = num;
            this.InterpActive = false;
            this.InterpDuration = -1f;
        }
        //this.ViewingCamera.transform.position = new Vector3(InterpHelpers.Smerp(this.StartCamearDest.x, this.NewCameraDest.x, this.InterpProgressTime / num), InterpHelpers.Smerp(this.StartCamearDest.y, this.NewCameraDest.y, this.InterpProgressTime / num), this.NewCameraDest.z);
        HACK_BoardLayoutManager.GetLayoutManager().NotifyCameraMovedToFixupLocation(new BoardPosition(0, 0, 0), this.ViewingCamera.transform.position);
    }

    public void SetViewingCameraPositionXY(Vector3 inPos)
    {
        Vector3 vector3 = inPos;
        Transform transform = this.ViewingCamera.transform;
        vector3.z = transform.position.z;
        transform.position = vector3;
    }

    private void TweenViewingCameraPositionXY(Vector3 inPos)
    {
        Vector3 vector3 = inPos;
        Transform transform = this.ViewingCamera.transform;
        vector3.z = transform.position.z;
        this.StartCamearDest = transform.position;
        this.NewCameraDest = vector3;
        this.InterpActive = true;
        this.InterpProgressTime = 0.0f;
    }

    public Vector3 ConvertToSexyTimeOffsetPosition(Vector3 posToConvert)
    {
        Vector3 zero = Vector3.zero;
        Vector3 vector3_1 = this.GetComponent<Camera>().worldToCameraMatrix.MultiplyPoint(posToConvert);
        float num = 7.5f;
        Vector3 vector3_2 = new Vector3(vector3_1.x, vector3_1.y, 0.0f);
        Vector3 v = vector3_1;
        v.z -= vector3_2.sqrMagnitude / num;
        //BoardControllerPrototype controllerPrototype = this.BoardController as BoardControllerPrototype;
        //v.z -= controllerPrototype.PullToCamera;
        return this.GetComponent<Camera>().cameraToWorldMatrix.MultiplyPoint(v);
    }
}