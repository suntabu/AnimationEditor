using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public class AnimationModel
      
{

    public string animationName;

    public float duration;

    public List<EffectModel> effects;

    public bool IsInit { get;  set; }



}
