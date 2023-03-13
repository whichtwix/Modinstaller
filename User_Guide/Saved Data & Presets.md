The program saves data at ```%Appdata%\Roaming\Modinstaller```

There are 2 things saved:

- A txt file saves any errors that are encountered during use of the exe when it is opened. When you reopen it the file will be wiped. If you encounter one and create a issue, reproduce the problem and send the log there.
- A json file stores presets that you create from the program for use in installing.

The json has the following structure:
```json
[
  {
    "Mod": "mod",
    "BaseFolder": "original folder",
    "DestinationFolder": "folder for mod"
  },
  {
    "Mod": "..",
    "BaseFolder": "..",
    "DestinationFolder": ".."
  }
]
```

Utilising the json file, you can install multiple mods at the same time provided the destination folder isnt the same for 2 or more mods, or  basefolders if the destination folder isnt provided. Additionally, installing a mod becomes easier as you can go through step 1 of installing mods once and skip it from now on.

Do not attempt to edit the file yourself as the file may become unusable by the program. Use the add and delete functions from within the program.
