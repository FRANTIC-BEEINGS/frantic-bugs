using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private GameObject _sounds;
    [SerializeField] private AudioSource _music;
    
    [SerializeField] private AudioClip _goldSound;
    public AudioClip GoldSound => _goldSound;

    private readonly List<AudioSource> _soundSources = new List<AudioSource>();
    private float _soundVolume = 2;
    public float SoundVolume {
        get => _soundVolume;
        set {
            _soundVolume = value;
            foreach (var soundSource in _soundSources) {
                soundSource.volume = _soundVolume;
            }
            
            // for settings later
            /*PlayerPrefs.SetFloat(SOUNDS, _soundVolume);
            PlayerPrefs.Save();*/
        }
    }

    private float _musicVolume;
    public float MusicVolume {
        get => _musicVolume;
        set {
            _musicVolume = value;
            _music.volume = _musicVolume;

            // for settings later
            /*PlayerPrefs.SetFloat(MUSIC, _musicVolume);
            PlayerPrefs.Save();*/
        }
    }

    public static SoundController Instance { get; private set; }

    public SoundController() {
        if (Instance != null)
            throw new ApplicationException("ONLY ONE SOUNDCONTROLLER ALLOWED");
        Instance = this;
    }

    /*private void Awake() {
        SoundVolume = PlayerPrefs.GetFloat(SOUNDS, 1f);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC, 1f);
    }*/

    public AudioSource PlaySound(AudioClip sound) {
        var source = _soundSources.FirstOrDefault(ss => !ss.isPlaying);
        if (source == null) {
            source = _sounds.AddComponent<AudioSource>();
            _soundSources.Add(source);
        }

        source.clip = sound;
        source.Play();
        source.volume = SoundVolume;
        source.pitch = 1;

        return source;
    }

    public AudioSource PlayMusic(AudioClip music) {
        _music.clip = music;
        _music.Play();
        _music.volume = MusicVolume;

        return _music;
    }
}
