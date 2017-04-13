using UnityEngine;
using DG.Tweening;

public class RingHead : MonoBehaviour {
	public  Animator ShieldAnimator;

    private Vector3 _startPosition;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show() {
	    gameObject.SetActive(true);
	}

	public void Hide() {
	    gameObject.SetActive(false);
	}

    public void MoveToSky()
    {
        _startPosition = gameObject.transform.position;
        Tweener t = gameObject.transform.DOMove (
            new Vector3 (gameObject.transform.position.x, 6f, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        t.OnComplete (() => {
            gameObject.transform.position = _startPosition;
            Hide();
        });

        PlayGoalAnimation();
    }

    private void PlayGoalAnimation()
    {
		ShieldAnimator.SetTrigger("isGoal");
    }
}
