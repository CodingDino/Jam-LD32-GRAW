// ************************************************************************ 
// File Name:   DialogueSection.cs 
// Purpose:    	Information about a section of dialogue
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using UnityEngine;
using System.Collections.Generic;
using System.Text;


// ************************************************************************ 
// Class: DialogueSection
// ************************************************************************ 
public class DialogueSection {

    // Overrides (will be set to parent values if no override present) 
    public DialoguePortraitSettings portraitSettings;
    public DialogueTextSettings textSettings;

    // Animations and Effects
    public string triggerAnimation;
    public string triggerEffect;
    public bool forceIdle = false;

    // Text
    public StringBuilder text;

    public static DialogueSection Load(Dictionary<string, object> JSON, DialogueSection defaults = null)
    {
        DialogueSection newObject;
        if (defaults == null)
            newObject = new DialogueSection();
        else
            newObject = defaults.MemberwiseClone() as DialogueSection;
    
        if (JSON.ContainsKey("portraitSettings"))
            newObject.portraitSettings = DialoguePortraitSettings.Load(JSON["portraitSettings"] as Dictionary<string, object>, defaults.portraitSettings);
        if (JSON.ContainsKey("textSettings"))
            newObject.textSettings = DialogueTextSettings.Load(JSON["textSettings"] as Dictionary<string, object>, defaults.textSettings);

        if (JSON.ContainsKey("triggerAnimation"))
            newObject.triggerAnimation = JSON["triggerAnimation"] as string;
        if (JSON.ContainsKey("triggerEffect"))
            newObject.triggerEffect = JSON["triggerEffect"] as string;
        if (JSON.ContainsKey("forceIdle"))
            newObject.forceIdle = bool.Parse(JSON["forceIdle"] as string);

        if (JSON.ContainsKey("text"))
            newObject.text = new StringBuilder(JSON["text"] as string);

        if (newObject.portraitSettings == null)
            Debug.LogError("portraitSettings is null in DialogueSection");
        if (newObject.textSettings == null)
            Debug.LogError("textSettings is null in DialogueSection");

        return newObject;
    }
}
