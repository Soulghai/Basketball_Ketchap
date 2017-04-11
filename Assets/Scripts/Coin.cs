﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Coin : MonoBehaviour {
	public static event Action<int> OnAddCoinsVisual;

	[HideInInspector] public GameObject parentObj;
	Vector3 _targetPos;
	float _velocity;
	float _moveAngle;
	bool _isAnglePlus;
	float _addAngleCoeff;
	bool _isShowAnimation;
	bool _isHideAnimation;
	bool _isMoveToTarget = false;
	const float VelocityMax = 0.2f;
	float _showTime = 0f;

	// Use this for initialization
	void Start () {
		
	}

	public void Show(bool _isAnimation) {
		_isShowAnimation = _isAnimation;

		if (_isShowAnimation) {
			transform.localScale = new Vector3 (0f, 0f, 1f);
		}
	}

	public void Hide(bool _isAnimation) {
		_isHideAnimation = _isAnimation;

		if (!_isHideAnimation) {
			Destroy (gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!_isMoveToTarget) {
			if (parentObj) transform.position = parentObj.transform.position;
		}

		if (_isShowAnimation)
		{
			if (_showTime >= 0.5f) {
				transform.localScale = new Vector3 (transform.localScale.x + 0.1f, transform.localScale.y + 0.1f, 1f);
				if (transform.localScale.x >= 1f) {
					_isShowAnimation = false;
					transform.localScale = new Vector3 (1f, 1f, 1f);
				}
			} else {
				_showTime += Time.deltaTime;
			}
		} else
			if (_isHideAnimation)
			{
				transform.localScale = new Vector3 (transform.localScale.x - 0.1f, transform.localScale.y - 0.1f, 1f);
				if (transform.localScale.x <= 0f) {
					//GameEvents.Send(OnAddCoinsVisual, 1);
					Destroy (gameObject);
				}
			}else
				if (_isMoveToTarget) {
					if (transform.localScale.x >= 1) transform.localScale = new Vector3(transform.localScale.x - 0.1f,
						transform.localScale.y - 0.1f, 1f);
					float _ang = Mathf.Atan2 (_targetPos.y - transform.position.y, _targetPos.x - transform.position.x);

					if (_isAnglePlus) {
						_moveAngle += _addAngleCoeff* Mathf.Deg2Rad;
						if (_moveAngle >= 180f* Mathf.Deg2Rad) _moveAngle -= 360f* Mathf.Deg2Rad;
					} else {
						_moveAngle -= _addAngleCoeff* Mathf.Deg2Rad;
						if (_moveAngle <= -180f* Mathf.Deg2Rad) _moveAngle += 360f* Mathf.Deg2Rad;
					}

					if (_addAngleCoeff < 35f) _addAngleCoeff += 0.5f;

					if (Mathf.Abs(_moveAngle - _ang) < _addAngleCoeff*1.5f* Mathf.Deg2Rad) _moveAngle = _ang;

					if (_velocity < VelocityMax) _velocity += 0.005f;
					transform.position = new Vector3(transform.position.x + _velocity * Mathf.Cos(_moveAngle),
						transform.position.y + _velocity * Mathf.Sin(_moveAngle), 1f);

					if (Vector2.Distance(transform.position, _targetPos) <= 0.1f) {
						_isMoveToTarget = false;

						GameEvents.Send(OnAddCoinsVisual, 1);
						Destroy(gameObject);
					    //DefsGame.btnBuyCoinsComponent.addOneCoinVisual();
					    //Defs.soundEngine.playSound(sndTouch);
					}
				}
	}

	public void MoveToEnd()
	{
	    Vector3 newPos = Camera.main.ScreenToWorldPoint(DefsGame.Coins.img.transform.position);
	    _targetPos = new Vector3(newPos.x, newPos.y, gameObject.transform.position.z);
		_velocity = 0.03f + Random.value*0.03f;
		if (Random.value < 0.5f) _moveAngle = Random.value * 180f* Mathf.Deg2Rad;
		else _moveAngle = -Random.value * 180f* Mathf.Deg2Rad;

		if (Random.value < 0.5f) _isAnglePlus = false; else _isAnglePlus = true;

		_addAngleCoeff = 5f;

		_isMoveToTarget = true;

		transform.localScale = new Vector3 (2f, 2f);
	} 
}
