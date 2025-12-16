using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using YG;

[Serializable, CreateAssetMenu(menuName = "Scriptables/SoundHolder")]
public class SoundHolder : DataHolder
{
    #region Singletone
    private static SoundHolder _default;
    public static SoundHolder Default => _default;
    #endregion

    [SerializeField] AudioSource audioSource;
    public static AudioSource AudioSource { get { return _default.audioSource; } }

    [SerializeField] AudioMixer mainMixer;

    [Serializable]
    public class SoundOption
    {
        public AudioClip clip;
        public float volume = 1f;
    }
    [Serializable]
    public class SoundPack
    {
        public string key = "";
        public List<SoundOption> sounds;
        public float addPitch;
    }

    public List<SoundPack> soundPacks;

    Dictionary<int, float> addPitch;
    Dictionary<int, Tween> addPitchTween;
    Tween hitTween;


    public override void Init()
    {
        _default = this;
        addPitch = new();
        addPitchTween = new();

        SetMusic(YandexGame.savesData.Music);
        SetSFX(YandexGame.savesData.SFX);
    }

    public void SetMusic(bool music) => mainMixer.SetFloat("Music Volume", music ? 0 : -80);
    public void SetSFX(bool sfx) => mainMixer.SetFloat("SFX Volume", sfx ? 0 : -80);

    public SoundPack GetSoundPack(string packName)
    {
        return soundPacks.Find((p) => p.key == packName);
    }

    public void PlayHit()
    {
        if (hitTween.IsActive()) return;
        hitTween = DOTween.Sequence().SetDelay(0.1f).OnComplete(() => {});
        PlayFromSoundPack("Hit");
    }

    /// <summary>
    /// Спавнит GameObject c AudioSource, помещает в него рандомный клип из набора с соответствующим названем, задает настройки громкости.
    /// Объект автоматически удаляется после проигрывания клипа.
    /// </summary>
    public AudioSource PlayFromSoundPack(string packName, Transform parent = null, bool isLoop = false)
    {
        int index = soundPacks.FindIndex((p) => p.key == packName);
        SoundPack pack = soundPacks[index];

        if (pack == null || pack.sounds.Count <= 0) return null;
        SoundOption sound = pack.sounds[UnityEngine.Random.Range(0, pack.sounds.Count)];
        if (!sound.clip) return null;

        float pitch = 1;
        if (pack.addPitch > 0)
        {
            if (addPitch.TryGetValue(index, out float value)) addPitch[index] += pack.addPitch;
            else addPitch[index] = 0;
            pitch += addPitch[index];

            if (pitch > 0)
            {
                if (addPitchTween.TryGetValue(index, out Tween tween)) tween.Kill();
                addPitchTween[index] = DOTween.Sequence().SetDelay(0.33f).OnComplete(() => addPitch.Remove(index));
            }
        }
        else pitch += UnityEngine.Random.Range(-0.1f, 0.1f);

        return SpawnSoundSource(sound.clip, parent, sound.volume, isLoop, pitch);
    }

    /// <summary>
    /// Спавнит GameObject c AudioSource, помещает в него рандомный клип.
    /// Объект автоматически удаляется после проигрывания клипа.
    /// </summary>
    public void PlayClip(AudioClip clip)
    {
        SpawnSoundSource(clip);
    }

    /// <summary>
    /// Возвращает рандомный звук с настройками громкости из соответствующего набора.
    /// </summary>
    public SoundOption GetRandomSound(string key)
    {
        SoundPack pack = soundPacks.Find((p) => p.key == key);
        if (pack == null || pack.sounds.Count <= 0) return null;
        return pack.sounds[UnityEngine.Random.Range(0, pack.sounds.Count)];
    }

    private AudioSource SpawnSoundSource(AudioClip clip, Transform parent = null, float volume = 1f, bool isLoop = false, float pitch = 1)
    {
        AudioSource source = new GameObject("AudioPlayer").AddComponent<AudioSource>();
        source.transform.SetParent(parent ? parent : Camera.main.transform);
        source.transform.localPosition = Vector3.zero;
        source.clip = clip;
        source.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        source.pitch = pitch;
        source.volume = volume;
        source.loop = isLoop;
        if (parent)
        {
            source.spatialBlend = 1;
            source.minDistance = audioSource.minDistance;
            source.maxDistance = audioSource.maxDistance;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
        }
        source.Play();
        if (!isLoop) Destroy(source.gameObject, clip.length + 2);
        return source;
    }
}