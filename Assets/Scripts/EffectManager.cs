using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class  EffectManager :MonoBehaviour
{
    private EffectManager() { }
    public static EffectManager effectManager;
    public  GameObject effect;
    public GameObject sound;
    public GameObject circleAni;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        effectManager = this;
    }


    //播放特效
    public void PlaySoundEffectAtPos(Vector3 pos) {
        effect.transform.position = pos;
        GameObject cAni = Instantiate(circleAni);
        cAni.transform.position = pos;
        cAni.GetComponent<Animator>().Play("CircleAni");
        foreach (ParticleSystem eff in effect.GetComponentsInChildren<ParticleSystem>())
        {
            eff.Play();
        }
        
        sound.GetComponent<AudioSource>().Play();

    }

}
