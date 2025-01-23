==========================================================================================
================================LOCALIZATION PACKAGE======================================
==========================================================================================

=====================================Description:=========================================
This package is designed to help you integrate and modify localized text in a Unity game.

==========================================================================================
=====================================Integration:=========================================

[Localization Manager]
Place the “LocalizationManager.prefab” prefab on your startup scene, it will transfer itself between the scenes
And specify the “LanguageSelection.tsv” file in the “TSV File” parameter of the “LocalizationComponent.cs” script.

Parameters:
languages -> list of languages you want to be supported by your game
defaultLanguage -> a language by default, in case of an error or if your game needs it
currentLanguage -> the current language selected for the game (change depending on the languages & startRoutine varaibles)
startRoutine -> while defin how the language is choosen at the start of the game, can be changed with "SetStartRoutine"

========================
[Localization Component]

Set the “LocalizationComponent.cs” component to the supposed gameObject containing the dialogs,
Then specify in the parameter the .tsv file containing the dialogs.

Parameters:
filePath -> The .tsv file containing the dialogs you want the component to be linked to

========================
[File .TSV]
the .tsv file must look like:

- The first line, with the exception of the first column, must contain the name of each language that will be assigned to the column,
  and therefore to the language of the dialogues that will be written in this same column.
  Language names should be written in English, according to the spelling of the “SystemLanguage” variables.

- The rest of the first column should contain, on each line, the identification key used to identify each dialog.
  It is this key that must be given when the dialogues are called up,
  and which will determine the translation into each language according to the texts written on the same line as the identification key.

If you have any doubts, refer to the .tsv files in the Sample Scene.

==========================================================================================
=========================================Usage:===========================================

"using LocalizationPackage;" & "LocalizationManager.Instance.xxx"
-> Using & variables to call the localization manager's methods.

=====================
[LocalizationManager]
"OnRefresh" event
-> Called everytime the text needs to be refreshed (for changing the language or anything else)
-> Returns the current language is was refreshed with

"CallRefresh()" method
-> Call the "OnRefresh" event

"ChangeLanguage(SystemLanguage newLanguage)" method
-> Change the language to the given parameter
-> If the given language doesn't exist, the language will be set to the default value

"GetLanguageForSelection(SystemLanguage languageToGet, bool sameAsSelectedLanguage)" method
-> Will return the name of the asked language, usefull for language selection in game options for the player
-> The Second parameter ask if the language's name should be returned in the native language or current language, set at false by default

=======================
[LocalizationComponent]
The Localization component should be refered as a variable (in SerializeField for example) if you want to use it in an external Script

"GetText(string Key)" method
-> Will return the text corresponding to the given Key in parameter, in the given .tsv file of the componenent.
-> The language of the text will be in the current language in the localization manager,
   If the text in said language doesn't exist or language isn't referenced, it will take the one in default language,
   If the text in default language doesn't exist, it will return an error
-> A second parameter exist but is not intended for this kind of usage, pls keep it at false to avoid useless errors

==========================================================================================
If you need more informations, feel free to explore the Sample Scene in the packages files