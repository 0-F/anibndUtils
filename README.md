# anibndUtils
Tool for the *.anibnd.dcx in Elden Ring.

## Compilation
Install Docker https://www.docker.com/
```
git clone --recurse-submodules https://github.com/0-F/anibndUtils.git
cd anibndUtils
.\publish.ps1
```

## Usage
```
Usage: anibndUtils [ compare | filter | index ]

anibndUtils compare <file1.anibnd.dcx> <file2.anibnd.dcx>
anibndUtils filter <file.anibnd.dcx> [ regex_filter ]
anibndUtils index <file.anibnd.dcx>

Examples
- Compare `c0000.anibnd.dcx` and `mod\c0000.anibnd.dcx` files:
anibndUtils compare c0000.anibnd.dcx mod\c0000.anibnd.dcx
- Get all events with jump_table_id=12 (kill character) in c0000.anibnd.dcx:
anibndUtils filter c0000.anibnd.dcx 12
- Get all events with jump_table_id=12 or jump_table_id=20 in c0000.anibnd.dcx:
anibndUtils filter c0000.anibnd.dcx "12|20"
```
