using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttTypeDefine;

public class MonsterActorV1 : BasePlayer
{

    #region 怪兽类型
    eMonsterBehaviourType m_eBehaviourType = eMonsterBehaviourType.Type_RunAttackRun;
    #endregion



    #region 定义击中怪兽的代理事件
    public delegate void HitMonter(int num);
    public delegate void MissMonster();
    //击中怪兽的代理事件
    public HitMonter m_delHitMonster;
    public MissMonster m_delMissMonster;
    #endregion

    #region 初始化怪物路径数据。
    static private Dictionary<eMonsterType, string> m_dMonsterPath = new Dictionary<eMonsterType, string>();
    static private bool bIsMonsterGlobalDataInitialized = false;
    //Property : 属性
    static public bool m_bIsMonsterGlobalDataInitialized
    {
        get
        {
            return bIsMonsterGlobalDataInitialized;
        }
    }

    static public void InitGlobalData()
    {
        eMonsterType type;
        //cactus
        for (type = eMonsterType.Cactus1; type <= eMonsterType.Cactus4; type++)
            m_dMonsterPath[type] = "Prefabs/Cactus" + ((int)type).ToString();
        //catbox
        for (type = eMonsterType.CatBox_1; type <= eMonsterType.CatBox_8; type++)
            m_dMonsterPath[type] = "Prefabs/CatBox" + ((int)(type - eMonsterType.Cactus4)).ToString();
        //mushroom
        for (type = eMonsterType.Mushroom1; type <= eMonsterType.Mushroom2; type++)
            m_dMonsterPath[type] = "Prefabs/Mushroom" + ((int)(type - eMonsterType.CatBox_8)).ToString();
        //CuteDragon
        for (type = eMonsterType.CuteDragon_01; type <= eMonsterType.CuteDragon_07; type++)
            m_dMonsterPath[type] = "Prefabs/cute_dragon0" + ((int)(type - eMonsterType.Mushroom2)).ToString();
    }
#endregion

    #region 出生位置
    private Vector3 m_vBirthPoint;
    #endregion

    #region 回收位置
    private Vector3 m_vDeadpoint;
    #endregion

    #region 设置运动速度
    private float m_fSpeed;
    #endregion

    #region 设置怪兽状态
    private eMonsterState m_eState = eMonsterState.Monster_Null;
    public eMonsterState EState
    {
        get
        {
            return m_eState;
        }
        set
        {
            if (value != m_eState)
                m_eState = value;
        }
    }
    #endregion

    #region 移动进度百分比
    private  float m_fPercentage;
    private float m_fCurentPercentage;
    #endregion 

    #region 是否是第一次转身攻击
    private bool m_bIsFirstAttack = true;
    #endregion

    #region 旋转变量
    private Quaternion m_qBirthQuaternion;
    private Quaternion m_qTurnQuaternion;
    private Quaternion m_qCurStateQuaternion;
    #endregion

    #region 定义转身枚举类型
    enum eTurnDirection
    {
        eTurn_Null,
        eTurn_Left,
        eTurn_Right,
    };

    eTurnDirection m_eTurn = eTurnDirection.eTurn_Null;
    #endregion

    #region 创建怪兽
    public static MonsterActorV1 CreateMonster(eMonsterType type, Vector3 pos, Quaternion rot, HitMonter _delHitMonster = null, MissMonster _delMissMonster = null, eMonsterBehaviourType bType = eMonsterBehaviourType.Type_RunAttackRun)
    {
        if (!bIsMonsterGlobalDataInitialized)
        {
            InitGlobalData();
            bIsMonsterGlobalDataInitialized = true;
        }
        string path = m_dMonsterPath[type];
        //根据指定路径，位置和旋转
        MonsterActorV1 ma = LoadPrefab<MonsterActorV1>(path, pos, rot);
        //=======================设置怪兽的基本属性===============================
        //1 设置出生位置
        ma.m_vBirthPoint = new Vector3(pos.x + ma.m_fSize, pos.y, pos.z);
        //2 设置回收未知
        //pos.x - Camera.main.position.x得到相机视野宽度
        //Camera.main.position.x - (pos.x - Camera.main.position.x) -> 相机视野左边界X坐标
        ma.m_vDeadpoint = new Vector3(2 * Camera.main.transform.position.x - pos.x - ma.m_fSize, pos.y, pos.z);

        //设置怪兽出生速度
        ma.m_fSpeed = Random.Range(3f, 5f);
        //用来判定怪兽什么时候开始转身
        ma.m_fPercentage = Random.Range(0.2f, 0.6f);
        ma.m_eState = eMonsterState.Monster_RunAway;
        //出生朝向角度
        ma.m_qBirthQuaternion = ma.transform.rotation;
        //攻击朝向角度
        ma.m_qTurnQuaternion = ma.transform.rotation * Quaternion.Euler(new Vector3(0f, -90f, 0f));
        if (null != ma.RB)
        {
            ma.RB.isKinematic = true;
            GlobalHelper.SetGameObjectLayer(ma.gameObject, "Monster");
            //ma.gameObject.layer = LayerMask.NameToLayer("Monster");
        }
        //获取当前游戏对象的动画控制器
        if (null != ma.ANIM 
            ) { }

        //启动怪兽UI
        ma.UI_BasePlayerInfos.OnStart(ma);

        //初始化shader
        ma.InitShaderProperties();

        //初始化命中怪兽代理事件
        if (null != _delHitMonster)
            ma.m_delHitMonster = _delHitMonster;

        //初始化丢失怪兽代理事件
        if (null != _delMissMonster)
            ma.m_delMissMonster = _delMissMonster;
        //===================================================================

        //设定怪兽出生行为类型
        ma.m_eBehaviourType = bType;
        if (bType == eMonsterBehaviourType.Type_BezierLine)
        {
            ma.InitializeBezierLineInfos();
        }

        return ma;
    }
    #endregion

    #region 初始化shader
    Shader m_sOrig;
    SkinnedMeshRenderer m_smr;
    Shader m_sDeadShader = null;
    float m_alpha = 0.6f;
    void InitShaderProperties()
    {
        m_smr = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        m_sOrig = m_smr.materials[0].shader;
    }
    #endregion

    #region Update
    AnimatorStateInfo asi;
    bool m_bIsDead = false;
    void Update()
    {
        RunAttackRun();
    }


    bool CheckDeathConditon()
    {
        switch (m_eBehaviourType)
        {
            case eMonsterBehaviourType.Type_RunAttackRun:
                {
                    if (
                        (transform.position.x <= m_vDeadpoint.x && !m_bIsDead) 
                        )
                        return true;
                    break;
                }
            case eMonsterBehaviourType.Type_BezierLine:
                {
                    if (
                        (m_fCurBezierPercent >= 1.0f && !m_bIsDead) 
                        )
                        return true;
                    break;
                }
        }
        return false;
    }
    /// <summary>
    /// 判断轨迹运动是否结束
    /// </summary>
    /// <returns></returns>
    bool CheckEndOfMove()
    {
        if (m_bIsDead)
            return true;
        //怪兽开始移动，判定当移动到了相机边界，那么停止运动
        if (CheckDeathConditon())
        {
            Invoke("SelfDestroy", 1.5f);
            m_bIsDead = true;
            if (null != m_delMissMonster)
                m_delMissMonster();
            return true;
        }
        return false;
    }

    float CalculateCurPercentage()
    {
        switch (m_eBehaviourType)
        {
            case eMonsterBehaviourType.Type_RunAttackRun:
                {
                    return (transform.position.x - m_vBirthPoint.x) / (m_vDeadpoint.x - m_vBirthPoint.x);
                }
            case eMonsterBehaviourType.Type_BezierLine:
                {
                    return m_fCurBezierPercent; 
                }
        }
        return -1;
    }

    void MoveMononster()
    {
        switch (m_eBehaviourType)
        {
            case eMonsterBehaviourType.Type_RunAttackRun:
                {
                    transform.Translate(new Vector3(0 - m_fSpeed * Time.deltaTime, 0f, 0f), Space.World);
                    break;
                }
            case eMonsterBehaviourType.Type_BezierLine:
                {
                    m_BezierCurve.BezierCurveMoveMonster();
                    break;
                }
        }
    }

    void RunAttackRun()
    {

        if (CheckEndOfMove())
            return;

        //计算当前运动的百分比进度
        m_fCurentPercentage = CalculateCurPercentage();
        switch (m_eState)
        {
            case eMonsterState.Monster_RunAway:
                {
                    /*
                     * 怪兽运动进度， 是否是第一次播放攻击动画 : 只有满足上述两个条件，当前运动进度 > 设定运动进度 && 是第一次进行攻击 -> 执行攻击逻辑
                     * */
                    if (m_fCurentPercentage > m_fPercentage && m_bIsFirstAttack)
                    {
                        m_bIsFirstAttack = false;
                        //设置转身模式
                        m_eState = eMonsterState.Monster_Turn;
                        //设置转身朝向
                        m_eTurn = eTurnDirection.eTurn_Left;
                        //设置当前状态旋转终点
                        m_qCurStateQuaternion = m_qTurnQuaternion;
                        return;
                    }

                    //播放前进动画
                    ANIM.SetFloat("Speed", 1.0f);
                    //修改怪兽位置
                    MoveMononster();
                    break;
                }
            case eMonsterState.Monster_Dead:
                {
                    //判定透明度
                    if (m_alpha <= 0)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    if (null == m_sDeadShader)
                    {
                        m_sDeadShader = Shader.Find("Custom/SimpleAlpha");
                        m_smr.materials[0].shader = m_sDeadShader;
                    }

                    m_smr.materials[0].SetFloat("_TransVal", m_alpha);
                    m_alpha -= Time.deltaTime * 0.5f;
                    transform.Translate(new Vector3(0f, 2 * Time.deltaTime, 0f));

                    break;
                }
            case eMonsterState.Monster_Attack:
                {

                    //获取当前的动画信息
                    asi = ANIM.GetCurrentAnimatorStateInfo(0);
                    //判定当前动画是否是idle动画
                    if (asi.fullPathHash == Animator.StringToHash("Base Layer.Idle"))
                    {
                        //代表攻击完毕, 需要转回去
                        ANIM.SetFloat("Speed", 1.0f);
                        //标识旋转方向
                        m_eTurn = eTurnDirection.eTurn_Right;
                        //标识旋转状态
                        m_eState = eMonsterState.Monster_Turn;
                        //标识旋转终点
                        m_qCurStateQuaternion = m_qBirthQuaternion;
                        return;
                    }
                    break;
                }
            case eMonsterState.Monster_Turn:
                {
                    //旋转怪兽到制定位置 -> Lerp的原理 ： Beizier Line.
                    transform.rotation = Quaternion.Lerp(transform.rotation, m_qCurStateQuaternion, 10 * Time.deltaTime);
                    //判定当前旋转是否到达指定角度
                    if (Mathf.Abs(transform.rotation.eulerAngles.y - m_qCurStateQuaternion.eulerAngles.y) < 0.1f)
                    {
                        //左转
                        if (m_eTurn == eTurnDirection.eTurn_Left)
                        {
                            //设置当前状态为攻击状态
                            m_eState = eMonsterState.Monster_Attack;
                            //播放攻击动画
                            ANIM.SetTrigger("Base Layer.Attack");
                            //AudioManager.PlayAudio(gameObject, eAudioType.Audio_Skill, "BI_sound");
                            //开启协程等待攻击结束
                            //StartCoroutine(WaitEndOfAttack());
                        }
                        //右转
                        else
                            m_eState = eMonsterState.Monster_RunAway;
                    }
                    break;
                }
        }
    }

    #endregion

    #region 判定是否点中怪物

    bool IsPointMonster()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 MonsterPos = transform.position;
            Vector3 pos = Camera.main.ScreenToWorldPoint(
                new Vector3(
                     Input.mousePosition.x,
                    Input.mousePosition.y,
                    transform.position.z - Camera.main.transform.position.z
                    )
            );

            if (
                (Mathf.Abs(pos.x - MonsterPos.x) <= (BC.size.x / 2f * transform.localScale.x))
                &&
                ((pos.y - MonsterPos.y) >= 0f && (pos.y - MonsterPos.y) <= (BC.size.y * transform.localScale.y))
                )
            {
                return true;
            }

            return false;
        }
        return false;
       
    }

    #endregion

    #region 等待攻击结束
    IEnumerator WaitEndOfAttack()
    {
        //等待一帧，让怪兽开始播放攻击动画
        //yield return null;

        while (true)
        {

            //if (ANIM.IsInTransition(0))
            //{
            //    yield return null;
            //}
            //获取当前的动画信息
            AnimatorStateInfo asi = ANIM.GetCurrentAnimatorStateInfo(0);
            //判定当前动画是否是idle动画
            if (asi.fullPathHash == Animator.StringToHash("Base Layer.Idle"))
                break;
            //else if (asi.fullPathHash == Animator.StringToHash("Base Layer.Locomotion"))
            //{
            //    yield return null;
            //}
            //else if (asi.fullPathHash == Animator.StringToHash("Base Layer.Attack"))
            //{
               
            //    yield return null;
            //}
            else
                yield return null;
        }

        //代表攻击完毕, 需要转回去
        ANIM.SetFloat("Speed", 1.0f);
        //标识旋转方向
        m_eTurn = eTurnDirection.eTurn_Right;
        //标识旋转状态
        m_eState = eMonsterState.Monster_Turn;
        //标识旋转终点
        m_qCurStateQuaternion = m_qBirthQuaternion;
    }
    #endregion

    #region 利用触发事件检测是否碰到怪兽
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Knife")
        {
            asi = ANIM.GetCurrentAnimatorStateInfo(0);
            if (asi.fullPathHash == Animator.StringToHash("Base Layer.Attack"))
            {
                int num = Random.Range(10, 30);
                //执行加分动画
                m_delHitMonster(num);
                UI_BasePlayerInfos.TakeDamage(num);
                Destroy(BC);
                //添加特效
                Instantiate(Resources.Load("IGSoft_Projects/SceneEffects/Common/E_Babble"),
                    new Vector3(transform.position.x, transform.position.y + BC.size.y/2f, transform.position.z)
                    , Quaternion.identity);
                AudioManager.PlayAudio(gameObject, eAudioType.Audio_Skill, "skill_addbuff");
                m_eState = eMonsterState.Monster_Dead;
                ANIM.SetTrigger("Base Layer.Die");
            }
        }
    }

    #endregion

    #region 利用raycast检测是否碰到怪兽
    bool IsRayCastMonster()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //获得从相机到inPos的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线的距离
            float dist = Camera.main.farClipPlane - Camera.main.nearClipPlane;
            //射线碰撞的目标单位
            int mask = 1 << LayerMask.NameToLayer("Monster");

            //返回的碰撞信息
            RaycastHit hitInfo;
            //检测碰撞
            if (Physics.Raycast(ray, out hitInfo, dist, mask))
            {
                //绘制射线    
                if (gameObject.name == hitInfo.transform.name)
                {
                    Debug.DrawLine(Camera.main.transform.position, hitInfo.point, Color.blue);
                    return true;
                }
             
            }

            return false;
        }
        return false;
    }

    #endregion

    #region 销毁自己
    void SelfDestroy()
    {

        if (m_eBehaviourType == eMonsterBehaviourType.Type_BezierLine)
        {
            BezierCurve.BezierCurveSelfDestroy(m_BezierCurve);
        }

        if (null != UI_BasePlayerInfos)
            Destroy(UI_BasePlayerInfos.gameObject);
        Destroy(gameObject);
    }
    #endregion
}
