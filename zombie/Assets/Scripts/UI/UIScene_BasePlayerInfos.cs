using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Utilities;
public class UIScene_BasePlayerInfos : MonoBehaviour
{

    /// <summary>
    /// 伤害冒字
    /// </summary>
    HUDText m_hudText = null;
    public GameObject m_oDamage;
    
    /// <summary>
    ///保存主角实例 
    /// </summary>
    BasePlayer m_basePlayer = null;

    public void OnStart(BasePlayer target)
    {
        //保存实例对象
        m_basePlayer = target;
        //设置跟随目标
        UIPlayerInfos.FollowTarget(gameObject, target);
    }

    void Update()
    {
        //伤害冒字
        if(null != m_hudText)
         m_hudText.transform.localPosition = m_basePlayer.UITop_HeadPos;
    }

    public void TakeDamage(int damage)
    {
        if (null == m_hudText)
        {
            m_hudText = (Instantiate(Resources.Load("UI/Prefabs/RedDamage")) as GameObject).GetOrAddComponent<HUDText>();
            m_hudText.transform.parent = m_oDamage.transform;
            m_hudText.transform.localPosition = Vector3.zero;
            m_hudText.transform.localRotation = Quaternion.identity;
            m_hudText.transform.localScale = Vector3.one;
            UIPanel panel = m_hudText.gameObject.AddComponent<UIPanel>();
            panel.depth = 12;
        }
        m_hudText.Add((object)damage, Color.blue, 0f);
    }

}
