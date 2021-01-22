# NotepadCompiler

Use this tool for university exams. How to use:

	--NOTEPAD INTEGRATED EDITOR
	In this mode you can use notepad, and NotepadCompiler will add features.
	- Open NotepadCompiler and make sure fake touchpad icon appears on windows bottom right notify area.
	- Press ALT+N to run notepad.exe
	- Notepad will open a new file stored on "C:\NPC\Senza titolo", to simulate a not saved file on notepad title.
	- Type code on notepad editor, then save it (CTRL+S)
	- Press ALT+S to compile source code. Watch fake mouse icon in windows notify area: Yellow=compiling, Red=compile error, Green=compile ok.
	- During the compilation process notepad will become topmost. This is to hide some compiler command prompts appearing on top of screen. Make sure notepad is maximized.
	- Press CTRL+V to paste your source code output(or compile errors)

	--CLIPBOARDS
	In this mode you can use all editors, not only notepad.
	- Open NotepadCompiler and make sure fake touchpad icon appears on windows bottom right notify area.
	- Type code on notepad editor, then copy it to clipboards (CTRL+C)
	- Press ALT+C to compile source code. Watch fake mouse icon in windows notify area: Yellow=compiling, Red=compile error, Green=compile ok.
	- Press CTRL+V to paste your source code output(or compile errors)


Other commands:
	- ALT+X --> Leave program. You must press ALT+X 5 times consecutively.


Install instructions:
	- Install MinGW on C:\MinGW\ folder. Make sure gcc is available.
	- Create new folder: "C:\NPC\"
	- To run NotepadCompiler is necessary .NET Framework 4.6. Install it if not yet installed.
	- Run NotepadCompiler.