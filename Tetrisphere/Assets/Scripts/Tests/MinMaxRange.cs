using System;
using UnityEngine;

[Serializable]
public class MinMaxRange
{
    public float Min;
    public float Max;

    public void ShiftRange(float shiftAmount)
    {
        this.Min += shiftAmount;
        this.Max += shiftAmount;
    }

    public bool IsInRangeInclusive(float checkValue)
    {
        bool flag = false;
        if ((double)checkValue >= (double)this.Min && (double)checkValue <= (double)this.Max)
            flag = true;
        return flag;
    }

    public float GetInverseLerpForValue(float checkValue)
    {
        if ((double)this.Min == (double)this.Max)
            Debug.LogWarning((object)("GetInverseLerpForValue will have trouble because Min and Max are equal Min/Max:" + (object)this.Min));
        return Mathf.Clamp01(!this.IsInRangeInclusive(checkValue) ? ((double)checkValue >= (double)this.Min ? 1f : 0.0f) : Mathf.InverseLerp(this.Min, this.Max, checkValue));
    }

    public float GetLerpForAlpha(float alpha)
    {
        return Mathf.Lerp(this.Min, this.Max, alpha);
    }
}