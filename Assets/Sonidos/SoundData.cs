using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    public string soundName;
    public AudioClip clip;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;

    public bool useRandomPitch = false;
    [Range(0.1f, 0.5f)] public float pitchRange = 0.1f;
}