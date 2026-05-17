using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private List<SoundData> soundList;
    private Dictionary<string, SoundData> soundDictionary;

    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar fuentes de audio
            sfxSource = gameObject.AddComponent<AudioSource>();

            // Llenar diccionario para búsqueda rápida
            soundDictionary = new Dictionary<string, SoundData>();
            foreach (var sound in soundList)
            {
                soundDictionary.Add(sound.soundName, sound);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string name)
    {
        if (!soundDictionary.TryGetValue(name, out SoundData sound))
        {
            Debug.LogWarning($"Sonido no encontrado: {name}");
            return;
        }

        // Aplicar variaciones si están activas
        float finalPitch = sound.pitch;
        if (sound.useRandomPitch)
        {
            finalPitch += Random.Range(-sound.pitchRange, sound.pitchRange);
        }

        // Reproducir sin interrumpir otros efectos en el mismo canal
        sfxSource.pitch = finalPitch;
        sfxSource.PlayOneShot(sound.clip, sound.volume);
    }
}