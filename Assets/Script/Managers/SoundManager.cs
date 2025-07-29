using UnityEngine;

namespace CardMatch
{
    // Manages all audio playback 
    public class SoundManager : Singleton<SoundManager>, IManager
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Card Sounds")]
        [SerializeField] private AudioClip cardFlipSound;
        [SerializeField] private AudioClip cardMatchSound;
        [SerializeField] private AudioClip cardMismatchSound;
        
        [Header("Game Event Sounds")]
        [SerializeField] private AudioClip gameWinSound;
        [SerializeField] private AudioClip gameOverSound;
        
        [Header("Audio Settings")]
        [SerializeField] private float masterVolume = 1f;
        [SerializeField] private float sfxVolume = 1f;
        
        [SerializeField] private bool isMuted = false;
        
        #region IManager Implementation
        
        public bool IsInitialized { get; private set; }
        
        public void Initialize()
        {
            if (IsInitialized) return;
            
            SetupAudioSources();
            LoadAudioSettings();
            IsInitialized = true;
            
            Debug.Log($"SoundManager: Initialized successfully");
        }
        
        public void Cleanup()
        {
            if (!IsInitialized) return;
            
            SaveAudioSettings();
            StopAllAudio();
            IsInitialized = false;
            
            Debug.Log($"SoundManager: Cleaned up successfully");
        }
        
        #endregion
        
        #region Audio Source Setup
        
        private void SetupAudioSources()
        {
            // Create SFX source if not assigned
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            // Apply initial volume settings
            UpdateVolumeSettings();
        }
        
        #endregion
        
        #region Card Sounds
        
        // Plays the card flip sound effect
        public void PlayCardFlip()
        {
            PlaySFX(cardFlipSound);
        }
        
        // Plays the card match sound effect
        public void PlayCardMatch()
        {
            PlaySFX(cardMatchSound);
        }
        
        // Plays the card mismatch sound effect
        public void PlayCardMismatch()
        {
            PlaySFX(cardMismatchSound);
        }
        
        #endregion
        
        #region Game Event Sounds
        
        // Plays the game win sound effect
        public void PlayGameWin()
        {
            PlaySFX(gameWinSound);
        }
        
        // Plays the game over sound effect
        public void PlayGameOver()
        {
            PlaySFX(gameOverSound);
        }
        
        #endregion
        
        #region Audio Control
        
        // Plays a sound effect with current SFX volume
        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null && !isMuted)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }
        
        // Stops all audio playback
        public void StopAllAudio()
        {
            if (sfxSource != null)
            {
                sfxSource.Stop();
            }
        }
        
        // Toggles mute state
        public void ToggleMute()
        {
            isMuted = !isMuted;
            UpdateVolumeSettings();
            SaveAudioSettings();
        }
        
        // Sets master volume (0-1)
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumeSettings();
            SaveAudioSettings();
        }
        
        // Sets SFX volume (0-1)
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumeSettings();
            SaveAudioSettings();
        }
        
        // Updates all audio source volumes based on current settings
        private void UpdateVolumeSettings()
        {
            if (sfxSource != null)
            {
                sfxSource.volume = isMuted ? 0f : sfxVolume * masterVolume;
            }
        }
        
        #endregion
        
        #region Settings Persistence
        
        // Loads audio settings from PlayerPrefs
        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        }
        
        // Saves audio settings to PlayerPrefs
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        #endregion
        
        #region Public Getters
        
        public float MasterVolume => masterVolume;
        public float SFXVolume => sfxVolume;
        public bool IsMuted => isMuted;
        
        #endregion
    }
} 