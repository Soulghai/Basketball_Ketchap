using UnityEngine;
using DG.Tweening;

public class RingHead : MonoBehaviour {
    private Vector3 _startPosition;

	// Use this for initialization
	void Start () {
	    _startPosition = gameObject.transform.position;
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
        Tweener t = gameObject.transform.DOMove (
            new Vector3 (gameObject.transform.position.x, 6f, 1f), 0.5f);
        t.SetEase (Ease.InCubic);
        t.OnComplete (() => {
            gameObject.transform.position = _startPosition;
            Hide();
        });
    }
}
