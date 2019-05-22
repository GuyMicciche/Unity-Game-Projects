using UnityEngine;

public static class HACK_BoardLayoutManager
{
    public static BoardLayoutManager instance;

    public static BoardLayoutManager GetLayoutManager()
    {
        if ((Object)HACK_BoardLayoutManager.instance == (Object)null)
            Debug.LogError((object)"HACK_BoardLayoutManager.instance isn't set");
        return HACK_BoardLayoutManager.instance;
    }
}