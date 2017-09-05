using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AttTypeDefine;
//告诉系统我们需要扩展那个脚本的编辑器
[CustomEditor(typeof(TestBezierLineForStu))]
public class TestBezierLineForStuEditor : Editor {
    int m_nSelectedIndex = -1;
    //根据输入的索引返回索引对应的点坐标
    Vector3 ShowPoint(int index)
    {
        //首先我需要TestBezierLineForStu的实例对象

        //获取curve里面的点坐标
        Vector3 p = curveTransform.TransformPoint(curve[index]);

        EditorGUI.BeginChangeCheck();


        if (curve.GetModeByPointIndex(index) == eBezierLineConstrainedMode.Mirror)
        {
            Handles.color = Color.cyan;
        }
        else
        {
            Handles.color = Color.white;
        }

        //1 : 当我们选中某一个点的时候，才会去执行DoPositionHandle，否则只会显示一个立方体.
        if (Handles.Button(p, curveTransform.rotation, 0.08f, 0.16f, Handles.DotHandleCap))
        {
            m_nSelectedIndex = index;
            Repaint();
        }


        if (index == m_nSelectedIndex)
        {
            //2 ： 将这四个点显示在scene view中
            p = Handles.DoPositionHandle(p, curveTransform.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                curve[index] = curveTransform.InverseTransformPoint(p);
                EditorUtility.SetDirty(curve);
            }
        }
     
        //返回曲线的指定index的点坐标
        return p;
    }

    TestBezierLineForStu curve;
    Transform curveTransform;
    public void OnSceneGUI()
    {

        //获取曲线实例对象
         curve = target as TestBezierLineForStu;
         curveTransform = curve.transform;
         Vector3 p0 = ShowPoint(0);
          for (int i = 1; i < curve.PointNum; i += 3)
          {
              Vector3 p1 = ShowPoint(i);
              Vector3 p2 = ShowPoint(i + 1);
              Vector3 p3 = ShowPoint(i + 2);

              Handles.color = Color.gray;
              Handles.DrawLine(p0, p1);
              Handles.DrawLine(p2, p3);

              Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
              p0 = p3;
          }
          
          //显示所有点的切线方向
          //ShowDirection();      
    }

    int m_nLineSteps = 10;
    float m_fScaleDir = 0.6f;
    void ShowDirection()
    {
        Vector3 point;
        Handles.color = Color.green;
        for (int i = 0; i < curve.PointNum * m_nLineSteps; i++)
        {
            point = curve.GetPoint(i / (float)(curve.PointNum * m_nLineSteps));
            Handles.DrawLine(point, point + curve.GetDirection(i / (float)(curve.PointNum * m_nLineSteps)) * m_fScaleDir);
        }
    }

    public override void OnInspectorGUI()
    {
        curve = target as TestBezierLineForStu;
        //绘制选中的point
        if (m_nSelectedIndex != -1)
        {
            DrawSelectedIndexPoint();
        }

        GameObject source = EditorGUILayout.ObjectField(curve.m_oCube, typeof(GameObject), true) as GameObject;
        if (source != curve.m_oCube)
        {
            Undo.RecordObject(curve, "Add Cube");
            curve.m_oCube = source;
            EditorUtility.SetDirty(curve);
        }
       

        if(GUILayout.Button("Add Curve")) {
            Undo.RecordObject(curve, "Add Curve");
            curve.AddCurve();
            EditorUtility.SetDirty(curve);
        }



    }   

    void DrawSelectedIndexPoint()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 p = EditorGUILayout.Vector3Field("point position", curve[m_nSelectedIndex]);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Change Point");
            curve[m_nSelectedIndex] = p;
            EditorUtility.SetDirty(curve);
        }

        EditorGUI.BeginChangeCheck();
        eBezierLineConstrainedMode mode = (eBezierLineConstrainedMode)EditorGUILayout.EnumPopup(curve.GetModeByPointIndex(m_nSelectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Change Mode");
            curve.SetBezierLineConstrainedMode(m_nSelectedIndex, mode);
            EditorUtility.SetDirty(curve);
        }




    }
}
