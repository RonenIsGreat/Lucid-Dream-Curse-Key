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
start /b mongod
@set ErrLevel=%errorlevel%
waitfor /T 5 A 
mongo records --eval "db.dropDatabase();"
@set /a "ErrLevel" = %ErrLevel%+%errorlevel%"
mongo records --eval "db.temp.insert({x:1})"
@set /a "ErrLevel" = %ErrLevel%+%errorlevel%"
taskkill /F /IM mongod.exe
@set /a "ErrLevel" = %ErrLevel%+%errorlevel%"
exit /b %ErrLevel%

:END