# Windows Packager - wpkg
A kind of dpkg-deb for Windows, completely written in C#.

## Introduction
If you own a device running iOS and it's jailbroken, you'll know that additional software ("Tweak" or "Theme") is distributed through iOS specific APT repositories and are packaged as `.deb` files.

Unless you work within WSL, managing debian packages is a breeze. The only downside to WSL are its permission issues.

I wanted a WSL-free solution and that's how wpkg was born: A pain free dpkg-deb, completely written in C#, for the platform everyone is familiar with.

___DISCLAIMER: It's not entirely dpkg-deb, to be clear.___ This utility was mainly designed to help and make (jailbroken) iOS development easier on Windows. However, you can still use it however you may please!

__PLEASE READ THROUGH THE WIKI (link here) TO UNDERSTAND ITS TRIGGERS__

## Features
- Creation of `.deb` files
- Extraction of `.deb` files
- Custom triggers
	- `--theme` to quickly create a skeleton base for creating themes
	- More can be added to the official wpkg! (To request, feel free to create an issue or just recompile it yourself!)

## Installation Guide
*--This is the recommended method so you can call it from anywhere. Else you'd have to add it to the Windows folder directly so it's visible globally--*

1. Open a new Explorer Window, navigate to your Windows boot drive *(commonly "C")* and create a folder titled "_mytools". (The name is an arbitrary choice, just for good measure we keep it clear)
	- The full path at the end should look like `C:\_mytools`
2. Open the Start Menu and searh for `env`. It should suggest editing the environment variables.
3. Once the Menu opens, select `Environment variables`.
4. Under System variables, search for `PATH` and edit it.
5. Add a new Entry
	- An Entry in the `PATH` is always a path, so here our entry is `C:\_mytools`
6. Now that we have added our folder to the `PATH`, you can now just download it, move our "_mytools" folder and it's now accessible from the commandline like any other tool!
7. Profit??

## Download
__! I CANNOT STRESS THIS ENOUGH: PLEASE READ THE WIKI IF YOU HAVE ANY ISSUES/ TROUBLES FIGURING OUT HOW THIS WORKS !__

Link (stable release): (reserved space for link)

## Thanks/ Credits
- F. Carlier for dotnet-packaging - This was a tremendously good resource to study!
- OpenGroup for the gool 'ol `.ar` and the necessary header
- MSDN for providing the best C# support
- ICShapCode for SharpZipLib - you guys rock!

## License
This project is licensed under MIT.
