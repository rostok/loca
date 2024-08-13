# LOCA
locate for Windows NTFS drives

# Purpose
LOCA allows you to quickly search for files by its name and path. It first creates dump of all files and folders on a drive and store this in a txt file. Dump is created using [NtfsReader](https://sourceforge.net/projects/ntfsreader/) library that parses NTFS partition in memory instead of lenghty directory crawl using system calls. A 512GB SSD disk should be read in couple of seconds. 

Once the index is created and stored in NtfsReader.dll C:\Users\{username}\.loca it is parsed with paths displayed that contain ALL the arguments passed.

# Building
Just run compile.bat

# Installation
Put exe and dll in PATH