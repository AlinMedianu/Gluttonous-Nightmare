using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ThemeFade : MonoBehaviour
{
    private AudioSource themePlayer;

    private void Start()
    {
        themePlayer = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (PlayerController.dead && themePlayer.volume > 0)
            themePlayer.volume -= Time.deltaTime;
    }

}
