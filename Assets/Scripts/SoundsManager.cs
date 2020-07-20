using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoSingleton<SoundsManager>
{
    public enum ClipType { Undefined, Win, GearPickUp, GearPut, GearSwap };
    [System.Serializable]
    public class Clip
    {
        public AudioClip autioClip;
        public ClipType type;
    }

    public List<Clip> clips = new List<Clip>();
    List<AudioSource> audioSources = new List<AudioSource>();

    public void Play(ClipType type)
    {
        var clip = clips.Find((Clip c) => { return c.type == type; });
        if(clip != null)
            Play(clip.autioClip);
    }

    void Play(AudioClip clip)
    {
        var source = audioSources.Find((AudioSource s) => { return !s.isPlaying; });
        if(source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            audioSources.Add(source);
        }
        source.clip = clip;
        source.Play();
    }
}
