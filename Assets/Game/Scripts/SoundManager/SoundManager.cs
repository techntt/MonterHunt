using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

	public AudioMixer master;
	public AudioMixerGroup sfxGroup;
	public AudioSource music;
	public SortedList<int, AudioSource> playingSound;
	public SortedList<int, LoopSoundRequest> loopSound;

	public AudioClip[] gameMusic;
	public AudioClip[] bossMusic;

	public AudioClip btnClickSound;

	void Awake () {
		if (Instance != this)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);
	}

	void Start () {
		playingSound = new SortedList<int, AudioSource>();
		loopSound = new SortedList<int, LoopSoundRequest>();
		if (PlayerSettingData.Instance.isMusic)
			MusicOn();
		else
			MusicOff();
		if (PlayerSettingData.Instance.isSound)
			SoundOn();
		else
			SoundOff();
	}

	public void PlaySfx (AudioClip sfx) {
		PlaySfxRewind(sfx);
	}

	public void PlaySfx (AudioClip sfx, SFX_PLAY_STYLE style) {
		switch (style) {
			case SFX_PLAY_STYLE.REWIND:
				PlaySfxRewind(sfx);
				break;
			case SFX_PLAY_STYLE.DONT_REWIND:
				PlaySfxNoRewind(sfx);
				break;
			case SFX_PLAY_STYLE.OVERRIDE:
				PlaySfxOverride(sfx);
				break;
		}
	}

	public void PlaySfxRewind (AudioClip sfx) {
		int id = sfx.GetInstanceID();
		if (playingSound.ContainsKey(id)) {
			playingSound[id].Stop();
			playingSound[id].Play();
		} else {
			AddAudioSource(sfx).Play();
		}
	}

	public void PlaySfxNoRewind (AudioClip sfx) {
		int id = sfx.GetInstanceID();
		if (playingSound.ContainsKey(id)) {
			if (!playingSound[id].isPlaying)
				playingSound[id].Play();
		} else {
			AddAudioSource(sfx).Play();
		}
	}

	public void PlaySfxOverride (AudioClip sfx) {
		int id = sfx.GetInstanceID();
		if (playingSound.ContainsKey(id)) {
			playingSound[id].PlayOneShot(sfx);
		} else {
			AddAudioSource(sfx).Play();
		}
	} 

	public void PlaySfxLoop (AudioClip sfx, int requesterId) {
		int id = sfx.GetInstanceID();
		if (loopSound.ContainsKey(id)) {
			loopSound[id].requester = requesterId;
			if (!loopSound[id].source.isPlaying)
				loopSound[id].source.Play();
		} else {
			AddLoopSoundRequest(sfx, requesterId).source.Play();
		}
	}

	public AudioSource AddAudioSource (AudioClip sfx) {
		AudioSource ac = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		ac.outputAudioMixerGroup = sfxGroup;
		ac.loop = false;
		ac.playOnAwake = false;
		ac.clip = sfx;
		playingSound.Add(sfx.GetInstanceID(), ac);
		return ac;
	}

	public LoopSoundRequest AddLoopSoundRequest (AudioClip sfx, int requesterId) {
		AudioSource ac = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		ac.outputAudioMixerGroup = sfxGroup;
		ac.loop = true;
		ac.playOnAwake = true;
		ac.clip = sfx;
		LoopSoundRequest r = new LoopSoundRequest();
		r.source = ac;
		r.requester = requesterId;
		loopSound.Add(sfx.GetInstanceID(), r);
		return r;
	}

	public bool StopLoopSound (AudioClip sfx, int requesterId) {
		int id = sfx.GetInstanceID();
		if (loopSound.ContainsKey(id)) {
			LoopSoundRequest lsr = loopSound[id];
			if (lsr.requester == requesterId) {
				lsr.source.Stop();
				return true;
			} else
				return false;
		} else
			return false;
	}
	public void PlayBossMusic () {
		music.clip = bossMusic[Random.Range(0, bossMusic.Length)];
		music.Play();
	}

	public void PlayGameMusic () {
		music.clip = gameMusic[Random.Range(0, gameMusic.Length)];
		music.Play();
	}

	public void PauseMusic () {
		music.Pause();
	}

	public void MusicOn () {
		master.SetFloat("music", 0);
	}

	public void MusicOff () {
		master.SetFloat("music", -80);
	}

	public void SoundOn () {
		master.SetFloat("sfx", 0);
	}

	public void SoundOff () {
		master.SetFloat("sfx", -80);
	}

	public void PlayUIButtonClick () {
		PlaySfxOverride(btnClickSound);
	}
}

public class LoopSoundRequest {
	public AudioSource source;
	public int requester;
}
/// <summary>
/// the way the sound fx is played
/// </summary>
public enum SFX_PLAY_STYLE {
	/// <summary>
	/// restart the sound
	/// </summary>
	REWIND,
	/// <summary>
	/// if the same sound is playing, dont play this one
	/// </summary>
	DONT_REWIND,
	/// <summary>
	/// always play the sound
	/// </summary>
	OVERRIDE,
	NONE
}