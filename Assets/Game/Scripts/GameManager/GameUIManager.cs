using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : SingletonMonoBehaviour<GameUIManager> {    
	public Text score;
	public Text gold;
	public Text time;
	public Image shipImage;
	public GameObject progressBar;
	public GameObject bossHealthBar;
	public Animator bossHealthAnim;
	public Image bossHealth;
	public Text centerText;
	public Animator HudAnim;
	public Animator centerTextAnim;
	public GameObject dailyQuest;
	public Image dailyImage;
	public Text dailyText;

	public AudioClip sfx_count;
	public AudioClip sfx_start;
	public AudioClip sfxWarning;

    public BasePopup revivePop, pausePop, gameoverPop;

	public void RunCountDown () {
		StartCoroutine("CountDown");
	}

	IEnumerator CountDown () {
		centerText.text = "3";
		SoundManager.Instance.PlaySfx(sfx_count);
		yield return new WaitForSecondsRealtime(0.5f);
		centerText.text = "2";
		SoundManager.Instance.PlaySfx(sfx_count);
		yield return new WaitForSecondsRealtime(0.5f);
		centerText.text = "1";
		SoundManager.Instance.PlaySfx(sfx_count);
		yield return new WaitForSecondsRealtime(0.5f);
		centerText.text = "start";
		SoundManager.Instance.PlaySfx(sfx_start);
		yield return new WaitForSecondsRealtime(0.5f);
		centerText.text = "";
	}

	void Start () {
		GameEventManager.Instance.PlayerGetScore += HandlePlayerGetScore;
		GameEventManager.Instance.PlayerGetCoin += HandlePlayerGetCoin;
		GameEventManager.Instance.GamePhaseChanged += HandleGamePhaseChanged;
		GameEventManager.Instance.BossAppear += OnBossFinishAppear;
		GameManager.Instance.OnTimeChange += ShowTime;
		shipImage.sprite = GameManager.Instance.player1.myRender.sprite;
		
	}
    
    public void ShowRevivePopup()
    {
        revivePop.Show();
    }

    public void ShowGameOverPopup()
    {
        gameoverPop.Show();
    }


    public void ShowPausePopup()
    {
        pausePop.Show();
    }

	void HandlePlayerGetCoin (Player p, Coin c) {
		gold.text = GameManager.Instance.coin.ToString();
	}

	void OnBossFinishAppear () {
		StartCoroutine("FillBossHealth");
	}

	void HandleGamePhaseChanged (GAME_PHASE phase) {
		if (phase == GAME_PHASE.BOSS) {
			SoundManager.Instance.PauseMusic();
			StartCoroutine("BossComing");
		}
	}

	void HandlePlayerGetScore (Player p, int value) {
		score.text = GameManager.Instance.score.ToString();
	}

	void ShowTime (int time) {
		int min = time / 60;
		int sec = time % 60;
		this.time.text = string.Format("{0}:{1}", min, sec.ToString("D2"));
	}

	IEnumerator BossComing () {
		SoundManager.Instance.PlaySfx(sfxWarning);
		progressBar.SetActive(false);
		centerText.text = "Boss is coming!";
		centerTextAnim.enabled = true;
		yield return new WaitForSeconds(3);
		centerTextAnim.enabled = false;
		centerText.text = "";
	}

	IEnumerator FillBossHealth () {
		bossHealthBar.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		bossHealthAnim.enabled = false;
		bossHealth.fillAmount = 1;
		GameEventManager.Instance.OnBossFinishAppear();
		SoundManager.Instance.PlayBossMusic();
	}

	void OnDestroy () {
		
	}
}