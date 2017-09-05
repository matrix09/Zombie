using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
public class BasePlayer : MonoBehaviour
{

    #region 获取UI管理器
    static UIManager uimgr;
    public static UIManager UIMgr{
        get{
            if (null == uimgr){
                GameObject obj = GameObject.FindGameObjectWithTag("UIMgr");
                if (null == obj)
                {
                    obj = new GameObject("UIMgr");
                    obj.tag = "UIMgr";
                }
                uimgr = obj.GetOrAddComponent<UIManager>();
            }
            return uimgr;
        }
    }
    #endregion

    #region 碰撞器
    private BoxCollider bc;
    public BoxCollider BC
    {
        get
        {
            if (null == bc)
            {
                bc = gameObject.GetComponent<BoxCollider>();
            }

            return bc;
        }
    }
    #endregion

    #region 刚体
    private Rigidbody rb;
    public Rigidbody RB
    {
        get
        {
            if (null == rb)
            {
                rb = gameObject.GetComponent<Rigidbody>();
                if (null == rb)
                    rb = gameObject.AddComponent<Rigidbody>();
            }

            return rb;
        }
    }
    #endregion

    #region 当前物体的尺寸
    private float fsize = 0f;
    public float m_fSize
    {
        get
        {
            if (0f == fsize)
            {
                fsize = BC.size.x > BC.size.y ? BC.size.x : BC.size.y;
            }
            return fsize;
        }
    }
    #   endregion

    #region 动画控制器
    Animator anim;
    public Animator ANIM
    {
        get
        {
            if (null == anim)
                anim = gameObject.GetComponent<Animator>();
            return anim;
        }
    }
    #endregion

    #region 加载角色
    protected static T LoadPrefab<T>(string path, Vector3 pos, Quaternion rot) where T : Component
    {
        UnityEngine.Object _obj = Resources.Load(path);
        if (null == _obj)
        {
            Debug.LogErrorFormat("obj is null, wrong path : {0}", path);
            return default(T);
        }

        GameObject obj = Instantiate(_obj) as GameObject;
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        T t = obj.GetOrAddComponent<T>();
        BasePlayer bp = t as BasePlayer;
        //头顶位置坐标
        bp.m_TopObj = GlobalHelper.GetGameObjectByName(obj, "Top_Head").transform;
        return t;
    }
    #endregion

    #region 角色UI位置信息
    private Transform m_TopObj;
    public Vector3 Top_HeadPos
    {
        get
        {
            return m_TopObj.position;
        }
    }

    private Vector3 uitopheadpos;
    public Vector3 UITop_HeadPos
    {
        get
        {
            return uitopheadpos;
        }
        set
        {
            if (value != uitopheadpos)
                uitopheadpos = value;
        }
    }

    private Vector3 uipos;
    public Vector3 UIPlayer_Root
    {
        get
        {
            return uipos;
        }
        set
        {
            if (value != uipos)
                uipos = value;
        }
    }
    #endregion

    #region 获取角色UI实例对象
    UIScene_BasePlayerInfos bpinfos;
    public UIScene_BasePlayerInfos UI_BasePlayerInfos
    {
        get
        {
            if (null == bpinfos)
            {
                bpinfos = BasePlayer.UIMgr.UI<UIScene_BasePlayerInfos>();
            }
            return bpinfos;
        }
    }
    #endregion

    #region 怪兽行为 ： BezierLine
    protected float curbezierPercent = 0f;
    public float m_fCurBezierPercent
    {
        get
        {
            return curbezierPercent;
        }
        set
        {
            if (value != curbezierPercent)
                curbezierPercent = value;
        }
    }
    //初始化曲线的点坐标信息和曲线模式
    protected void InitializeBezierLineInfos()
    {
        //动态创建一个游戏对象
        GameObject obj = new GameObject("BezierLine");
        //加载贝塞尔曲线到游戏对象上面
        m_BezierCurve = obj.AddComponent<BezierCurve>();

        m_BezierCurve.OnStart((BasePlayer)this);

    }

    protected BezierCurve m_BezierCurve;

    #endregion

}
