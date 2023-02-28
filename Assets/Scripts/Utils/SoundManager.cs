using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
public class SoundManager : MonoBehaviour
{

    public Sounds[] sounds; //reference to Sound Class
    static SoundManager sM; //reference to this gameobject (Sound Manager)

    private void Awake()
    {
        DontDestroyOnLoad(this); //Sound manager instance is never destroyed on scene change
        if(sM == null)
        {
            sM = this;
        }
        else
        {
            Destroy(gameObject); //makes sure only 1 sound manager is present inthe scene
        }
    }
    void Start()
    {
       
        foreach (Sounds s in sounds) //for every sound added in the inspector, this adds a few components to it like volume,pitch etc
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
        Play("BG"); //plays the background music once the game starts
    }

    public void Play(string name) //function which takes a string variable and plays the sound once
    {
        
        Sounds s =Array.Find(sounds, sounds => sounds.name == name);
        s.source.Play();
       
    }

    public void Stop(string name)//function which takes a string variable and stops the sound 
    {
        Sounds s = Array.Find(sounds, sounds => sounds.name == name);
        s.source.Stop();
    }


}
