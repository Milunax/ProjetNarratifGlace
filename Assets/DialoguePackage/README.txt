------------------------------------------------------
--------------- Package Dialogue Graph ---------------
------------------------------------------------------
This package is made to ease the workload of game designers
and any person working on the dialogue of a game.
It is easy to use, and lets the user create and save multiple
dialogues with forking paths, different types of endings and 
even set up pictures and names for the talking characters.
------------------------------------------------------
Installation:
------------------------------------------------------
How to create your own Dialogue Object:
	- Right clicking the folders menu, you will notice a new category of item you can create, "Dialogue", click on the "Dialogue" option.
	- You can then select the Dialogue_SO option in the dropdown which will then create a Dialogue_SO Scriptable object
	   In the current folder
	- Write the wanted name and enter to finish creating your dialogue object.

How to access the graph and setup your dialogue:
	- You can open the Dialogue Graph by double clicking the newly created Dialogue_SO.
	- Right clicking in the dialogue graph window will open a pop-up menu
	- Selecting the Nodes option will open another pop-up menu, in which you can access the
	  different kind of nodes you'll be able to use on your graphs.
	- Each graph needs a beginning and an end node.

------------------------------------------------------
Template Scene:
------------------------------------------------------


We have a dialogue controller prefab that holds the DialogueController script, holding references to the required items in the scene

A Speaker that acts as the entry point to the dialogue, they have an example script called DialogueTalkZone that simulates an interaction by the player to launch a dialogue
DialogueTalk is the main script that handles the flow of dialogue

The script Dialogue Talk may contain a reference to a Tsv file, allowing use alongside the localisation package made by Titouan, if both packages are present at the same time, the behavior of the DialogueGraph changes slightly, changing the values from being set directly by the nodes to searching for a translation in the referenced tsv document.

------------------------------------------------------
Nodes:
------------------------------------------------------

Start node: 
	Entry Point
	Only important value is the id, when using the StartDialogue in your scripts, you want to send the id of your start 	node if there's multiple present, otherwise it's going to select the first one, this allows multiple starting points 	to coexist in a single dialogue graph

End node:
	End Point
	Has different types of ending behaviors
	Repeat (repeat the same node)
	Go back (go back one node)
	Return Start (return to the first node of the graph)
	End (End dialogue)

Event node:
	not implemented yet

Dialogue node:
	Add choice button:
		Multiple choices for the buttons at the button of the screen, allows for branching dialogue
	Picture Input:
		Input to set the sprite/artwork that will popup alongside your dialogue
	Dropdown menu:
		If the picture selected is to be set to the left icon or right icon (defined in the controller)
	Name:
		Name/Key for the Name textbox
	Key text:
		Text/Key for the Text textbox