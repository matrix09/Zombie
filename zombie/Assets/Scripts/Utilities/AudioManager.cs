using UnityEngine;
public class AudioManager : MonoBehaviour {

    private static GameObject sourceObj_UI;
    private static GameObject GetUISourceObj()
    {
        if (sourceObj_UI == null)
        {
            sourceObj_UI = new GameObject();
            sourceObj_UI.name = "UIAudioObj";
            sourceObj_UI.AddComponent<AudioSource>();
            sourceObj_UI.transform.parent = Camera.main.transform;
            sourceObj_UI.transform.localPosition = Vector3.zero;
        }

        return sourceObj_UI;
    }

    public static void PlayAudio(GameObject sourceObj, AttTypeDefine.eAudioType type, string name, float volumn = -1, bool isloop = false)
    {

        string route = "";
        switch (type)
        {
            case AttTypeDefine.eAudioType.Audio_Skill:
                {
                    route = "Audios/Skill/";
                    break;
                }
            case AttTypeDefine.eAudioType.Audio_BackGround:
                {
                    route = "Audios/BackGround/";
                    sourceObj = Camera.main.gameObject;
                    isloop = true;
                    break;
                }
            case AttTypeDefine.eAudioType.Audio_UI:
                {
                    sourceObj = GetUISourceObj();
                    route = "Audios/UI/";
                    break;
                }
        }

        AudioClip ac = Resources.Load(route + name) as AudioClip;


        AudioSource[] srcs = sourceObj.GetComponents<AudioSource>();
        AudioSource aduioSrc = null;
        for (int i = 0; i < srcs.Length; i++)
        {
            if(!srcs[i].isPlaying)
            {
                aduioSrc = srcs[i];
            }
        }

        if(null == aduioSrc)
            aduioSrc = sourceObj.AddComponent<AudioSource>();

        aduioSrc.Stop();
        aduioSrc.loop = isloop;
        aduioSrc.clip = ac;
        aduioSrc.playOnAwake = true;

        if (volumn > 0)
            aduioSrc.volume = volumn;
        else
            aduioSrc.volume = 1f;

        aduioSrc.Play();

    }

}
