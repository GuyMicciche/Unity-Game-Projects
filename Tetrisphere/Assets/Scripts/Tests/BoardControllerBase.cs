using UnityEngine;

public abstract class BoardControllerBase : CoreMonoBehaviour
{
    protected BoardViewerBase BoardViewer;

    protected override void Awake()
    {
        base.Awake();
        this.LinkToBoardViewer();
    }

    public BoardViewerBase GetBoardViewer()
    {
        return this.BoardViewer;
    }

    protected virtual void LinkToBoardViewer()
    {
        this.BoardViewer = this.GetComponent<BoardViewerBase>();
        if ((Object)this.BoardViewer != (Object)null)
        {
            this.BoardViewer.RegisterBoardController(this);
        }
        else
        {
            Debug.LogError((object)"BoardController Unable to link to BoardViewer");
        }
    }
}