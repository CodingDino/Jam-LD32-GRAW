// ************************************************************************ 
// File Name:   DialogueTextSettings.cs 
// Purpose:    	Settings for text printout during a section of dialogue.
// Project:		Armoured Engines
// Author:      Sarah Herzog  
// Copyright: 	2015 Bounder Games
// ************************************************************************ 


// ************************************************************************ 
// Imports 
// ************************************************************************ 
using System.Collections.Generic;


// ************************************************************************ 
// Class: DialogueTextSettings
// ************************************************************************
public class DialogueTextSettings {

    public float textSpeed = 1.0f;
    public float textPitch = 1.0f;
    public float textPitchVariation = 0.0f;
    public string textAudio = "Dialogue-Default";

    public DialogueTextSettings ShallowCopy()
    {
        return this.MemberwiseClone() as DialogueTextSettings;
    }

    static public DialogueTextSettings Load(Dictionary<string, object> JSON, DialogueTextSettings defaults = null)
    {
        DialogueTextSettings newObject;
        if (defaults == null)
            newObject = new DialogueTextSettings();
        else
            newObject = defaults.MemberwiseClone() as DialogueTextSettings;

        if (JSON.ContainsKey("textSpeed"))
            newObject.textSpeed = float.Parse(JSON["textSpeed"].ToString());
        if (JSON.ContainsKey("textPitch"))
            newObject.textPitch = float.Parse(JSON["textPitch"].ToString());
            
        if (JSON.ContainsKey("textPitchVariation"))
            newObject.textPitchVariation = float.Parse(JSON["textPitchVariation"].ToString());
        if (JSON.ContainsKey("textAudio"))
            newObject.textAudio = JSON["textAudio"] as string;

        return newObject;
    }

    public override string ToString()
    {
        return "textSpeed = " + textSpeed + ", textPitch = " + textPitch + ", textPitchVariation = " + textPitchVariation + ". textAudio = " + textAudio;
    }
}