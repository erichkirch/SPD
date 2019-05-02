// SOURCE: http://www.boxheadproductions.com.au/deserializing-top-level-arrays-in-json-with-unity/

using System;
using UnityEngine;


/*
    This class is used to help serailize and de-serialize JSON arrays
 */
public static class JsonHelper
{
    public static T[] FromJson<T>(string json) 
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Levels;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Levels = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Levels = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Levels;
    }
}