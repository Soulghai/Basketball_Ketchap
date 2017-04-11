using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    public static event Action OnBallInBasket;
    public static event Action<int> OnGoal;
    public static event Action OnThrow;
    public static event Action<float> OnMiss;
    public static event Action OnCoinSensor;

    private int _targetLinePointCount;
    public GameObject TargetLinePoint;
    private const int TargetHintPartCountMax = 20;
    private int _targetHintPartCount;
    private readonly GameObject[] _targetLinePoints = new GameObject[20 + TargetHintPartCountMax];
    private Vector3 _mouseTarget;
    private int _indicatorCurrentParth = 0;
    private Rigidbody2D _body;

    private Vector3 _startPosition;
    private readonly Vector3 _mouseStartPosition = new Vector3(-1.5f, 2.6f, 1f);
    private Vector3 _targetLosePosition;

    private bool _isSetStartPoint;
    private bool _isThrow;
    private float _lifeTime;
    private float _lifeDelay = 1.0f;
    private bool _isGoal;
    private bool _isGoalTrigger;
    private bool _isShowBall;
    private bool _isLose = false;
    private bool _isHideBall = true;

    private bool _isDrawTargetLine = false;

    private AudioSource _ballThrow;

    private float _oldVelocityY;

    private int _pointsCount;
    private Vector3 _oldMousePosition;
    private Vector3 _mousePosition;
    private bool _isShield;

    private bool _isTryThrow;
    private Sprite _sprite;
    private SpriteRenderer _spriteRenderer;

    // Use this for initialization
    void Start()
    {
        DefsGame.Ball = this;

        _body = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        loadSprites (DefsGame.currentFaceID);
        _targetLinePointCount = _targetLinePoints.Length - TargetHintPartCountMax;

        _startPosition = transform.position;

        for (int i = 0; i < _targetLinePoints.Length; i++)
        {
            _targetLinePoints[i] = (GameObject) Instantiate(TargetLinePoint, transform.position, Quaternion.identity);
        }

        _ballThrow = GetComponent<AudioSource>();

        _targetHintPartCount = TargetHintPartCountMax;

        _targetLosePosition = _mouseStartPosition;
        SetNewSkin(DefsGame.currentFaceID);
    }

    public void SetNewSkin(int _id) {
        loadSprites (DefsGame.currentFaceID);
        _spriteRenderer.sprite = _sprite;
    }

    void loadSprites(int _id){
        //if ((Sprite)_sprite) Resources.UnloadAsset(_sprite);
        _sprite = Resources.Load<Sprite>("gfx/Balls/ball" +(_id+1).ToString());
    }

    public void Respown(Vector3 position)
    {
        if (_isLose && DefsGame.currentPointsCount == 0) _mouseTarget = _targetLosePosition;
        else
            _mouseTarget = _mouseStartPosition;

        _mouseTarget = _targetLosePosition;

        _startPosition = position;
        transform.position = new Vector3(_startPosition.x + 0.2f, _startPosition.y + 0.5f, _startPosition.z);

        _lifeTime = 0f;
        _isShield = false;
        _isLose = false;
        _isGoal = false;
        _isGoalTrigger = false;
        _isSetStartPoint = false;
        _isHideBall = false;
        _isShowBall = true;
        transform.localScale = new Vector3(0f, 0f, transform.localScale.z);
        _isThrow = false;
        _body.isKinematic = true;
        _body.velocity = new Vector2();
        _body.angularVelocity = 0f;
        _body.rotation = 0;

        _isTryThrow = false;
        _pointsCount = 1;
    }

    public void Hide()
    {
        _isHideBall = true;
    }

    public void Show()
    {
        _isShowBall = true;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_oldVelocityY > 0f) && (_body.velocity.y <= 0f))
        {
            D.Log("Gothca");
        }

        _oldVelocityY = _body.velocity.y;

        if (!_isThrow)
        {
            if ((!_isSetStartPoint)&&((DefsGame.currentScreen == DefsGame.SCREEN_GAME)||(DefsGame.currentScreen == DefsGame.SCREEN_MENU)))
                SetStartPoint();

            if (_isDrawTargetLine) DrawTargetLine();

            if ((_isSetStartPoint) && (InputController.IsTouchOnScreen(TouchPhase.Ended)))
            {
                _isTryThrow = true;
            }

            if ((_isTryThrow) && (!_isShowBall))
            {
                ThrowBall();
            }

            if (_isShowBall)
            {
                if (transform.localScale.x < 1f)
                {
                    transform.localScale = new Vector3(transform.localScale.x + 0.1f, transform.localScale.y + 0.1f,
                        transform.localScale.z);
                }
                else
                {
                    _isDrawTargetLine = true;
                    _isShowBall = false;
                    transform.localScale = new Vector3(1f, 1f, transform.localScale.z);
                }
            }
        }
        else
        {
            HideTargetLine(true);

            if (_isHideBall)
            {
                if (transform.localScale.x > 0f)
                {
                    transform.localScale = new Vector3(transform.localScale.x - 0.1f, transform.localScale.y - 0.1f,
                        transform.localScale.z);
                }
                else
                {
                    _isHideBall = false;
                    transform.localScale = new Vector3(0f, 0f, transform.localScale.z);
                    //if (_isLose && !DefsGame.RingManager.WaitMoveToStartPosition) Respown(_startPosition);
                }
            }

            if (_isGoal)
            {
                _lifeTime += Time.deltaTime;
                if (_lifeTime >= _lifeDelay)
                {
                    GameEvents.Send(OnBallInBasket);
                    _isHideBall = true;
                    _body.isKinematic = true;
                    if (_targetHintPartCount > 0)
                    {
                        _targetHintPartCount -= 4;
                        if (_targetHintPartCount < 0) _targetHintPartCount = 0;
                    }
                    _lifeTime = -10000f;
                }
            }

            if (_isLose)
            {
                _lifeTime += Time.deltaTime;
                if (_lifeTime >= _lifeDelay)
                {
                    _isHideBall = true;
                    _body.isKinematic = true;
                    _lifeTime = -10000f;
                    GameEvents.Send(OnMiss, 0.1f);
                }
            }
        }
    }

    private void SetStartPoint()
    {
        if (InputController.IsTouchOnScreen(TouchPhase.Began))
        {
            _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _isSetStartPoint = true;
            D.Log("Set Start Point");
        }
    }

    private void DrawTargetLine()
    {
        if (_isSetStartPoint)
            if (InputController.IsTouchOnScreen(TouchPhase.Moved))
            {
                _oldMousePosition = _mousePosition;
                _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                _mouseTarget.x += (_mousePosition.x - _oldMousePosition.x) * 2f;
                _mouseTarget.y += (_mousePosition.y - _oldMousePosition.y) * 2f;

                if (_mouseTarget.y > ScreenSettings.ScreenHeight * 0.5f)
                {
                    _mouseTarget.y = ScreenSettings.ScreenHeight * 0.5f;
                }
            }

        Vector2 dist = new Vector2(_mouseTarget.x - transform.position.x, _mouseTarget.y - transform.position.y);

        CutDistance(ref dist);

        Vector2 force = CalcForce(dist.x, dist.y);
        float t = force.y / Physics.gravity.y;

        float _pX = transform.position.x;
        float _pY = transform.position.y;
        float dTime = t / _targetLinePointCount;
        float velY = -force.y;
        float velX = dist.x / _targetLinePointCount;

        if (_indicatorCurrentParth < _targetLinePointCount)
            ++_indicatorCurrentParth;
        else
            _indicatorCurrentParth = 0;

        GameObject _object;
        float _scale = 1f;
        for (int i = 0; i < _targetLinePointCount + _targetHintPartCount; i++)
        {
            _pX += velX;
            velY += Physics.gravity.y * dTime;
            _pY += velY * dTime;
            _object = _targetLinePoints[i];
            if (i == _targetLinePointCount - 1)
                _scale = 1.0f;
            //else
            //	if (i == indicatorCurrentParth)
            //	_scale = 1f;
            else
            {
                if (i < _targetLinePointCount + 1)
                    _scale = 0.15f + ((float) i / (float) _targetLinePointCount) * 0.5f;
                else
                    _scale = 0.65f - (float) (i - TargetHintPartCountMax) * (0.5f / TargetHintPartCountMax);

                if (i == _indicatorCurrentParth) _scale *= 1.2f;
            }

            _object.transform.localScale = Vector3.Lerp(_object.transform.localScale, new Vector3(_scale, _scale, 1f),
                0.2f);

            _object.transform.position = new Vector3(_pX, _pY, 1f);
        }
    }

    private void HideTargetLine(bool _anim = true)
    {
        if (_anim)
        {
            GameObject _object;
            for (int i = 0; i < _targetLinePointCount + TargetHintPartCountMax; i++)
            {
                _object = _targetLinePoints[i];
                _object.transform.localScale = Vector3.Lerp(_object.transform.localScale, new Vector3(0, 0, 1f), 0.05f);
            }
        }
        else
        {

        }
    }

    private void ThrowBall()
    {
        if (!_isSetStartPoint) return;

        ++DefsGame.ThrowsCounter;

        _targetLosePosition = _mouseTarget;

        GameEvents.Send(OnThrow);

        Vector2 dist = new Vector2(_mouseTarget.x - transform.position.x, _mouseTarget.y - transform.position.y);

        CutDistance(ref dist);

        _pointsCount = 3;

        _body.velocity = new Vector2();
        _body.angularVelocity = 0f;
        _body.isKinematic = false;

        Vector2 force = CalcForce(dist.x, dist.y);
        _body.AddForce(new Vector2(force.x * 74.4f, force.y * 74.4f)); //49.45
        _body.AddTorque(1);
        _isThrow = true;

        _ballThrow.Play();
    }

    Vector2 CalcForce(float distX, float distY)
    {
        Vector2 force = new Vector2();
        force.y = Mathf.Sqrt(2f * distY * (-Physics.gravity.y));
        float t = force.y / -Physics.gravity.y;
        force.x = distX / t;
        return force;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isGoal)
            return;
        if (other.CompareTag("LoseTrigger"))
        {
            _isLose = true;
            _lifeTime = 0;
            _lifeDelay = 0.1f;
        }
        else if (other.CompareTag("GoalTrigger"))
        {
            _isGoalTrigger = true;
            if (_isLose)
            {
                _isLose = false;
                _pointsCount = 30;
            }
        }
        else if (other.CompareTag("GoalTrigger2"))
        {
            if (_isGoalTrigger)
            {
                _isGoal = true;
                _lifeDelay = 1f;
                _lifeTime = 0;
                if (_isShield)
                {
                    if (_pointsCount == 3)
                        _pointsCount = 1;
                    else if (_pointsCount == 30)
                    {
                        _pointsCount = 10;
                    }
                }
                GameEvents.Send(OnGoal, _pointsCount);
            }
        }
        else if (other.CompareTag("CoinSensor"))
        {
            GameEvents.Send(OnCoinSensor);
            D.Log("CoinSensor");
        }
    }

void OnCollisionEnter2D(Collision2D other) {
		if (_isGoal)
			return;
        if (other.gameObject.CompareTag("LoseTrigger"))
        {
            if (_isLose)
            {
                _lifeDelay = 1f;
            }
            else
            {
                _isLose = true;
                _lifeTime = 0;
                _lifeDelay = 2f;
            }
        } else
        if (other.gameObject.CompareTag("ShieldTrigger")) {
            _isShield = true;

        }
	}

    private void CutDistance(ref Vector2 dist) {
		if (dist.y < 1f) {
			dist.y = 1f;
		}

		if (dist.x < 1.0f) {
			dist.x = 1.0f;
		} else if (dist.x > 8.5f) {
			dist.x = 8.5f;
		}
	}
}
