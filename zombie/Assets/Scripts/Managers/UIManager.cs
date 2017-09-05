using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
public class UIManager : MonoBehaviour
{

    /// <summary>
     /// ui anchor
     /// </summary>
    private GameObject uianchor;
    public GameObject UIANCHOR
    {
        get
        {
            if (null == uianchor)
            {
                for (int i = 0; i < UIROOT.transform.childCount; i++)
                {
                    if (UIROOT.transform.GetChild(i).name == "Anchor")
                    {
                        uianchor = UIROOT.transform.GetChild(i).gameObject;
                        break;
                    }
                }
            }
            return uianchor;
        }
    }

     /// <summary>
     /// ui root
     /// </summary>
     GameObject root;
     public GameObject UIROOT
     {
         get
         {
             if (null == root)
                 root = Instantiate(Resources.Load(PrefabRoute + "UI Root")) as GameObject;
             return root;
         }
     }

    /// <summary>
    /// UI 预制体路径
    /// </summary>
     string PrefabRoute = "UI/Prefabs/";
    
    /// <summary>
     /// 加载指定类型的ui实例并返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="bLoad"> @ true 接口是为了加载新的UI； @false 获取当前是指定UI实例,并将结果返回</param>
    /// <returns></returns>
    /// 
     public T UI<T>(bool bLoad = true) where T : Component
     {
         Type type = typeof(T);
        
         if (bLoad)
         {
             GameObject obj = Instantiate(Resources.Load(PrefabRoute + type.Name)) as GameObject;
             obj.transform.parent = UIANCHOR.transform;
             obj.transform.localPosition = Vector3.zero;
             obj.transform.localRotation = Quaternion.identity;
             obj.transform.localScale = Vector3.one;
             T _t = obj.GetOrAddComponent<T>();
             return _t;
         }
         else
         {
             string name = type.Name;
             for (int i = 0; i < UIANCHOR.transform.childCount; i++)
             {
                 if (name == UIANCHOR.transform.GetChild(i).name)
                 {
                     return UIANCHOR.transform.GetChild(i).gameObject.GetComponent<T>();
                 }
             }
             return null;  
         }
     }
}