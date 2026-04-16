using UnityEngine;

/// <summary>
/// Procedural sound effects using AudioSource.
/// Generates short synth clips at runtime (similar to ZzFX in HTML version).
/// </summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public enum Clip
    {
        Jump, Land, Charge, Echo, Crystal,
        Shockwave, Door, Die, Goal, Flip, Grade
    }

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
    }

    public void Play(Clip clip)
    {
        AudioClip ac = GenerateClip(clip);
        if (ac != null) audioSource.PlayOneShot(ac);
    }

    private AudioClip GenerateClip(Clip clip)
    {
        // Each clip: frequency, duration, type
        switch (clip)
        {
            case Clip.Jump:      return MakeTone(400f, 0.08f, WaveType.Square, 0.5f, 600f);
            case Clip.Land:      return MakeTone(150f, 0.06f, WaveType.Noise, 0.3f);
            case Clip.Charge:    return MakeTone(80f, 0.04f, WaveType.Sine, 0.15f, 200f);
            case Clip.Echo:      return MakeTone(600f, 0.15f, WaveType.Sine, 0.25f, -200f);
            case Clip.Crystal:   return MakeTone(800f, 0.2f, WaveType.Sine, 0.4f, 400f);
            case Clip.Shockwave: return MakeTone(200f, 0.25f, WaveType.Sawtooth, 0.5f, -150f);
            case Clip.Door:      return MakeTone(300f, 0.15f, WaveType.Square, 0.3f, 200f);
            case Clip.Die:       return MakeTone(100f, 0.3f, WaveType.Noise, 0.4f, -80f);
            case Clip.Goal:      return MakeTone(500f, 0.4f, WaveType.Sine, 0.5f, 300f);
            case Clip.Flip:      return MakeTone(800f, 0.04f, WaveType.Square, 0.15f);
            case Clip.Grade:     return MakeTone(600f, 0.3f, WaveType.Sine, 0.6f, 200f);
            default:             return null;
        }
    }

    private enum WaveType { Sine, Square, Sawtooth, Noise }

    private AudioClip MakeTone(float freq, float duration, WaveType type, float volume, float freqSlide = 0f)
    {
        int sampleRate = 44100;
        int samples = (int)(duration * sampleRate);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / sampleRate;
            float env = 1f - (t / duration); // linear decay
            float currentFreq = freq + freqSlide * t;
            float phase = t * currentFreq * 2f * Mathf.PI;

            float sample = 0f;
            switch (type)
            {
                case WaveType.Sine:
                    sample = Mathf.Sin(phase);
                    break;
                case WaveType.Square:
                    sample = Mathf.Sin(phase) > 0 ? 1f : -1f;
                    break;
                case WaveType.Sawtooth:
                    sample = 2f * ((t * currentFreq) % 1f) - 1f;
                    break;
                case WaveType.Noise:
                    sample = Random.Range(-1f, 1f);
                    break;
            }

            data[i] = sample * env * volume;
        }

        AudioClip clip = AudioClip.Create("sfx", samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
