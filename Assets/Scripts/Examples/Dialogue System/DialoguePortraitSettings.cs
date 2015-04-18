// ************************************************************************ 
// File Name:   DialoguePortraitSettings.cs 
// Purpose:    	Settings for the portrait during a section of dialogue.
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections.Generic;


// ************************************************************************ 
// Class: DialoguePortraitSettings
// ************************************************************************
public class DialoguePortraitSettings {

    public bool active = true;
    public bool large = false;
    
    public string image;
    public string displayName;
    public enum PortraitPosition { left, right }
    public PortraitPosition position = PortraitPosition.left;
    
    public string idleAnimation = "Neutral";
    public string talkAnimation = "Neutral";

    public DialoguePortraitSettings ShallowCopy()
    {
        return this.MemberwiseClone() as DialoguePortraitSettings;
    }

    static public DialoguePortraitSettings Load(Dictionary<string, object> JSON, DialoguePortraitSettings defaults = null)
    {
        DialoguePortraitSettings newObject;
        if (defaults == null)
            newObject = new DialoguePortraitSettings();
        else
            newObject = defaults.MemberwiseClone() as DialoguePortraitSettings;

        // Display Info
        if (JSON.ContainsKey("active"))
            newObject.active = bool.Parse(JSON["active"].ToString());
        if (JSON.ContainsKey("image"))
            newObject.image = JSON["image"] as string;
        if (JSON.ContainsKey("displayName"))
            newObject.displayName = JSON["displayName"] as string;
        if (JSON.ContainsKey("position"))
            newObject.position = JSON["position"] as string == "left" ? PortraitPosition.left : PortraitPosition.right;
        if (JSON.ContainsKey("large"))
            newObject.large = bool.Parse(JSON["large"].ToString());
            
        // Animations
        if (JSON.ContainsKey("idleAnimation"))
            newObject.idleAnimation = JSON["idleAnimation"] as string;
        if (JSON.ContainsKey("talkAnimation"))
            newObject.talkAnimation = JSON["talkAnimation"] as string;

        return newObject;
    }
}