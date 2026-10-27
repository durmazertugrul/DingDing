using System.Collections.Generic;
using UnityEngine;

// One manager for the whole game. It survives scene changes, so the same
// pool of sources serves the menu, the rankings and the match.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public GameSounds sounds;
    public int sourceCount = 12;
    public int maxCopiesOfOneClip = 3;
    public float pitchVariation = 0.05f;
    public float masterVolume = 0.3f;

    private List<AudioSource> sources = new List<AudioSource>();

    private void Awake()
    {
        // Every scene carries one of these; the first one wins and the
        // later copies remove themselves.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Only a root object can survive a scene change.
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        if (sources.Count > 0) return;

        for (int index = 0; index < sourceCount; index++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            sources.Add(source);
        }
    }

    public void Play(AudioClip clip)
    {
        Play(clip, 1f);
    }

    public void Play(AudioClip clip, float volume)
    {
        if (clip == null) return;
        if (sources == null || sources.Count == 0) return;

        // A cascade can fire the same clip dozens of times in one frame;
        // past a few copies it only turns into noise.
        int playingCopies = 0;
        AudioSource freeSource = null;

        foreach (var source in sources)
        {
            if (source == null) continue;

            if (source.isPlaying && source.clip == clip) playingCopies++;
            else if (!source.isPlaying && freeSource == null) freeSource = source;
        }

        if (playingCopies >= maxCopiesOfOneClip) return;
        if (freeSource == null) return;

        freeSource.clip = clip;
        freeSource.volume = volume * masterVolume;
        freeSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        freeSource.Play();
    }

    // The pop climbs through the cascade; after the last clip it stays
    // at the top.
    public void PlayMatchPop(int stepIndex)
    {
        if (sounds == null || sounds.matchPops == null || sounds.matchPops.Length == 0) return;

        int clipIndex = Mathf.Min(stepIndex, sounds.matchPops.Length - 1);
        Play(sounds.matchPops[clipIndex]);
    }
}