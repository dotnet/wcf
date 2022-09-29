:: Build tests on Docker host machine. 

:: Windows Release Build
call ..\..\..\..\eng\common\cibuild.cmd -configuration Release -prepareMachine /p:Root_Certificate_Installed=true /p:Client_Certificate_Installed=true /p:SSL_Available=true /p:Test=false




