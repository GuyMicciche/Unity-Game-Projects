
using UnityEngine;

public abstract class CoreMonoBehaviour : MonoBehaviour
{
    protected GameObject MyGameObject;
    protected bool StartExecuted;

    public Transform MyTransform { get; protected set; }

    protected virtual void Awake()
    {
        this.SetMyTransform();
        this.SetMyGameObject();
    }

    protected virtual void Start()
    {
        this.StartExecuted = true;
    }

    protected virtual void SetMyTransform()
    {
        this.MyTransform = this.transform;
    }

    protected virtual void SetMyGameObject()
    {
        this.MyGameObject = this.gameObject;
    }
}
