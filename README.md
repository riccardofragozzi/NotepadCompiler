# NotepadCompiler

NotepadCompiler (or NPC), by Riccardo Fragozzi.

Use this tool for university exams. How to use:
	--NOTEPAD INTEGRATED EDITOR
	In this mode you can use notepad, and NPC will add features.
	- Open NotepadCompiler and make sure fake touchpad icon appears on windows bottom right notify area.
	- Press ALT+N to run notepad.exe
	- Notepad will open a new file stored on "C:\NPC\Senza titolo", to simulate a not saved file on notepad title.
	- Type code on notepad editor, then save it (CTRL+S)
	- Press ALT+S to compile source code. Watch fake mouse icon in windows notify area: Yellow=compiling, Red=compile error, Green=compile ok.
	- Press CTRL+V to paste your source code output(or compile errors), or show it on your phone(read XAMPP integration)
	
	--CLIPBOARDS
	In this mode you can use all editors, not only notepad.
	- Open NotepadCompiler and make sure fake touchpad icon appears on windows bottom right notify area.
	- Type code on notepad editor, then copy it to clipboards (CTRL+C)
	- Press ALT+C to compile source code. Watch fake mouse icon in windows notify area: Yellow=compiling, Red=compile error, Green=compile ok.
	- Press CTRL+V to paste your source code output(or compile errors), or show it on your phone(read XAMPP integration)

Note:
	- During the compilation process, if enabled(by default is enabled, you can disable by pressing ALT+T), notepad.exe and Chrome.exe will become topmost.
	  This is to hide some compiler command prompts appearing on top of screen. Make sure notepad anche chrome are maximized.
	- On notepad, the char sequence "----------" will be replaced with "int main(){"
	- On notepad, all libs will automatically added(stdio, stdlib, stddef, string)
Other commands:
	- ALT+X --> Leave program. You must press ALT+X 5 times consecutively.
	- ALT+V --> Before to press CTRL+V, ALT+V applies some replacements on clipboards, ex. "in t" --> "int", "#inc lude" --> "#inc lude", ecc...
	- ALT+T --> Enable/Disable compilation topmost for chrome and notepad. If enabled, when press ALT+T NPC icon becomes green for 1 second, else, NPC icon is red for 1 second.


Install instructions:
	- Install MinGW on C:\MinGW\ folder. Make sure gcc is available.
	- Create new folder: "C:\NPC\"
	- To run NotepadCompiler is necessary .NET Framework 4.6. Install it if not yet installed.
	- Run NotepadCompiler.
	
XAMPP integration:
	By integrating XAMPP, any compilation or executable output can be transmitted to another device.
	- Install XAMPP, and activate Apache from XAMPP control panel
	- Create folder [your XAMPP directory]\htdocs\NPC\
	- Create new file C:\NPC\NPC_configuration.txt and write "network_output_dir:[your XAMPP directory]\htdocs\NPC\"
	- Discover your ip(right click on NPC status icon > Info > IP Address)
	- Type your ip followed by "/npc" on your phone browser, that must be connected to the same PC network. Example: "192.168.1.1/npc"
	- You shold see the NPC page.
	- Any compilation output result or status will appear inside this page.
	
	
	