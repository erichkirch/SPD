using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource1; //Player attack sound
    public AudioSource efxSource2; //Enemy attack sound
    public static SoundManager instance = null;

    public void PlayerAttack()
    {
        efxSource1.Play();
    }
    public void EnemyAttack()
    {
        efxSource2.Play();
    }
}
