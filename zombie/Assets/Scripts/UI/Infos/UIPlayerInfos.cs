using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
public class UIPlayerInfos : MonoBehaviour {

    /// <summary>
    /// 标识当前个人信息跟随的对象
    /// </summary>
    public BasePlayer m_sBasePlayer;

    public static UIPlayerInfos FollowTarget(GameObject owner, BasePlayer target)
    {
        if (null == owner || null == target)
        {
            Debug.LogError("Input parameter illegal");
            return null;
        }

        UIPlayerInfos ft = owner.GetOrAddComponent<UIPlayerInfos>();
        ft.m_sBasePlayer = target;

        return ft;
    }

    /// <summary>
    /// 标识当前游戏的主相机
    /// </summary>
    public Camera m_cGameCamera;

    /// <summary>
    /// 标识当前游戏的UI相机
    /// </summary>
    public Camera m_cUICamera;

    void Start()
    {
        if (null == m_cGameCamera) m_cGameCamera = Camera.main;
        if (null == m_cUICamera) m_cUICamera = NGUITools.FindCameraForLayer(gameObject.layer);
    }

    #region 定义描述角色位置的枚举类型
    enum PosType
    {
        ePos_Null,
        ePos_PlayerTopHead,
        ePos_PlayerRoot,
    }
    #endregion


    /// <summary>
    /// 将世界坐标转换成对应的ui坐标
    /// </summary>
    /// <param name="_pos">角色指定位置的世界坐标</param>
    /// 计算的位置信息是相对于UIPlayerInfos挂接的游戏对象来计算的
    void TranslateCoordinate(PosType postype, Vector3 _pos)
    {
        Vector3 pos = m_cGameCamera.WorldToViewportPoint(_pos);

        int bIsVisible = (m_cGameCamera.orthographic || pos.z > 0) && (pos.x > 0 && pos.x < 1 && pos.y > 0 && pos.y < 1) ? 1 : 0;

        if (1 == bIsVisible)
        {
            //计算获取角色在ui坐标系下的局部坐标
            pos = m_cUICamera.ViewportToWorldPoint(pos);
            switch (postype)
            {
                case PosType.ePos_PlayerRoot: { 
                    m_sBasePlayer.UIPlayer_Root = transform.InverseTransformPoint(pos); 
                    break; 
                }
                case PosType.ePos_PlayerTopHead: { 
                    m_sBasePlayer.UITop_HeadPos = transform.InverseTransformPoint(pos); 
                    break; 
                }
            } 
        }
    }

    void Update()
    {
        if (null != m_sBasePlayer && null != m_cUICamera)
        {
            /*
             * 世界坐标
             * 模型坐标
             * 屏幕坐标
             * 视口坐标 ： 范围在x， y方向是0 - 1。标识当前相机视口大小的百分比
             * 视口大小 ： 对于NGUI来说是高度为2，宽度根据当前设备宽高比计算
             * */
            //获取
            TranslateCoordinate(PosType.ePos_PlayerRoot, m_sBasePlayer.transform.position);
            TranslateCoordinate(PosType.ePos_PlayerTopHead ,m_sBasePlayer.Top_HeadPos);
        }
        else if (null == m_sBasePlayer && null != m_cUICamera)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogError("Error Logic");
        }
    }

}
