using UnityEngine;
using System.Collections;

public class FittingValue
{
    private float mXRatio = 0;
    private float mYRatio = 0;
    private float[] mValue;
    private CustomAnimationCurve mPathCurveX;
    private CustomAnimationCurve mPathCurveY;

    public FittingValue(float[] values, float xRatio, float yRatio)
    {
        mXRatio = xRatio;
        mYRatio = yRatio;
        mValue = values;
        InitData();
    }

    public void InitData()
    {
        if (mPathCurveX == null)
        {
            mPathCurveX = new CustomAnimationCurve();
            mPathCurveY = new CustomAnimationCurve();
            for (int i = 0; i < mValue.Length; i++)
            {
                if (i % 3 == 0)
                {
                    float x = mValue[i] * mXRatio;
                    float y = mValue[i + 1] * mYRatio;
                    float t = mValue[i + 2];


                    mPathCurveX.addKeyFrame(new Keyframe(t, x));
                    mPathCurveY.addKeyFrame(new Keyframe(t, y));
                }
            }

        }
    }

    public Vector2 GetValue(float t)
    {
        var v = new Vector2(mPathCurveX.evaluate(t),mPathCurveY.evaluate(t));
        return v;
    }

}
