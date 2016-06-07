@if "%_echo%" neq "on" echo off

:: To run tests outside of MSBuild.exe
:: %1 is the path to the tests\<OSConfig> folder

pushd %1

FOR /D %%F IN (*.Tests) DO (
	IF EXIST %%F\netcoreapp1.0 (
		pushd %%F\netcoreapp1.0
		@echo "corerun.exe xunit.console.netcore.exe %%F.dll -xml testResults.xml -notrait category=outerloop -notrait category=failing -notrait category=nonwindowstests -notrait Benchmark=true -notrait category=IgnoreForCI"
		corerun.exe xunit.console.netcore.exe %%F.dll -xml testResults.xml -notrait category=outerloop -notrait category=failing -notrait category=nonwindowstests -notrait Benchmark=true -notrait category=IgnoreForCI
		popd
	)
)

popd
