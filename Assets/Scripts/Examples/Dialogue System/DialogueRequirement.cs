// ************************************************************************ 
// File Name:   DialogueRequirement.cs 
// Purpose:    	Information about requirements for a conversation
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
// Class: DialogueRequirement
// ************************************************************************
public class DialogueRequirement {
    public string compareID;
    public enum RequirementType {
        INVALID = -1,
        QUEST_COMPLETE,
        QUEST_NOT_COMPLETE,
        CONVERSATION_SEEN,
        CONVERSATION_NOT_SEEN,
        CHOICE_MADE,
        CHOICE_NOT_MADE,
        NUM
    }
    public RequirementType requirementType = RequirementType.INVALID;

    static public DialogueRequirement Load(Dictionary<string, object> JSON, DialogueRequirement defaults = null)
    {
        DialogueRequirement newObject;
        if (defaults == null)
            newObject = new DialogueRequirement();
        else
            newObject = defaults.MemberwiseClone() as DialogueRequirement;

        if (JSON.ContainsKey("questComplete"))
        {
            newObject.requirementType = RequirementType.QUEST_COMPLETE;
            newObject.compareID = JSON["questComplete"] as string;
        }
        else if (JSON.ContainsKey("questNotComplete"))
        {
            newObject.requirementType = RequirementType.QUEST_NOT_COMPLETE;
            newObject.compareID = JSON["questNotComplete"] as string;
        }
        else if (JSON.ContainsKey("conversationSeen"))
        {
            newObject.requirementType = RequirementType.CONVERSATION_SEEN;
            newObject.compareID = JSON["conversationSeen"] as string;
        }
        else if (JSON.ContainsKey("conversationNotSeen"))
        {
            newObject.requirementType = RequirementType.CONVERSATION_NOT_SEEN;
            newObject.compareID = JSON["conversationNotSeen"] as string;
        }
        else if (JSON.ContainsKey("choiceMade"))
        {
            newObject.requirementType = RequirementType.CHOICE_MADE;
            newObject.compareID = JSON["choiceMade"] as string;
        }
        else if (JSON.ContainsKey("choiceNotMade"))
        {
            newObject.requirementType = RequirementType.CHOICE_NOT_MADE;
            newObject.compareID = JSON["choiceNotMade"] as string;
        }

        return newObject;
    }
    
	public bool MeetsRequirements(PlayerProfile _profile)
    {
        switch (requirementType)
        {
//            case RequirementType.QUEST_COMPLETE:
//                return _profile.completedQuests.Contains(compareID);
//            case RequirementType.QUEST_NOT_COMPLETE:
//                return !_profile.completedQuests.Contains(compareID);
//            case RequirementType.CONVERSATION_SEEN:
//                return _profile.conversationsSeen.Contains(compareID);
//            case RequirementType.CONVERSATION_NOT_SEEN:
//                return !_profile.conversationsSeen.Contains(compareID);
//            case RequirementType.CHOICE_MADE:
//                return _profile.choicesMade.Contains(compareID);
//            case RequirementType.CHOICE_NOT_MADE:
//                return !_profile.choicesMade.Contains(compareID);
            default :
                Debug.LogError("Unknown RequirementsType: " + requirementType);
                return false;
        }
    }
}
