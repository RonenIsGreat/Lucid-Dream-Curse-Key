@ECHO OFF

:cond1
:: check if mongod already running

echo --- Checking the MongoDB in path...
:: MongoDB in path:
echo|set /p=------ MongoDB:
mongod -help >nul 2>&1
if %errorlevel% == 9009 (
   echo MongoDB doesn't exist in Path.
   goto :error
)
echo ok

:Start
mongod
:END