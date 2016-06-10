using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[Serializable]
public class EffectModel
{
    public string effectName;

    public float startTime;
    public float stopTime;
    public float[] size;
    public float[] pathValues;
    public float[] scaleValues;
    public float[] rotateValues;
    public float[] position;



    private FittingValue pathFitting;
    private FittingValue scaleFitting;


    public FittingValue PathFitting
    {
        get
        {
            if (pathFitting == null)
            {
                pathFitting = new FittingValue(pathValues, ControlManager.container_width, ControlManager.container_height);
            }
            return pathFitting;
        }
    }

    public FittingValue ScaleFitting
    {
        get
        {
            if (scaleFitting == null)
            {
                scaleFitting = new FittingValue(scaleValues, 1, 1);
            }
            return scaleFitting;
        }
    }

    public RawImage ImageGameObject { get; set; }
}
