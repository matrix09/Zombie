using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBezierLine : MonoBehaviour {

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 250, 100), "Create Monster"))
        {
            MonsterActorV1.CreateMonster(AttTypeDefine.eMonsterType.Cactus1, Vector3.zero, Quaternion.Euler(new Vector3(0, 180, 0)), null, null, AttTypeDefine.eMonsterBehaviourType.Type_BezierLine);
        }
    }

}
