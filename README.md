![GitHub all releases](https://img.shields.io/github/downloads/whichtwix/Modinstaller/total?color=%20%2332CD32&style=plastic)
![GitHub release (latest by date)](https://img.shields.io/github/v/release/whichtwix/Modinstaller?style=plastic)
#  :hammer_and_wrench:Modinstaller
A small application that can install multiple mods with minimal user interaction, making manual installs automatic


## :grey_question:Why this over [ModManager](https://github.com/MatuxGG/ModManager) and the ingame updater already present?
- ModManager requires setup before you can install your mods - here you can get right in
- Issues such as with sign in and conflicts with mod's ingame updater have been noted to arise with ModManager
- You have to put in more work navigating through more menus with ModManager  
- Comparing to the updater, you dont have to open the game, update the mod, and restart the game - the exe is ultimately faster
- The updater is rendered unusable with mod updates aimed at new among us versions and a manual installation would have to be done anyway

## 	:gear:How it works
- A user inputs the file path to the folder to install the mod and what mod they want - they only have to do this
- The program fetches the latest download link and version number from the github repo's API
- The zip is downloaded, extracted, and contents are set up in the folder without further input

## Usage
- Refer to the [User guide](https://github.com/whichtwix/Modinstaller/wiki/User-guide) to know what to do
- You will need to install this once and can reuse it every update apart from any breaking changes that may occur to the exe's functionality
