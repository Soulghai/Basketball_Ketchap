using UnityEngine;
using System.Collections;

public class ShieldVisual : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
