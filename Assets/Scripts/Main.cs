using UnityEngine;

public class Main : MonoBehaviour {
	void Awake() {
		DefsGame.LoadVariables ();
		Defs.audioSource = GetComponent<AudioSource> ();
	}
}
