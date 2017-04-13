using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class RingManager : MonoBehaviour {
    public static event Action OnParallaxMove;
	public GameObject[] rings;
	public GameObject ball;
    public GameObject CoinPrefab;

    private GameObject _coinObject;
    private GameObject _prevRing;
	[HideInInspector] public GameObject NextRing;

	private const float StartXPosition = -5.1f;

    [HideInInspector] public bool WaitMoveToStartPosition;

    private AudioClip[] _audioClips = new AudioClip[10];
    private int _applauseID;

    // Use this for initialization
	void Start () {
		DefsGame.RingManager = this;

	    for (int i = 0; i < 10; i++)
	    {
	        _audioClips[i] = Resources.Load<AudioClip>("snd/Applause/applause" + (i+1).ToString());
	    }
	    _applauseID = -1;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnEnable() {
	    Ball.OnGoal += Ball_OnGoal;
		Ball.OnBallInBasket += Ball_OnBallInBasket;
	    Ball.OnCoinSensor += Ball_OnCoinSensor;
	}

	void OnDisable() {
	    Ball.OnGoal -= Ball_OnGoal;
		Ball.OnBallInBasket -= Ball_OnBallInBasket;
	    Ball.OnCoinSensor -= Ball_OnCoinSensor;
	}

    private void Ball_OnCoinSensor()
    {
        AddTenPoints();
    }

    private void Ball_OnGoal(int pointsCount)
    {
        Invoke("MoveToSky", 0.7f);
        if (pointsCount > 3)
        {
            if (_applauseID < _audioClips.Length - 1) ++_applauseID;
            else _applauseID = 0;
            Defs.PlaySound(_audioClips[_applauseID]);
        }
    }

    private void MoveToSky()
    {
        RingHead ringHead = NextRing.GetComponentInChildren<RingHead>();
        ringHead.MoveToSky();
    }

    private void Ball_OnBallInBasket()
	{
	    MoveCurrentBasket();
		_prevRing = NextRing;

		CreateNewRing ();
	}

    public void Miss()
    {
        if (DefsGame.currentPointsCount != 0) MoveToStartPosition();
        else
        {
            RespownBall();
        }
        DefsGame.CoinSensor.Hide();
    }

    private void Init()
    {
        WaitMoveToStartPosition = false;
        _prevRing = null;
        NextRing = null;
        CreateFirstRing ();
        CreateNewRing ();
    }


    private void AddTenPoints()
    {
        for (int i = 0; i < 10; i++)
        {//Camera.main.ScreenToWorldPoint(
            GameObject coin = (GameObject) Instantiate(CoinPrefab, DefsGame.CoinSensor.gameObject.transform.position,
                Quaternion.identity);
            Coin coinScript = coin.GetComponent<Coin>();
            coinScript.MoveToEnd();
        }
        DefsGame.CoinSensor.Hide();
    }

    private void MoveToStartPosition()
    {
        Tweener t = _prevRing.transform.DOMove (new Vector3 (StartXPosition, -2f, 1f), 0.5f);
        t.SetEase (Ease.InCubic);

        Vector3 newPoint = ChooseNextPoint ();

        t = NextRing.transform.DOMove (newPoint, 0.5f);
        t.SetEase (Ease.InCubic);
        WaitMoveToStartPosition = true;
        t.OnComplete (() => {
            DefsGame.CoinSensor.Hide();
            RespownBall();
            WaitMoveToStartPosition = false;
        });
    }


    private void MoveCurrentBasket()
    {
        RingObject ringObject = _prevRing.GetComponent<RingObject> ();
        Tweener t = _prevRing.transform.DOMove (new Vector3 (StartXPosition - 5f, _prevRing.transform.position.y, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        ringObject.Remove ();

        t = NextRing.transform.DOMove (new Vector3 (StartXPosition, NextRing.transform.position.y, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        t.OnComplete (() => {
            RingHead ringHead = _prevRing.GetComponentInChildren<RingHead>();
            ringHead.Hide();

			//ringObject = _prevRing.GetComponent<RingObject> ();
			//ringObject.ShieldVisual.Hide();
        });

        GameEvents.Send(OnParallaxMove);
    }

    private void RespownBall() {
		Ball ballScript = ball.GetComponent<Ball> ();
		ballScript.Respown (_prevRing.transform.position);
	}

    private void CreateFirstRing() {
		_prevRing = GetInactveRing ();
		_prevRing.transform.position = new Vector3 (StartXPosition - 5f, -2f, 1f);
		Tweener t = _prevRing.transform.DOMove (new Vector3 (StartXPosition, -2f, 1f), 0.5f);
		t.SetEase (Ease.InCubic);
		RingHead ringHead = _prevRing.GetComponentInChildren<RingHead> ();
		ringHead.Hide();
		//RingObject ringObject = _prevRing.GetComponent<RingObject> ();
		//ringObject.ShieldVisual.Hide();
	}

    private void CreateNewRing() {
		Vector3 newPoint = ChooseNextPoint ();
		NextRing = GetInactveRing ();
		NextRing.transform.position = new Vector3(newPoint.x + 10f, newPoint.y, newPoint.z);
		RingHead ringHead = NextRing.GetComponentInChildren<RingHead> (true);
		ringHead.Show ();
		//RingObject ringObject = _nextRing.GetComponent<RingObject> ();
		//ringObject.ShieldVisual.Hide();

		Tweener t = NextRing.transform.DOMove (newPoint, 0.5f);
		t.SetEase (Ease.InCubic);
		t.OnComplete (() => {
			RespownBall();
		    if ((DefsGame.QUEST_THROW_Counter == 2) || ((DefsGame.QUEST_THROW_Counter > 5) && (DefsGame.QUEST_THROW_Counter + 2) % 5 == 0))
		    {
		        RingObject ro = NextRing.GetComponent<RingObject>();
		        DefsGame.CoinSensor.gameObject.transform.position = ro.PointsPlace.transform.position;
		        DefsGame.CoinSensor.Init ();
		        DefsGame.CoinSensor.Show (true);
		    }
		});
	}

    private Vector3 ChooseNextPoint() {
        float posX = 0;

        if ((DefsGame.ScreenGame.IsGameOver)||(DefsGame.currentPointsCount < 5)) posX = Random.Range(0f, 1.5f); else
        if (DefsGame.currentPointsCount < 10) posX = Random.Range(0.5f, 2.5f); else
        if (DefsGame.currentPointsCount < 20) posX = Random.Range(1.0f, 3.5f); else
        if (DefsGame.currentPointsCount < 30) posX = Random.Range(1.5f, 4.5f); else
        if (DefsGame.currentPointsCount < 40) posX = Random.Range(2f, 5.5f); else
        if (DefsGame.currentPointsCount < 50) posX = Random.Range(2.5f, 6.5f);
        else
            posX = Random.Range(3f, 7.4f);
		 
		return new Vector3(posX, Random.Range(-ScreenSettings.ScreenHeight*0.30f, 0.0f), 1f);
	}

    private GameObject GetInactveRing() {
		GameObject ring;
		if (!rings [0].activeSelf)
			ring = rings [0];
		else if (!rings [1].activeSelf)
			ring = rings [1];
		else if (!rings [2].activeSelf)
			ring = rings [2];
		else
			ring = rings [3];

		ring.SetActive (true);

		return ring;
	}
}
