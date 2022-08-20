# C Double-Flat
<p align="center">
  <i>My most monstrous creation yet.</i> <br>
  <img src="https://raw.githubusercontent.com/GitHababi/C-Double-Flat/main/assets/cbb_logo.png" alt="C-Double Flat" width="300"/>
</p>

This is a programming language that was first intended to be esoteric, but ended up actually usable (a bit).
I'll continue to update this so long as I am not bored and or dead. Go to the [Releases](https://github.com/GitHababi/C-Double-Flat/releases) tab to download the latest .exe installer for end users. Use the `cbb` command in command prompt or PowerShell as a shortcut. Check out the [Wiki](https://github.com/GitHababi/C-Double-Flat/wiki) for more info on C Double Flat and its inner workings.

## Project Structure
- `C-Double-Flat` contains all the core project files that are necessary for cbb to run. Import this as a dependency when trying to make custom libraries.
  - `Core` core to the running of C Double Flat.
    - `Parser` deals with the conversion of strings of characters into ASTs and syntax.
    - `Runtime` deals with the actual running of C Double Flat as a program, which is ran by using generated items from the `Parser` namespace into it.
    - `Utilities` has all the necessary boilerplate to make Variables, Libraries, Functions, and ASTs work.

- `C-Double-Flat.App` is the project where the actual cbb application that runs is generated. Depends on the other two projects.
- `C-Double-Flat.StandardLibrary` is the standard library of functions that are usable in default running of the `C-Double-Flat.App` program.
- `C-Double-Flat.Graphics` is a library that utilizes raylib and parts of code from [Raylib-cs](https://github.com/ChrisDill/Raylib-cs) to bring fully functional 2D graphics to cbb.

*Copyleft Hababisoft Corporation, 2022. All rights unreserved.*\
*You could copy this code, but why would you? This sucks.*

