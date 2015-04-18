// ************************************************************************ 
// File Name:   ExtensionMethods.cs 
// Purpose:    	Extends common Unity classes with useful methods
// Project:		Framework
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections;


// ************************************************************************ 
// Class: ExtensionMethods
// ************************************************************************ 
public static class ExtensionMethods {
	
	
	// ********************************************************************
	// Function:	ResetTransform()
	// Purpose:		Sets the transform to it's default state
	// ********************************************************************
	public static void ResetTransform(this Transform _trans)
	{
		_trans.position = Vector3.zero;
		_trans.localRotation = Quaternion.identity;
		_trans.localScale = new Vector3(1, 1, 1);
	}


	// ********************************************************************
	// Function:	DestroyChildren()
	// Purpose:		Destroys all the transform's children
	// ********************************************************************
	public static void DestroyChildren(this Transform _trans)
	{
		for (int i = 0; i < _trans.childCount; ++i)
		{
			GameObject.Destroy (_trans.GetChild(i).gameObject);
		}
	}
}
