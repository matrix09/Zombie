/*Attention : 
 * 1 添加mode操作来解决两条曲线连接处共用的顶点切线速度不一致的问题，如果不讲共用顶点问题解决，那么模型在运动过程中往往会出现运动方向骤然转变，非常影响体验。
 * 2 将整个地图分成16个小区域，初始化这16个小区域
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AttTypeDefine;
public class BezierCurve : MonoBehaviour {

    public enum BezierLineMode
    {
        Free,
        Mirror,
    };

    [SerializeField]
    BezierLineMode[] modes;

    /*
 * 规则 ：
 * 曲线的个数 ： 1 - 3条
 * 
 * 
 * */
    const int MinCurve = 2;
    const int MaxCurve = 4;
    //获取点方向
    public eBezierLineDirection RandomBezierDir
    {
        get
        {
            int n = UnityEngine.Random.Range(0, 4);
            return (eBezierLineDirection)n;
        }
    }
    public enum eBezierLineSetPointMode{
        Mode_StartPoint,//起点
        Mode_EndPoint,//终点
    }
    eBezierLineDirection m_eStartPointDir;
    eBezierLineDirection m_eEndPointDir;

    const int m_nRectAreaNum = 4;
    static Rect[] m_RectAreas = new Rect[m_nRectAreaNum];
    static bool m_bInit = false;
    static void InitializeSingleRectAreas()
    {
        if (m_bInit)
            return;
        int powtwo = GlobalHelper.GetPowTwo(m_nRectAreaNum);
        m_bInit = true;
        int singleWidth = (int)GlobalHelper.GetMainCameraRect().width / powtwo * 2;
        int singleHeight = (int)GlobalHelper.GetMainCameraRect().height / powtwo * 2;
        for (int i = 0; i < m_nRectAreaNum / powtwo; i++)
        {
            for (int j = 0; j < m_nRectAreaNum / powtwo; j++)
            {
                m_RectAreas[i * powtwo + j] = new Rect(
                        0 - GlobalHelper.GetMainCameraRect().width + j * singleWidth,
                        GlobalHelper.GetMainCameraRect().height - singleHeight * i,
                        singleWidth,
                        singleHeight
                    );
            }
        }
    }

    //获取边缘点点坐标
    Vector3 GetTerminalPoint(eBezierLineSetPointMode settingpointmode)
    {
        eBezierLineDirection dir = RandomBezierDir;
        Vector3 point = Vector3.zero;
         float size = m_bp.m_fSize * 2;
        //获取位置模式
        switch(settingpointmode) {
            case eBezierLineSetPointMode.Mode_StartPoint:{
                m_eStartPointDir = dir;
                break;
            }
            case eBezierLineSetPointMode.Mode_EndPoint:{
                m_eEndPointDir = dir;
                break;
            }
        }

               

        switch(dir) {
            case eBezierLineDirection.Dir_Left:{
                  point = new Vector3(
                        Camera.main.transform.position.x - GlobalHelper.GetMainCameraRect().width - size,
                        UnityEngine.Random.Range(-GlobalHelper.GetMainCameraRect().height, GlobalHelper.GetMainCameraRect().height),
                        0f
                  );
                break;
            }
            case eBezierLineDirection.Dir_Right:{
                point = new Vector3(
                      Camera.main.transform.position.x + GlobalHelper.GetMainCameraRect().width + size,
                      UnityEngine.Random.Range(-GlobalHelper.GetMainCameraRect().height, GlobalHelper.GetMainCameraRect().height),
                      0f
                );
                break;
            }
            case eBezierLineDirection.Dir_Top:{
                point = new Vector3(
                      UnityEngine.Random.Range(Camera.main.transform.position.x - GlobalHelper.GetMainCameraRect().width, Camera.main.transform.position.x + GlobalHelper.GetMainCameraRect().width),
                       Camera.main.transform.position.y + GlobalHelper.GetMainCameraRect().height + size,
                      0f
                );
                break;
            }
            case eBezierLineDirection.Dir_Bottom:{
                point = new Vector3(
                      UnityEngine.Random.Range(Camera.main.transform.position.x - GlobalHelper.GetMainCameraRect().width, Camera.main.transform.position.x + GlobalHelper.GetMainCameraRect().width),
                       Camera.main.transform.position.y - GlobalHelper.GetMainCameraRect().height - size,
                      0f
                );
                break;
            }
        }

        return point;
    }

    //曲线归属对象
    BasePlayer m_bp;
    public void OnStart(BasePlayer bp)
    {

        //创建一个游戏对象(朝向位置 : GetPoint() + GetDirective)
        NextObj = new GameObject("NextObj");

        //初始化地图小区域 
        InitializeSingleRectAreas();

        //设置曲线归属
        m_bp = bp;

        //设置曲线个数
        int curveNum = UnityEngine.Random.Range(MinCurve, MaxCurve);

        //设置曲线点个数
        int pointNum = curveNum * 3 + 1;

        //实例化坐标
        points = new Vector3[pointNum];
        //设置曲线模式的个数
        modes = new BezierLineMode[CurveCount + 1];

        //初始化起点和终点
        points[0] = GetTerminalPoint(eBezierLineSetPointMode.Mode_StartPoint);
        points[CurvePointCount - 1] = GetTerminalPoint(eBezierLineSetPointMode.Mode_EndPoint);

        //设置剩余曲线点的位置
        /*
         * 判定规则 ; 将空间分成4个区域，选择哪个区域是随机计算的。
         * */
        for (int i = 1; i < CurvePointCount - 1; i++)
        {
            int n = UnityEngine.Random.Range(0, m_nRectAreaNum);
            Rect rect = m_RectAreas[n];
            points[i] = new Vector3(
                UnityEngine.Random.Range(rect.x, rect.x + rect.width),
                UnityEngine.Random.Range(rect.y, rect.y - rect.height),
                0f
                );
            SetCurrentIndexMode(i, BezierLineMode.Mirror);
        }       
    }

    public Vector3 this[int index] {

        get
        {
            return points[index];
        }
        set
        {              
            if (index % 3 == 0)
            {
                Vector3 delta = value - points[index];

                if (index > 0)
                {
                    points[index - 1] += delta;
                }

                if (index < CurvePointCount)
                {
                    points[index + 1] += delta;
                }
            }
            points[index] = value;
            EnforceMode(index);
        }
    }

    [SerializeField]
    Vector3[] points;

    //顶点个数
    public int CurvePointCount
    {
        get
        {
            return points.Length;
        }
    }

    //曲线个数
    public int CurveCount
    {
        get
        {
            return (CurvePointCount - 1) / 3;
        }
    }
    GameObject NextObj;
    const float m_fSpeed = 0.12f;
    //void Update()
    //{
    //    if (m_fCurPercent >= 1.0f){
    //        Destroy(NextObj);
    //        Destroy(m_bp.gameObject);
    //        Destroy(gameObject);
    //        return;
    //    }
        
    //}
    public static void BezierCurveSelfDestroy(BezierCurve curve)
    {
        if (null != curve)
        {
            Destroy(curve.NextObj);
            Destroy(curve.gameObject);
        }
    }
    public void BezierCurveMoveMonster()
    {
        m_bp.transform.position = GetPoint(m_bp.m_fCurBezierPercent);
        NextObj.transform.position = GetPoint(m_bp.m_fCurBezierPercent);
        Vector3 Dir = GetDirection(m_bp.m_fCurBezierPercent);
        NextObj.transform.LookAt(
         NextObj.transform.position + Dir
           );
        m_bp.transform.rotation = Quaternion.Lerp(m_bp.transform.rotation, NextObj.transform.rotation, 5 * Time.deltaTime);

        m_bp.m_fCurBezierPercent += (m_fSpeed * Time.deltaTime);
    }


    #region 模式接口
    /// <summary>
    /// 1 : 在Inspector中Enforce mode.
    /// 2：在Scene中Enforce mode.
    /// </summary>
    public BezierLineMode GetCurrentIndexMode(int index)
    {
        return modes[(index + 1) / 3];
    }

    public void SetCurrentIndexMode(int index, BezierLineMode _mode)
    {
        modes[(index + 1) / 3] = _mode;
        EnforceMode(index);
    }

    public void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierLineMode mode = modes[modeIndex];

        /*判定返回条件
         * 1如果当前点的模式是free，那么不用进行后面的操作计算
         * 2如果当前点是开始节点或者最终节点，不需要进行后面的操作计算
         * */
        if (mode == BezierLineMode.Free || index == 0 || index == CurvePointCount - 1)
        {
            return;
        }

        Vector3 middlePoint, fixedPoint, enforcedPoint;
        int middleIndex, fixedIndex, enforcedIndex;
        middleIndex = modeIndex*3;
     
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

        if (enforcedIndex > CurvePointCount - 1 || enforcedIndex < 0)
            return;

        middlePoint = points[middleIndex];
        if (mode == BezierLineMode.Mirror)
        {
            fixedPoint = points[fixedIndex];
            enforcedPoint = middlePoint + (middlePoint - fixedPoint).normalized * Vector3.Distance(middlePoint, fixedPoint);
            points[enforcedIndex] = enforcedPoint;
        }
    }
    #endregion

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

    public Vector3 GetPoint(float t)
    {

        int i;
        if (t >= 1.0f)
        {
            t = 1.0f;
            i = CurvePointCount - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;//取整 判定是第几根曲线
            t -= i;//取余 判定是当前曲线的百分之多少
            i *= 3;
        }
      
        return  transform.TransformPoint(BezierCurve.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    //计算点坐标的导数-切线方向的速度
    public static Vector3 GetFirstDirective(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            3f * oneMinusT * oneMinusT * (p1 - p0) +
            6f * oneMinusT * t * (p2 - p1) +
            3f * t * t * (p3 - p2);
    }

    //获取速度
    public  Vector3 GetVelocity(float t)
    {
        
        int i;
        if (t >= 1.0f)
        {
            t = 1.0f;
            i = CurvePointCount - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;//取整 判定是第几根曲线
            t -= i;//取余 判定是当前曲线的百分之多少
            i *= 3;
        }

        return transform.TransformPoint(BezierCurve.GetFirstDirective(points[i], points[i +1], points[i + 2], points[i + 3], t)) - transform.position;

    }

    //获取方向
    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Array.Resize(ref points, points.Length + 3);
        point.x += 1;
        points[points.Length - 3] = point;
        point.x += 1;
        points[points.Length - 2] = point;
        point.x += 1;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(modes.Length - 4);
    }

}
