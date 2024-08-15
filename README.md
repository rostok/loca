# LOCA
locate for Windows NTFS drives

# Purpose
LOCA allows you to quickly search for files by its name and path. It first creates dump of all files and folders on a drive and store this in a txt file. Dump is created using [NtfsReader](https://sourceforge.net/projects/ntfsreader/) library that parses NTFS partition in memory instead of lenghty directory crawl using system calls. A 512GB SSD disk should be read in couple of seconds. 

Once the index is created and stored in C:\Users\\{username}\\.loca folder it is parsed and files with paths containing ALL the arguments passed are displayed.

# Building
Just run compile.bat

# Installation
Put exe and dll in PATH

# Help
```
Usage: loca [drive] [options]                                                                
Options:                                                                                     
  --help                  Display this help message and show index information.              
  --index [drive]         Index all files and folders in the specified drive.                
                          If no drive is specified, the current drive is used.               
  [filters]               Use arguments as filters to display files containing all terms.    
                          Example: loca --index C | loca some important file                 
```

# License 
MIT with additional condition that you can't use this for anything concerning military.
