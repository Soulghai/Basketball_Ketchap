using System;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class RingManager : MonoBehaviour {
    public static event Action OnParallaxMove;
	public GameObject[] rings;
	public GameObject ball;
    public GameObject CoinPrefab;

    private GameObject coinObject;
	GameObject _prevRing;
	[HideInInspector] public GameObject CurrentRing;
	GameObject _nextRing;

	const float START_X_POSITION = -5.6f;

    [HideInInspector] public bool WaitMoveToStartPosition;

	// Use this for initialization
	void Start () {
		DefsGame.RingManager = this;
		Init ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnEnable() {
		Ball.OnBallInBasket += Ball_OnBallInBasket;
	   // Ball.OnMiss += Ball_OnMiss;
	    Ball.OnCoinSensor += Ball_OnCoinSensor;
	}

	void OnDisable() {
		Ball.OnBallInBasket -= Ball_OnBallInBasket;
	    //Ball.OnMiss -= Ball_OnMiss;
	    Ball.OnCoinSensor -= Ball_OnCoinSensor;
	}

    private void Ball_OnMiss(float value)
    {

    }

    private void Ball_OnCoinSensor()
    {
        AddTenPoints();
    }

    private void Ball_OnBallInBasket()
	{
	    MoveCurrentBasket();
		_prevRing = _nextRing;

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
        CurrentRing = null;
        _nextRing = null;
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
        Tweener t = _prevRing.transform.DOMove (new Vector3 (START_X_POSITION, -2f, 1f), 0.5f);
        t.SetEase (Ease.InCubic);

        Vector3 newPoint = ChooseNextPoint ();

        t = _nextRing.transform.DOMove (newPoint, 0.5f);
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
        Tweener t = _prevRing.transform.DOMove (new Vector3 (START_X_POSITION - 5f, _prevRing.transform.position.y, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        ringObject.Remove ();

        t = _nextRing.transform.DOMove (new Vector3 (START_X_POSITION, _nextRing.transform.position.y, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        t.OnComplete (() => {
            RingHead ringHead = _prevRing.GetComponentInChildren<RingHead>();
            ringHead.Show(false);

            ShieldVisual shieldVisual = _prevRing.GetComponentInChildren<ShieldVisual>();
            shieldVisual.Hide();
        });

        GameEvents.Send(OnParallaxMove);
    }

    private void RespownBall() {
		Ball ballScript = ball.GetComponent<Ball> ();
		ballScript.Respown (_prevRing.transform.position);
	}

    private void CreateFirstRing() {
		_prevRing = GetInactveRing ();
		_prevRing.transform.position = new Vector3 (START_X_POSITION - 5f, -2f, 1f);
		Tweener t = _prevRing.transform.DOMove (new Vector3 (START_X_POSITION, -2f, 1f), 0.5f);
		t.SetEase (Ease.InCubic);
		RingHead ringHead = _prevRing.GetComponentInChildren<RingHead> ();
		ringHead.Show (false);
        ShieldVisual shieldVisual = _prevRing.GetComponentInChildren<ShieldVisual>();
        shieldVisual.Hide();
	}

    private void CreateNewRing() {
		Vector3 newPoint = ChooseNextPoint ();
		_nextRing = GetInactveRing ();
		_nextRing.transform.position = new Vector3(newPoint.x + 10f, newPoint.y, newPoint.z);
		RingHead ringHead = _nextRing.GetComponentInChildren<RingHead> (true);
		ringHead.Show (true);
        ShieldVisual shieldVisual = _nextRing.GetComponentInChildren<ShieldVisual>(true);
        shieldVisual.Show();

		Tweener t = _nextRing.transform.DOMove (newPoint, 0.5f);
		t.SetEase (Ease.InCubic);
		t.OnComplete (() => {
			RespownBall();
		    if ((DefsGame.QUEST_THROW_Counter == 2) || ((DefsGame.QUEST_THROW_Counter > 5) && (DefsGame.QUEST_THROW_Counter + 2) % 5 == 0))
		    {
		        RingObject ro = _nextRing.GetComponent<RingObject>();
		        DefsGame.CoinSensor.gameObject.transform.position = ro.PointsPlace.transform.position;
		        DefsGame.CoinSensor.Init ();
		        DefsGame.CoinSensor.Show (true);
		    }
		});

	    CurrentRing = _nextRing;
	}

    private Vector3 ChooseNextPoint() {
        float posX = 0;

        if ((DefsGame.ScreenGame.isGameOver)||(DefsGame.currentPointsCount < 5)) posX = Random.Range(0f, 1.5f); else
        if (DefsGame.currentPointsCount < 10) posX = Random.Range(0.5f, 2.5f); else
        if (DefsGame.currentPointsCount < 20) posX = Random.Range(1.0f, 3.5f); else
        if (DefsGame.currentPointsCount < 30) posX = Random.Range(1.5f, 4.5f); else
        if (DefsGame.currentPointsCount < 40) posX = Random.Range(2f, 5.5f); else
        if (DefsGame.currentPointsCount < 50) posX = Random.Range(2.5f, 6.5f);
        else
            posX = Random.Range(3f, 7.4f);
		 
		return new Vector3(posX, Random.Range(-ScreenSettings.ScreenHeight*0.35f, 0.0f), 1f);
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
