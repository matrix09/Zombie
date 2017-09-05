/*
 * class one
 * what is override scene gui and inspector gui
 * what is Reset
 * what is SerializeField
 * what is property instance
 * what is TransformPoint, InverseTransformPoint
 * 
 * class two
 * 
 *  2.1在Inspector中添加按钮"Add Curve"
         循环处理所有的点 -> OnSceneGUI -> for (int i = 1; i < curve.PointNum; i+=3)
     2.2在Inspector中绘制当前选中的按钮信息
     2.3when to use repaint();
3 ： 在曲线上取点取切线，来决定模型的轨迹和模型的朝向GetPoint, GetDirection, 
3.1 完成例子程序
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AttTypeDefine;
public class TestBezierLineForStu : MonoBehaviour
{

    #region 添加限制模式
    //主要目的 ： 限制曲线连接处的mode
    [SerializeField]
    eBezierLineConstrainedMode[] m_eMode;

    //根据点的index获取对应的mode数组中的mode类型
    public eBezierLineConstrainedMode GetModeByPointIndex(int index)
    {
        return m_eMode[(index + 1)/3];
    }

    public void SetBezierLineConstrainedMode(int index, eBezierLineConstrainedMode mode)
    {
        m_eMode[(index + 1) / 3] = mode;
        EnforceMode(index);
    }

    /// <summary>
    /// 1在inspector中修改模式的时候，要调用此接口
    /// 2当在sceneview中修改点的坐标的时候需要调用此接口
    /// 功能介绍 ： 将曲线mode为mirror的点进行mirror模式计算。这样做可以让曲线连接处的速度不会出现骤变
    /// 
    /// 基本逻辑 ： 
    /// 
    /// 判断入参Index是否合法，
    /// 
    /// 判断入参Index对应的mode是否是mode的开始或者mode的结束为止
    /// 
    /// 判断入参Index对应的mode数，是否为free，如果是则直接返回
    /// 
    /// 
    /// </summary>
    /// <param name="index">Point Index</param>
    public void EnforceMode(int index)
    {
        if (index < 0 || index > PointNum)
            return;
        
        int modeIndex = (index + 1) / 3;
        if (modeIndex == 0 || modeIndex == m_eMode.Length - 1)
            return;
        
        if(m_eMode[modeIndex] == eBezierLineConstrainedMode.Free)
            return;
        else if(m_eMode[modeIndex] == eBezierLineConstrainedMode.Mirror)
        {
            int middleIndex = modeIndex * 3;
            Vector3 middlePoint, fixedPoint, enforcedPoint;
            int fixedIndex, enforcedIndex;
            middlePoint = m_vPoints[middleIndex];
            if (index > middleIndex)
            {
               
                fixedIndex = middleIndex + 1;
                enforcedIndex = middleIndex - 1;
            }
            else
            {
                fixedIndex = middleIndex - 1;
                enforcedIndex = middleIndex + 1;
            }

            fixedPoint = m_vPoints[fixedIndex];
            enforcedPoint = middlePoint + (middlePoint - fixedPoint).normalized * Vector3.Distance(middlePoint, fixedPoint);
            m_vPoints[enforcedIndex] = enforcedPoint;
        }
            

    }

    #endregion

    //绘制曲线，首先我们需要有顶点。
    [SerializeField]
    Vector3[] m_vPoints;

    public GameObject m_oCube;

    float m_fCurPercent = 0f;

    MonsterActorV1 m_monster;

    void Update()
    {
        if (m_fCurPercent >= 1.0f)
        {
            Destroy(gameObject);
            return;
        }

        m_oCube.transform.position = GetPoint(m_fCurPercent);
        Vector3 dir = GetDirection(m_fCurPercent);
        m_oCube.transform.LookAt(m_oCube.transform.position + dir);
        m_fCurPercent += 0.2f * Time.deltaTime;
    }

    public Vector3 this[int index]
    {
        get
        {
            return m_vPoints[index];
        }
        set
        {
            if (value != m_vPoints[index])
            {

                if (index % 3 == 0)
                {
                    Vector3 delta = m_vPoints[index] - value;
                    if (index > 0 && index < m_vPoints.Length - 1)
                    {
                        m_vPoints[index + 1] += delta;
                        m_vPoints[index - 1] += delta;
                    }
                }

                m_vPoints[index] = value;
                EnforceMode(index);
            }
          
        }
    }

    public void Reset()
    {
        m_vPoints = new Vector3[] {
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(3, 0, 0),
            new Vector3(4, 0, 0)
        };

        m_eMode = new eBezierLineConstrainedMode[] { 
            eBezierLineConstrainedMode.Free,
            eBezierLineConstrainedMode.Free
        };
    }

    public int PointNum
    {
        get
        {
            return m_vPoints.Length;
        }
    }
    public void AddCurve()
    {
        Vector3 point = m_vPoints[PointNum - 1];
        Array.Resize(ref m_vPoints, PointNum + 3);
        point.x += 1;
        m_vPoints[PointNum - 3].x = point.x;
        point.x += 1;
        m_vPoints[PointNum - 2].x = point.x;
        point.x += 1;
        m_vPoints[PointNum - 1].x = point.x;

        //在添加曲线的时候，增加mode数组大小，并给他初始化。
        Array.Resize(ref m_eMode, m_eMode.Length + 1);
        m_eMode[m_eMode.Length - 1] = eBezierLineConstrainedMode.Free;
    }

    public int CurveNum
    {
        get {
            return (PointNum - 1) / 3;
        }
    }

    //根据进度获取点
    public Vector3 GetPoint(float t)
    {
        int i;
        if (t >= 1.0f)
        {
            t = 1.0f;
            i = PointNum - 4;
        }
        else
        {
            t *= CurveNum;
            i = (int)t;
            t = t - i;
            i *= 3;
        }
        return transform.TransformPoint(TestBezierLineForStu.GetPoint(m_vPoints[i], m_vPoints[i + 1], m_vPoints[i + 2], m_vPoints[i + 3], t));
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public Vector3 GetVelocity(float t)
    {
        int i;
        if (t >= 1.0f)
        {
            t = 1.0f;
            i = PointNum - 4;
        }
        else
        {
            t *= CurveNum;
            i = (int)t;
            t = t - i;
            i *= 3;
        }
        return transform.TransformPoint(GetFirstDerivative(m_vPoints[i], m_vPoints[i + 1], m_vPoints[i + 2], m_vPoints[i + 3], t)) - transform.position;
    }

    public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }

}
