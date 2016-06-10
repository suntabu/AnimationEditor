using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CustomAnimationCurve
{
    private List<Keyframe> frames;

    public CustomAnimationCurve()
    {
        this.frames = new List<Keyframe>();
    }

    public CustomAnimationCurve(List<Keyframe> frames)
    {
        this.frames = new List<Keyframe>();
        this.frames.AddRange(frames);
        sortOrder();
    }


    public void addKeyFrame(Keyframe frame)
    {
        frames.Add(frame);
        sortOrder();
    }

    public void removeKeyFrame(Keyframe frame)
    {

    }


    public float evaluate(float t)
    {
        for (int i = 0; i < frames.Count; i++)
        {
            Keyframe kf = frames[i];
            if (kf.time >= t)
            {
                if (i - 1 >= 0)
                {
                    Keyframe pre = frames[i - 1];

                    float xdelta = kf.time - pre.time;
                    float ydelta = kf.value - pre.value;
                    if (ydelta == 0 || xdelta == 0)
                    {
                        return kf.value;
                    }

                    float k = ydelta / xdelta;

                    float value = k * (t - pre.time) + kf.value;
                    return value;
                }
                else
                {
                    return kf.value;
                }
            }


        }

        return frames.Count > 0 ? frames[frames.Count - 1].value : 0;
    }


    private void sortOrder()
    {
        frames.Sort(SortByX);
    }

    private int SortByX(Keyframe o1, Keyframe o2)
    {
        return o1.time.CompareTo(o2.time);
    }
}
