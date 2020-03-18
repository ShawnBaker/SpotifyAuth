@echo off

set DEL_COMMAND=del /q /a:
if "%@eval[2 + 2]%" == "4" set DEL_COMMAND=%DEL_COMMAND% /e

for /R %%x in (obj) do if exist %%x rd /s /q %%x
for /R %%x in (bin) do if exist %%x rd /s /q %%x
for /R %%x in (.vs) do if exist %%x rd /s /q %%x

%DEL_COMMAND% /s *.user
