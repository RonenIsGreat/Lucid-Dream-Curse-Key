@ECHO OFF

set restheartPath=%restheart%

echo --- Checking the directory parameter...
:: MongoDB in path:
if not exist "%restheartPath%" (
   echo given directory does not exist on the filesystem
   goto :END
)
if not exist "%restheartPath%/restheart.jar"(
   echo restheart.jar file does not exist in folder
   goto :END
) 
echo ok

echo --- Checking the java in path...
:: java in path:
echo|set /p=------ Java:
java -help >nul 2>&1
if %errorlevel% == 9009 (
   echo Java doesn't exist in Path.
   goto :error
)
echo ok

:Start
java -Dfile.encoding=UTF-8 -server -jar "%restheartPath%/restheart.jar" "%restheartPath%/etc/restheart.yml"
GOTO End

:END