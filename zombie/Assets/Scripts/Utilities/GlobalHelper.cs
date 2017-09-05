using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalHelper  {


    public static int GetPowTwo(int num)
    {
        if (num % 2 != 0)
            return -1;
        int totalnum = 0;
        while (num != 1)
        {
            num >>= 1;
            totalnum++;
        }
        return totalnum;
      
    }


    public static Rect GetMainCameraRect()
    {

        float halfWidth = Camera.main.orthographicSize * Camera.main.aspect; ;

        float halfHeight = Camera.main.orthographicSize;

        return new Rect(0, 0, halfWidth, halfHeight);
    }
    
    public static void SetGameObjectLayer(GameObject obj, string name)
    {
        if (null == obj)
            return;
        obj.layer = LayerMask.NameToLayer(name);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetGameObjectLayer(obj.transform.GetChild(i).gameObject, name);
        }
    }

    public static GameObject GetGameObjectByName(GameObject obj, string name)
    {
        if (null == obj)
            return null;
        if (obj.name == name)
            return obj;
        GameObject _obj;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            if (null != (_obj = GetGameObjectByName(obj.transform.GetChild(i).gameObject, name)))
            {
                return _obj;
            }
        }
        return null;
    }
}
