# Modinstaller
A small application that installs the Among Us mod [town of us](https://github.com/eDonnes124/Town-Of-Us-R) with minimal user interaction. Making manual to automatic

## Why this over [ModManager](https://github.com/MatuxGG/ModManager) and the ingame updater already present?
- ModManager requires setup before you can install your mods - here you can get right in
- Issues such as with sign in and conflicts with mod's ingame updater have been noted to arise with ModManager
- You have to put in more work navigating through more menus with ModManager  
- Comparing to the updater, you dont have to open the game, update the mod, and restart the game - the exe is ultimately faster
- The updater is rendered unusable with mod updates aimed at new among us versions and a manual installation would have to be done anyway

## How it works
- A user inputs the file path to the folder to install the mod - this is the only thing they have to do
- The program fetches the latest download link and version number from the github repo's API
- The zip is downloaded, extracted, and contents are set up in the folder without further input

## Usage
- Refer to the [latest release](https://github.com/whichtwix/Modinstaller/releases/latest) to know what to do
- You will need to install this once and can reuse it every update
