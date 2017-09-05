using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttTypeDefine;
public class TestHudText : MonoBehaviour {
    
    MonsterActorV1 monster;

    void Awake()
    {
        monster = MonsterActorV1.CreateMonster(eMonsterType.Cactus1, Vector3.zero, Quaternion.identity);
        monster.EState = eMonsterState.Monster_Null;
        monster.UI_BasePlayerInfos.OnStart((BasePlayer)monster);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Press Hud Text"))
        {
            monster.UI_BasePlayerInfos.TakeDamage(Random.Range(125, 257));
        }
    }
}
