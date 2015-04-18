// ************************************************************************ 
// File Name:   MusicManager.cs 
// Purpose:    	
// Project:		Framework
// Author:      Sarah Herzog  
// Copyright: 	2014 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// ************************************************************************ 
// Attributes 
// ************************************************************************ 


// ************************************************************************ 
// Class: MusicManager
// ************************************************************************ 
public class MusicManager : MonoBehaviour {


    // ********************************************************************
    // Private Data Members 
    // ********************************************************************
	[SerializeField]
	private FadeAudio m_fadeAudio;
	private AudioClip m_nextClip;
	
	
	// ********************************************************************
	// Static Data Members 
	// ********************************************************************
	private static MusicManager s_instance;
	
	
	// ********************************************************************
	// Properties 
	// ********************************************************************
	public static MusicManager instance { 
		get {			
			if ( s_instance != null)
				return s_instance;
			else
				return s_instance = (MusicManager)FindObjectOfType(typeof(MusicManager));
		} 
	}


	// ********************************************************************
	public static void FadeOut()
	{
		instance.m_fadeAudio.FadeOut();
	}
	// ********************************************************************


	// ********************************************************************
	public static void FadeIn()
	{
		instance.m_fadeAudio.FadeIn();
	}
	// ********************************************************************
	
	
	// ********************************************************************
	public static void PlayMusic(string clipResourceLocation) {
		AudioClip clip = (AudioClip) Resources.Load(clipResourceLocation);
		PlayMusic(clip);
	}
	// ********************************************************************


    // ********************************************************************
	public static void PlayMusic(AudioClip clip) {
		Debug.Log ("Queuign clip "+clip.name);
		// fade current audio out if clip does not match, then when faded out, switch clips and fade back in
		if (instance.m_fadeAudio.audioSource.clip != clip)
		{
			instance.m_fadeAudio.FadeOut();
			instance.m_nextClip = clip;
		}
	}
	// ********************************************************************

	
	// ********************************************************************
	void Update()
	{
		if (m_nextClip != null && m_fadeAudio.isInaudible)
		{
			Debug.Log ("Playing clip "+m_nextClip.name);
			instance.m_fadeAudio.audioSource.clip = m_nextClip;
			instance.m_fadeAudio.audioSource.Play();
			m_nextClip = null;
			instance.m_fadeAudio.FadeIn();
		}
	}
	// ********************************************************************


}