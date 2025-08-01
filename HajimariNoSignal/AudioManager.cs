using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using HajimariNoSignal;

public class AudioManager
{
    private Song _currentSong;
    private Song _previewSong;
    private Dictionary<string, SoundEffect> _sfx;
    private List<SoundEffect> _hitSounds = new();

    public AudioManager()
    {
        _sfx = new Dictionary<string, SoundEffect>();
    }

    // MUSIC
    public void PlayMusic(Song song, bool loop = true)
    {
        StopMusic();
        _currentSong = song;
        MediaPlayer.IsRepeating = loop;
        MediaPlayer.Play(_currentSong);
    }

    public void StopMusic()
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }
        _currentSong = null;
    }

    public void PlayPreview(Song preview)
    {
        StopMusic();
        _previewSong = preview;
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_previewSong);
    }

    public void StopPreview()
    {
        if (_previewSong != null)
        {
            MediaPlayer.Stop();
            _previewSong = null;
        }
    }

    // SOUND EFFECTS
    public void LoadSFX(string name, SoundEffect effect)
    {
        _sfx[name] = effect;
    }

    public void PlaySFX(string name)
    {
        if (_sfx.ContainsKey(name))
        {
            _sfx[name].Play();
        }
    }
    public void LoadHitSounds(int count)
    {
        _hitSounds.Clear();
        for (int i = 1; i <= count; i++)
        {
            var sfx = Game1.StaticContent.Load<SoundEffect>($"Audio/Hitsounds/hitsound_{i}");
            _hitSounds.Add(sfx);
        }
    }

    public void PlayRandomHitSound()
    {
        if (_hitSounds.Count > 0)
        {
            var sfx = _hitSounds[Game1.Instance._random.Next(_hitSounds.Count)];
            sfx.Play();
        }
    }

    public void StopAll()
    {
        StopMusic();
        //_sfx.Clear();
    }
}
