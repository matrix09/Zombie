
namespace AttTypeDefine  {


    /// <summary>
    /// 标识曲线起点和终点的方向
    /// </summary>
    public enum eBezierLineDirection
    {
        Dir_Top,
        Dir_Bottom,
        Dir_Left,
        Dir_Right,
    }


    public enum eBezierLineConstrainedMode { 
        Free,
        Mirror,
    };

    public enum eAudioType
    {
        Audio_Skill,
        Audio_BackGround,
        Audio_UI,
    }

    public enum eMonsterBehaviourType
    {
        Type_RunAttackRun,
        Type_BezierLine,
    }


    public enum eMonsterType {
        Cactus1 = 1,
        Cactus2,
        Cactus3,
        Cactus4,
        CatBox_1,
        CatBox_2,
        CatBox_3,
        CatBox_4,
        CatBox_5,
        CatBox_6,
        CatBox_7,
        CatBox_8,
        Mushroom1,
        Mushroom2,
        CuteDragon_01,
        CuteDragon_02,
        CuteDragon_03,
        CuteDragon_04,
        CuteDragon_05,
        CuteDragon_06,
        CuteDragon_07,
        Monster_Size = CuteDragon_07,
    };

    public enum eMonsterState
    {
        Monster_Null,
        Monster_RunAway,
        Monster_Turn,
        Monster_Attack,
        Monster_Dead,
    }


    public struct MonsterMovePath
    {
        //怪兽运动节点数组
        public MonsterMoveNode[] m_MoveNode;
        //怪兽运动节点个数
        public int MonsterMoveNodeNum;
        //当前运动节点编号
        public int CurMoveNodeIndex;
    }

    //怪兽运动节点结构
    public struct MonsterMoveNode
    {
        
    }



}
