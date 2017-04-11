﻿using UnityEngine;
using System.Collections;

public class RingObject : MonoBehaviour
{
    public Transform PointsPlace;
	float _time;
	const float Delay = 1f;
	bool _isRemove;

	// Use this for initialization
	void Start () {
		_isRemove = false;
		_time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (_isRemove) {
			_time += Time.deltaTime;
			if (_time >= Delay) {
				_time = 0f;
				_isRemove = false;
				gameObject.SetActive (false);
			}
		}
	}

	public void Remove() {
		_time = 0f;
		_isRemove = true;
	}
}