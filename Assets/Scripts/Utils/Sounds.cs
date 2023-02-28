using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sounds 
{
    [HideInInspector]
    public AudioSource source; //audio source
  
        


    public string name; //name for the sound effect
    public AudioClip clip; // Sound clip for the given effect

    [Range(0,1f)]
    public float volume; //volume of sound effect
    [Range(0.1f, 3f)]
    public float pitch; //pitch of sound effect
    public bool loop; //is sound effect has to be looped
    public bool playOnAwake;
}
