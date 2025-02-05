using System;
using System.Collections.Generic;
using UnityEngine;
using GMSpace;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Parameters")]
    [SerializeField] private AUDIO_PROPERTIES _noCategory = new AUDIO_PROPERTIES();
    [SerializeField] private AUDIO_PROPERTIES _ambiance = new AUDIO_PROPERTIES();
    [SerializeField] private AUDIO_PROPERTIES _music = new AUDIO_PROPERTIES();
    [SerializeField] private AUDIO_PROPERTIES _SFX = new AUDIO_PROPERTIES();
    [SerializeField] private AUDIO_PROPERTIES _foley = new AUDIO_PROPERTIES();
    [SerializeField] private AUDIO_PROPERTIES _voices = new AUDIO_PROPERTIES();

    [Serializable]
    public enum AUDIO_CATEGORY
    {
        NONE = 0,
        AMBIANCE = 10,
        MUSIC = 11,
        SFX = 12,
        FOLEY = 13,
        VOICES = 14
    }
    [Serializable]
    public struct AUDIO_PROPERTIES
    {
        [Range(0, 1)] public float volume;
        [Range(-3, 3)] public float pitch;
        [Range(-1, 1)] public float stereoPan;
        [Range(0, 1)] public float spacialBlend;
        [Range(0, 1.1f)] public float reverbZoneMix;

        public AUDIO_PROPERTIES(bool value = true)
        {
            volume = 1.0f;
            pitch = 1.0f;
            stereoPan = 0.0f;
            spacialBlend = 0.0f;
            reverbZoneMix = 1.0f;
        }
    }

    [Header("Audio List")]
    [SerializeField] private List<AudioClip> _audioList;

    [Header("Audio Sources")]
    [SerializeField] private GameObject audioChild;
    private bool _isChildAtCamera = false;
    public bool GetIsChildAtCamera { get => _isChildAtCamera; }

    [SerializeField] private AudioSource _ambianceAudioSource;
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _voiceIAAudioSource;
    [SerializeField] private List<AudioSource> _anyAudioSource = new List<AudioSource>();

    public AudioClip GetAudioClip(string audioName)
    {
        foreach (AudioClip audio in _audioList)
        {
            if (audioName == audio.name) return audio;
        }

        return null;
    }
    public AudioClip GetAudioClip(List<AudioClip> localAudioList, string audioName)
    {
        foreach (AudioClip audio in localAudioList)
        {
            if (audioName == audio.name) return audio;
        }

        return null;
    }
    public AudioClip GetAudioClip(AudioClip[] localAudioList, string audioName)
    {
        foreach (AudioClip audio in localAudioList)
        {
            if (audioName == audio.name) return audio;
        }

        return null;
    }

    public AUDIO_PROPERTIES GetAudioProperties(AUDIO_CATEGORY category)
    {
        switch (category)
        {
            case AUDIO_CATEGORY.AMBIANCE:
                return _ambiance;
            case AUDIO_CATEGORY.MUSIC:
                return _music;
            case AUDIO_CATEGORY.SFX:
                return _SFX;
            case AUDIO_CATEGORY.FOLEY:
                return _foley;
            case AUDIO_CATEGORY.VOICES:
                return _voices;

            case AUDIO_CATEGORY.NONE:
            default:
                return _noCategory;
        }
    }

    private void Start()
    {
        AUDIO_PROPERTIES properties;

        _ambianceAudioSource.loop = true;
        properties = GetAudioProperties(AUDIO_CATEGORY.AMBIANCE);
        _ambianceAudioSource.volume = properties.volume;
        _ambianceAudioSource.pitch = properties.pitch;
        _ambianceAudioSource.panStereo = properties.stereoPan;
        _ambianceAudioSource.spatialBlend = properties.spacialBlend;
        _ambianceAudioSource.reverbZoneMix = properties.reverbZoneMix;

        _musicAudioSource.loop = true;
        properties = GetAudioProperties(AUDIO_CATEGORY.MUSIC);
        _musicAudioSource.volume = properties.volume;
        _musicAudioSource.pitch = properties.pitch;
        _musicAudioSource.panStereo = properties.stereoPan;
        _musicAudioSource.spatialBlend = properties.spacialBlend;
        _musicAudioSource.reverbZoneMix = properties.reverbZoneMix;

        _voiceIAAudioSource.loop = true;
        properties = GetAudioProperties(AUDIO_CATEGORY.VOICES);
        _voiceIAAudioSource.volume = properties.volume;
        _voiceIAAudioSource.pitch = properties.pitch;
        _voiceIAAudioSource.panStereo = properties.stereoPan;
        _voiceIAAudioSource.spatialBlend = properties.spacialBlend;
        _voiceIAAudioSource.reverbZoneMix = properties.reverbZoneMix;
    }

    public void SetParentToCamera()
    {
        audioChild.transform.SetParent(Camera.main.transform);
        audioChild.transform.localPosition = Vector3.zero;

        _isChildAtCamera = true;
    }
    public void SetParentToGameManager()
    {
        audioChild.transform.SetParent(GameManager.Instance.transform);
        audioChild.transform.localPosition = Vector3.zero;

        _isChildAtCamera = false;
    }

    private void FixedUpdate()
    {
        for (int i = 0;  i < _anyAudioSource.Count; i++)
        {
            if (!_anyAudioSource[i].isPlaying && !_anyAudioSource[i].loop)
            {
                Destroy(_anyAudioSource[i]);
                _anyAudioSource.RemoveAt(i);
            }
        }
    }

    public bool PlayAudio(AudioClip clip, AUDIO_CATEGORY configuration = AUDIO_CATEGORY.NONE, float randomPitch = 0f)
    {
        if (_audioList.Count < 29 && clip != null) // 32 sounds limit - 3 permanent sounds
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            audio.clip = clip;
            audio.loop = false;

            AUDIO_PROPERTIES properties = GetAudioProperties(configuration);
            audio.volume = properties.volume;
            audio.pitch = UnityEngine.Random.Range(properties.pitch - randomPitch, properties.pitch + randomPitch);
            audio.panStereo = properties.stereoPan;
            audio.spatialBlend = properties.spacialBlend;
            audio.reverbZoneMix = properties.reverbZoneMix;

            audio.Play();

            _anyAudioSource.Add(audio);
            return true;
        }
        else return false;
    }
    public bool PlayAudio(string audioName, AUDIO_CATEGORY configuration = AUDIO_CATEGORY.NONE, float randomPitch = 0f)
    {
        if (_audioList.Count < 29) // 32 sounds limit - 3 permanent sounds
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            AudioClip clip = GetAudioClip(audioName);
            if (clip == null) return false;

            audio.clip = clip;
            audio.loop = false;

            AUDIO_PROPERTIES properties = GetAudioProperties(configuration);
            audio.volume = properties.volume;
            audio.pitch = UnityEngine.Random.Range(properties.pitch - randomPitch, properties.pitch + randomPitch);
            audio.panStereo = properties.stereoPan;
            audio.spatialBlend = properties.spacialBlend;
            audio.reverbZoneMix = properties.reverbZoneMix;

            audio.Play();


            _anyAudioSource.Add(audio);
            return true;
        }
        else return false;
    }

    public AudioSource PlayAudioLoop(AudioClip clip, AUDIO_CATEGORY configuration = AUDIO_CATEGORY.NONE)
    {
        if (_audioList.Count < 29 && clip != null) // 32 sounds limit - 3 permanent sounds
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();

            audio.clip = clip;
            audio.loop = true;

            AUDIO_PROPERTIES properties = GetAudioProperties(configuration);
            audio.volume = properties.volume;
            audio.pitch = properties.pitch;
            audio.panStereo = properties.stereoPan;
            audio.spatialBlend = properties.spacialBlend;
            audio.reverbZoneMix = properties.reverbZoneMix;

            audio.Play();


            _anyAudioSource.Add(audio);
            return _anyAudioSource[_anyAudioSource.IndexOf(audio)];
        }
        else return null;
    }
    public AudioSource PlayAudioLoop(string audioName, AUDIO_CATEGORY configuration = AUDIO_CATEGORY.NONE)
    {
        if (_audioList.Count < 29) // 32 sounds limit - 3 permanent sounds
        {
            AudioSource audio = gameObject.AddComponent<AudioSource>();
            AudioClip clip = GetAudioClip(audioName);
            if (clip == null) return null;

            audio.clip = clip;
            audio.loop = true;

            AUDIO_PROPERTIES properties = GetAudioProperties(configuration);
            audio.volume = properties.volume;
            audio.pitch = properties.pitch;
            audio.panStereo = properties.stereoPan;
            audio.spatialBlend = properties.spacialBlend;
            audio.reverbZoneMix = properties.reverbZoneMix;

            audio.Play();


            _anyAudioSource.Add(audio);
            return _anyAudioSource[_anyAudioSource.IndexOf(audio)];
        }
        else return null;
    }
    public void StopAudioLoop(ref AudioSource source)
    {
        int index = _anyAudioSource.IndexOf(source);
        if (index >= 0 && _anyAudioSource[index].loop == true)
        {
            _anyAudioSource[index].Stop();
            Destroy(_anyAudioSource[index]);
            _anyAudioSource.RemoveAt(index);
        }

        source = null;
    }
    public void ChangeAudioLoopVolume(AudioSource source, float volume)
    {
        int index = _anyAudioSource.IndexOf(source);
        if (index >= 0)
        {
            _anyAudioSource[index].volume = volume;
        }
    }
    public void ChangeAudioLoopPitch(AudioSource source, float pitch)
    {
        int index = _anyAudioSource.IndexOf(source);
        if (index >= 0)
        {
            _anyAudioSource[index].pitch = pitch;
        }
    }
    public void ChangeAudioLoopBalance(AudioSource source, float balance)
    {
        int index = _anyAudioSource.IndexOf(source);
        if (index >= 0)
        {
            _anyAudioSource[index].panStereo = balance;
        }
    }

    public void PlayAmbianceSound(AudioClip audioClip)
    {
        _ambianceAudioSource.Stop();
        _ambianceAudioSource.clip = audioClip;
        _ambianceAudioSource.Play();
    }
    public void PlayAmbianceSound(string audioName)
    {
        AudioClip clip = GetAudioClip(audioName);
        if (clip == null) return;

        _ambianceAudioSource.Stop();
        _ambianceAudioSource.clip = clip;
        _ambianceAudioSource.Play();
    }
    public void PauseAmbianceSound()
    {
        if (_ambianceAudioSource.isPlaying) _ambianceAudioSource.Pause();
        else _ambianceAudioSource.UnPause();
    }
    public void PauseAmbianceSound(bool pause)
    {
        if (pause) _ambianceAudioSource.Pause();
        else _ambianceAudioSource.UnPause();
    }

    public void PlayMusicSound(AudioClip audioClip)
    {
        _musicAudioSource.Stop();
        _musicAudioSource.clip = audioClip;
        _musicAudioSource.Play();
    }
    public void PlayMusicSound(string audioName)
    {
        AudioClip clip = GetAudioClip(audioName);
        if (clip == null) return;

        _musicAudioSource.Stop();
        _musicAudioSource.clip = clip;
        _musicAudioSource.Play();
    }
    public void PauseMusicSound()
    {
        if (_musicAudioSource.isPlaying) _musicAudioSource.Pause();
        else _musicAudioSource.UnPause();
    }
    public void PauseMusicSound(bool pause)
    {
        if (pause) _musicAudioSource.Pause();
        else _musicAudioSource.UnPause();
    }

    public void PlayVoiceSound(AudioClip audioClip)
    {
        _voiceIAAudioSource.Stop();
        _voiceIAAudioSource.clip = audioClip;
        _voiceIAAudioSource.Play();
    }
    public void PlayVoiceSound(string audioName)
    {
        AudioClip clip = GetAudioClip(audioName);
        if (clip == null) return;

        _voiceIAAudioSource.Stop();
        _voiceIAAudioSource.clip = clip;
        _voiceIAAudioSource.Play();
    }
}
