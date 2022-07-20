# C Double-Flat
*My most monstrous creation yet.*
<p align="center">
  <img src="https://raw.githubusercontent.com/GitHababi/C-Double-Flat/main/assets/cbb_logo.png" alt="C-Double Flat" width="300"/>
</p>
This is a programming language that was first intended to be esoteric, but ended up actually usable (a bit).
I'll continue to update this so long as I am not bored and or dead. Soon, I will be creating documentation on this very Github page.
Release 2.1.0 has been released, please submit bug reports so I know why I'm dumb.

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

