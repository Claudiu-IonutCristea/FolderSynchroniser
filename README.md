# File synchronisation tool
### A program that periodically synchronises the files and folders from a SOURCE folder to a REPLICA folder

## Program Input
The program expects command line arguments before it is run in order to set the following settings for the program:
* Path to source folder
* Path to replica folder
* Path to log file
* Sync interval

Each argument is preceded by a command that starts with the character '-', the commands are **not** case sensitive. If a command is used, the default value for that setting will be used.  
The program accepts both relative and absolute paths
<br/><br/>

### Path to source folder (-s [path] OR -source [path])
###### DEFAULT: "./source"
Sets the path of the folder that the files will be copied **from**.
<br/><br/>

### Path to replica folder (-r [path] OR -replica [path])
###### DEFAULT: "./replica"
Sets the path of the folder that the files will be copied **to**.
<br/><br/>

### Path to log file (-l [path] OR -log [path])
###### DEFAULT: "./log.txt"
Sets the path for the file that will log the modified files during synchronisation.  
only supported extension is ".txt". If the path does not end with an extension, it will be treated as a directory path and will add a log.txt file to that directory.
<br/><br/>

### Sync interval (-i [timeInterval] OR -sync [timeInterval]
###### DEFAULT: "10s"
Sets the interval at which the app ensures that the replica folder is the same as the source folder.
interval can be expressed id days, hours, minutes, seconds, and supports the following formats:  

| | | |
| --- | --- | --- |
| 0s | 0m | 0h |
| 0h0m | 0h0s | 0m0s |
| 0h0m0s | 0d | 0d0h |
| 0d0m | 0d0s | 0d0h0m |
| 0d0h0s | 0d0m0s | 0d0h0m0s |  

> 0 represents an integer value.  

d - days (any positive value)  
h - hours (0 - 23)  
m - minutes (0 - 59)  
s - seconds (0 - 59)  
<br/>
h, m and s can only have 1 leading zero! (01h05m is accepted, but 001h will return error 201)

### Example arguments
![image_2023-03-07_024028879](https://user-images.githubusercontent.com/111143114/223289000-145f0425-b1ed-4b6b-8ff1-b0dd38bfe9b1.png)  
<br/><br/>

## Errors
If there are any errors in the parsing of the CLI arguments the app will not start and will list the source and reason for the errors. The app also exists with the error code of the first error in the list.
| Error code | Reason |
| --- | --- |
| 101 | Length of path # is invalid |
| 102 | Drive letter # is invalid |
| 103 | Drive # does not exist on this devide |
| 104 | Path # contains an invalid character |
| 201 | Sync period invalid format |
| 301 | Invalid command line argument format! (#) Commands have to start with "-" |
| 302 | Source folder and Replica folder cannot be the same |
| 303 | Command # not recognised |
| 304 | Extension # is not supported for the log file. Use ".txt" instead |
> char # is replaced by the source of the error

<br/>

![image_2023-03-07_030556951](https://user-images.githubusercontent.com/111143114/223292544-d9dcbcf7-8c33-4cb4-bfa7-54fa91905dbe.png)
> In this case the program exited with code 201

<br/>

## Known issues
* If a command is passed twice, the value from the last command will be used
