using UnityEngine;
using System.Collections;

public class AnimationListener : MonoBehaviour {
	
	// ********************************************************************
	// Events 
	// ********************************************************************
	public delegate void AnimationEventFired(GameObject animationObject, string eventID);
	public static event AnimationEventFired OnAnimationEvent;

	public void AnimationEventFunc (string eventID) {
		//Debug.Log ("AnimationEvent fired: "+eventID);
		if (OnAnimationEvent != null)
		{
			OnAnimationEvent(gameObject, eventID);
		}
	}

}
