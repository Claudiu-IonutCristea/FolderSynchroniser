# File synchronisation tool
## A program that periodically synchronises the files and folders from a SOURCE folder to a REPLICA folder

### Program Input
The program expects command line arguments before it is run in order to set the following settings for the program:
* Path to source folder
* Path to replica folder
* Path to log file
* Sync interval

Each argument is preceded by a command that starts with the character '-', the commands are **not** case sensitive.  
The program accepts both relative and absolute paths

### Path to source folder (-s [path] OR -source [path])
###### DEFAULT: "./source"
Sets the path of the folder that the files will be copied **from**.

### Path to replica folder (-r [path] OR -replica [path])
###### DEFAULT: "./replica"
Sets the path of the folder that the files will be copied **to**.

### Path to log file (-l [path] OR -log [path])
###### DEFAULT: "./log.txt"
Sets the path for the file that will log the modified files during synchronisation.  
only supported extension is ".txt". If the path does not end with an extension, it will be treated as a directory path and will add a log.txt file to that directory.

### Sync interval (-i [timeInterval] OR -sync [timeInterval]
###### DEFAULT: "10s"
Sets the interval at which the app ensures that the replica folder is the same as the source folder.
interval can be expressed id days, hours, minutes, seconds, and supports the following formats:
| --- | --- | --- |
| 0s | 0m | 0h |
| 0h0m | 0h0s | 0m0s |
| 0h0m0s | 0d | 0d0h |
| 0d0m | 0d0s | 0d0h0m |
| 0d0h0s | 0d0m0s | 0d0h0m0s |
