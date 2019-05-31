@echo off
powershell -ExecutionPolicy ByPass -NoProfile -command "& """%~dp0Build.ps1""" -restore -build -test -integrationTest -pack -publish -ci %*"
