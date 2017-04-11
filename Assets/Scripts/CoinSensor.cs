using UnityEngine;
using System.Collections;

public class CoinSensor : MonoBehaviour {
    bool _isShowAnimation;
    bool _isHideAnimation;

    private Collider2D collider;

	// Use this for initialization
	void Start ()
	{
	    DefsGame.CoinSensor = this;
	    collider = GetComponent<Collider2D>();
	}

    public void Init()
    {
        transform.localScale = new Vector3 (0f, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
	    if (_isShowAnimation)
	    {
            transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
            if (transform.localScale.x >= 1f) {
                _isShowAnimation = false;
                transform.localScale = new Vector3 (1f, 1f, 1f);
            }
	    } else
	    if (_isHideAnimation)
	    {
	        transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
	        if (transform.localScale.x <= 0f) {
	            //GameEvents.Send(OnAddCoinsVisual, 1);
	            //Destroy (gameObject);
	            _isHideAnimation = false;
	            transform.localScale = new Vector3 (0f, 0f, 0f);
	        }
	    }
	}

    public void Show(bool _isAnimation) {
        _isShowAnimation = _isAnimation;
        collider.enabled = true;

        if (_isShowAnimation)
        {
            transform.localScale = new Vector3 (0f, 0f, 1f);
        }
    }

    public void Hide()
    {
        collider.enabled = false;
        _isHideAnimation = true;
    }
}
