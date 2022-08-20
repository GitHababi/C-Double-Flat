# C Double-Flat Graphics
*An extension to an already hellish creation*
<p align="center">
  <img src="https://raw.githubusercontent.com/GitHababi/C-Double-Flat/main/assets/cbb_logo.png" alt="C-Double Flat" width="300"/>
</p>
This is a programming language that was first intended to be esoteric, but ended up actually usable (a bit).
I'll continue to update this so long as I am not bored and or dead. Soon, I will be creating documentation on this very Github page.
Release 2.1.0 has been released, please submit bug reports so I know why I'm dumb.

## Attribution
Lots of code here has originally been written by members of the Raylib-cs project, which can be found [here](https://github.com/ChrisDill/Raylib-cs). Mainly structs and interops
*Copyleft Hababisoft Corporation, 2022. All rights unreserved.*\
*You could copy this code, but why would you? This sucks.*

## Use and installation

To use and install this library first, load the C Double Flat Project, build this library (no need to publish), then find the .dll file it creates, and you can use it.
There is also a Graphics.dll library included in the releases tab. You will also need to install raylib. Do this by downloading either [x86](https://github.com/raysan5/raylib/releases/download/4.2.0/raylib-4.2.0_win32_msvc16.zip) or [x64](https://github.com/raysan5/raylib/releases/download/4.2.0/raylib-4.2.0_win64_msvc16.zip) (depending on your system) and extracting the zip file.
If you want to use this library at startup, place it in the lib/ directory of your C Double Flat application. 
In addition, you must also add the raylib.dll file to the root directory of the Cbb application, (i.e. where the executable is). 
If you wish to dynamically load this library, place the raylib.dll file in the same directory as the Graphics dll.