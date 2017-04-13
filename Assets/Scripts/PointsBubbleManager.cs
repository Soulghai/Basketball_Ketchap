using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointsBubbleManager : MonoBehaviour {
	public GameObject pointsObject;
	// Use this for initialization
	bool isAddPoint = false;
	float time = 0;
	float delay = 0.2f;
	int count = 0;
	int colorID;
	Vector3 pos;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isAddPoint) {
			time += Time.deltaTime;
			if (time >= delay) {
				isAddPoint = false;
				time = 0;
				GameObject _go = (GameObject)Instantiate (pointsObject, pos, Quaternion.identity);
				Text _text = _go.GetComponentInChildren<Text> ();
				_text.text = "+" + count.ToString ();
			    switch (count)
			    {
			        case 1:
			        {
			            _text.color = new Color(170f / 255f, 75f / 255f, 196f / 255f);
			            _go.transform.localScale = new Vector3(_go.transform.localScale.x * 0.5f, _go.transform.localScale.x * 0.5f,
			                1);
			        }
			            break;
			        case 3:
			        {
			            _text.color = new Color(77f / 255f, 97f / 255f, 217f / 255f);
			            _go.transform.localScale = new Vector3(_go.transform.localScale.x * 0.75f,
			                _go.transform.localScale.x * 0.75f,
			                1);
			        }
			            break;
			        case 10:
			        {
			            _text.color = new Color(222f / 255f, 125f / 255f, 48f / 255f);
			            _go.transform.localScale = new Vector3(_go.transform.localScale.x * 1.5f, _go.transform.localScale.x * 1.5f,
			                1);
			        }
			            break;
			        case 30:
			        {
			            _text.color = new Color(42f / 255f, 131f / 255f, 30f / 255f);
			            _go.transform.localScale = new Vector3(_go.transform.localScale.x * 2f, _go.transform.localScale.x * 2f,
			                1);
			        }
			            break;
			    }
			}
		}
	}

	public void AddPoints(int _count) {
		isAddPoint = true;
		colorID = DefsGame.BUBBLE_COLOR_ONE;
		count = _count;


		RingObject ringObject = DefsGame.RingManager.NextRing.GetComponent<RingObject> ();
		pos = ringObject.PointsPlace.position;
	}
}
