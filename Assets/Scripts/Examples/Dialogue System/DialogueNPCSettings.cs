// ************************************************************************ 
// File Name:   DialogueNPCSettings.cs 
// Purpose:    	Information about dialogue settings for a specific NPC
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using System.Collections.Generic;
using UnityEngine;


// ************************************************************************ 
// Class: DialogueNPCSettings
// ************************************************************************ 
public class DialogueNPCSettings  {
    public string id;
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Runtime Objects
    public Sprite largePortrait; // TODO: Animation?

    public static DialogueNPCSettings Load(Dictionary<string, object> JSON, DialogueNPCSettings defaults = null)
    {
        DialogueNPCSettings newObject;
        if (defaults == null)
            newObject = new DialogueNPCSettings();
        else
            newObject = defaults.MemberwiseClone() as DialogueNPCSettings;

        if (JSON.ContainsKey("id"))
            newObject.id = JSON["id"] as string;

        if (JSON.ContainsKey("portraitSettings"))
            newObject.portraitSettings = DialoguePortraitSettings.Load(JSON["portraitSettings"] as Dictionary<string, object>, newObject.portraitSettings);
        if (JSON.ContainsKey("textSettings"))
            newObject.textSettings = DialogueTextSettings.Load(JSON["textSettings"] as Dictionary<string, object>, newObject.textSettings);

        return newObject;
    }

    public override string ToString()
    {
        return "portraitSettings = {" + portraitSettings + "}, textSettings = {" + textSettings + "}";
    }

}
