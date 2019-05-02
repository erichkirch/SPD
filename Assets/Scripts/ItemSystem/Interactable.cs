using UnityEngine;

/*	
	This component is for all objects that the player can
	interact with such as enemies, items etc. It is meant
	to be used as a base class.
*/

public class Interactable : MonoBehaviour {
	public Transform interactionTransform;	// The transform from where we interact in case you want to offset it

	bool isFocus = false;	// Is this interactable currently being focused?
	Transform player;		// Reference to the player transform

	bool hasInteracted = false;	// Have we already interacted with the object?

	public virtual void Interact ()
	{
		// This method is meant to be overwritten
		Debug.Log("Interacting with " + transform.name);
	}
    

	void Update ()
	{
		if (isFocus && !hasInteracted)	// If we are currently being focused and we haven't already interacted with the object
		{
			Interact();
			hasInteracted = true;
		}
	}

	// Called when the object starts being focused
	public void OnFocused (Transform playerTransform)
	{
		isFocus = true;
		player = playerTransform;
		hasInteracted = false;
	}

	// Called when the object is no longer focused
	public void OnDefocused ()
	{
		isFocus = false;
		player = null;
		hasInteracted = false;
	}

}
