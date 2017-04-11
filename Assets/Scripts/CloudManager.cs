using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour {
    public GameObject[] Clouds;

    private const float PositionY = 3.05f;
    private const float PositionXSpace = 1f;
    private const float PositionYSpace = 1.3f;

	// Use this for initialization
	void Start ()
	{
	    GameObject go;
	    go = Clouds[0];
	    go.transform.position = new Vector3(-9.5f + Random.Range(-PositionXSpace, PositionXSpace),
	        PositionY + Random.Range(-PositionYSpace, PositionYSpace), 1f);
	    go = Clouds[1];
	    go.transform.position = new Vector3(0f + Random.Range(-PositionXSpace, PositionXSpace),
	        PositionY + Random.Range(-PositionYSpace, PositionYSpace), 1f);
	    go = Clouds[2];
	    go.transform.position = new Vector3(9.5f + Random.Range(-PositionXSpace, PositionXSpace),
	        PositionY + Random.Range(-PositionYSpace, PositionYSpace), 1f);
	}

	// Update is called once per frame
	void Update () {
	    float width = Camera.main.pixelWidth;
	    Vector2 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector2 (0, 0));
	    Vector2 bottomRight = Camera.main.ScreenToWorldPoint(new Vector2 (width, 0));
	    SpriteRenderer sprite;
	    GameObject go;
	    for (int i = 0; i < Clouds.Length; i++)
	    {
	        go = Clouds[i];
	        sprite = go.GetComponent<SpriteRenderer>();
	        go.transform.position = new Vector3(go.transform.position.x - 0.005f, go.transform.position.y, go.transform.position.z);

	        if (go.transform.position.x + sprite.bounds.size.x*0.5f < bottomLeft.x)
	        {
	            go.transform.position = new Vector3(bottomRight.x + sprite.bounds.size.x*0.5f  + Random.Range(0, PositionXSpace),
	                PositionY + Random.Range(-PositionYSpace, PositionYSpace), 1f);
	        }
	    }


	}
}
