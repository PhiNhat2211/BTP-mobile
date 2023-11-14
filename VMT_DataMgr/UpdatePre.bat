REM - Add copy command to replace modified Header files of this libraries
REM - it have to copy to MLSDK default include directory to use them correctly
REM - 
REM -  ex) copy mlmmctrl.h ..\..\include
REM - 



REM ========================================================
REM = UPDATE HEADER FILES
REM ========================================================




IF "%1" == "/d" GOTO DEBUG
IF "%1" == "/r" GOTO RELEASE
GOTO END

REM ========================================================
REM = UPDATE DEBUG BUILD OUTPUT
REM ========================================================
:DEBUG
REM = TODO =

GOTO END



REM ========================================================
REM = UPDATE RELEASE BUILD OUTPUT
REM ========================================================
:RELEASE

REM = TODO =
GOTO END

:END