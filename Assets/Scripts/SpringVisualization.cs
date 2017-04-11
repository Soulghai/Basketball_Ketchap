using UnityEngine;
using System.Collections;

public class SpringVisualization : MonoBehaviour {
	public Transform point1;
	public Transform point2;
	float startDistance;

	// Use this for initialization
	void Start () {
		startDistance = Vector3.Distance (point1.position, point2.position);
	}

	// Update is called once per frame
	void Update () {
		transform.position = CalcPosition (point1.position, point2.position);
		transform.rotation = Quaternion.AngleAxis(CalcAngle (point1.position, point2.position) * Mathf.Rad2Deg, Vector3.forward);
		float _newScale = Vector3.Distance (point1.position, point2.position)/startDistance;
		transform.localScale = new Vector3 (_newScale, Mathf.Min(_newScale, 1f), 1f);
	}

	Vector3 CalcPosition(Vector3 _point1, Vector3 _point2) {
		return (_point1 + _point2)/2f;
	}

	float CalcAngle(Vector3 _point1, Vector3 _point2) {
		return  Mathf.Atan2 (_point1.y - _point2.y, _point1.x - _point2.x);
	}
}
