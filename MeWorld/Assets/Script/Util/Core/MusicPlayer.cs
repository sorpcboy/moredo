using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour 
{
    public  AudioSource Sound;
    private static AudioSource _Sound;
    public enum WHICH_SOUND
    {
        NOTHING, PUTCUBE, BREAKCUBE
    };

    void Start() 
    {
        _Sound = Sound;
    }

    public static void Play(WHICH_SOUND whichSound)
    {
        ///< 调用Resources方法加载AudioClip资源
        switch (whichSound)
        {
            case WHICH_SOUND.PUTCUBE:
                _Sound.clip = (AudioClip)Resources.Load("Sounds/put", typeof(AudioClip));
                break;
            case WHICH_SOUND.BREAKCUBE:
                _Sound.clip = (AudioClip)Resources.Load("Sounds/dig", typeof(AudioClip));
                break;
            default:
                return;
        }
        ///< 播放
        _Sound.Play();
    }

    void OnDestroy()
    {
        _Sound = null;
    }
}
