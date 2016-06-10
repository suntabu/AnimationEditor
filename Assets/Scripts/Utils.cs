using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class Utils
{
    public static List<T> getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }
    [Serializable]
    private class Wrapper<T>
    {
        public List<T> array;
    }
}

