using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttTypeDefine;
public class MonsterLauncher : MonoBehaviour {

    public void OnStart(UIScene_Game _game)
    {
        float width = Camera.main.orthographicSize * Camera.main.aspect;

        //设置怪物的位置
        transform.position = new Vector3(Camera.main.transform.position.x + width + 2, transform.position.y, 0f);

        StartCoroutine(GeneratingMonster(_game));
        //MonsterActorV1.CreateMonster(eMonsterType.Cactus1, transform.position, transform.rotation);
    }

    IEnumerator GeneratingMonster(UIScene_Game _game)
    {
        while (true)
        {
            float n = Random.Range(4f, 7f);
            int m = Random.Range(1, (int)(eMonsterType.Monster_Size + 1));
            eMonsterType type = (eMonsterType)m;
            MonsterActorV1.CreateMonster(type, transform.position, transform.rotation, _game.HitMonster, _game.MissMonster, eMonsterBehaviourType.Type_BezierLine);
            yield return new WaitForSeconds(n);
        }
    }

 
}
