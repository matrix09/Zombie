using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveEditor : Editor {

    BezierCurve curve;
    Transform curveTransform;
    int m_nSelectedIndex = -1;
    float m_fDotSize = 0.08f;
    float m_fPickedDotSize = 0.16f;
    private int m_nCountStep = 10;
    float m_fDirScale = 0.2f;
    void OnSceneGUI()
    {
        //获得曲线
        curve = target as BezierCurve;
        curveTransform = curve.transform;
        Vector3 p0 = ShowPoint(0);
        for (int i = 1; i < curve.CurvePointCount; i += 3)
        {
            //显示所有的点坐标
            Vector3 p1 = ShowPoint(i );
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);
            Handles.color = Color.gray;

            //绘制点连线
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            //绘制贝塞尔曲线
            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            p0 = p3;
        }
      
        //绘制每个点的速度线段
        ShowDirection();
    }

    void ShowDirection()
    {
        int n = curve.CurvePointCount * m_nCountStep;
        Handles.color = Color.green;
        float t = 0f;
        for (int i = 0; i < n; i++)
        {
            t = i / (float)n;
            Vector3 p0 = curve.GetPoint(t);
            Handles.DrawLine(p0, p0 + curve.GetDirection(t) * m_fDirScale);
        }

    }
    
    Vector3 ShowPoint(int index)
    {
        //根据index获取坐标点
        Vector3 point = curveTransform.TransformPoint(curve[index]);

        if (Handles.Button(point, curveTransform.rotation, m_fDotSize, m_fPickedDotSize, Handles.DotHandleCap))
        {
            m_nSelectedIndex = index;
            Repaint();
        }

        //确定当前选中的就是这个点
        if (m_nSelectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();

            point = Handles.DoPositionHandle(point, curveTransform.rotation);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve[index] = curveTransform.InverseTransformPoint(point);
            }
        }
        return point;
    }

    public override void OnInspectorGUI()
    {
        curve = target as BezierCurve;

        //添加按钮，增加曲线
        if (GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(curve, "Add Curve");
            curve.AddCurve();
            EditorUtility.SetDirty(curve);
        }

        if (-1 != m_nSelectedIndex)
        {
            GUILayout.Label("Selected Point");
            //显示当前点的局部坐标
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", curve[m_nSelectedIndex]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                curve[m_nSelectedIndex] = point;
                EditorUtility.SetDirty(curve);
            }

            EditorGUI.BeginChangeCheck();
            BezierCurve.BezierLineMode mode = (BezierCurve.BezierLineMode)EditorGUILayout.EnumPopup("Mode", curve.GetCurrentIndexMode(m_nSelectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Change Point Mode");
                curve.SetCurrentIndexMode(m_nSelectedIndex, mode);
                EditorUtility.SetDirty(curve);
            }
        }
    }
}
