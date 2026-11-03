# DVMS HexButtonBar

HexButtonBar is the honeycomb presentation of the DVMS desktop launcher. It
shares the launcher, sound, SQLite, history, process tracking, and file activity
implementation with ButtonBar while remaining a separate executable with its
own database and window settings.

Buttons use a point-up hexagonal visible and clickable region, with vertical
left/right edges. Adjacent buttons share edges, and alternate rows shift by
half a hexagon to nest into the row above as the window is resized. The menu,
locking, editing, cloning, drag-and-drop, sound-only actions, always-on-top,
launch history, and monitoring features behave like ButtonBar.

Data is stored in `%LOCALAPPDATA%\DVMS\HexButtonBar\button-bar.db`.
