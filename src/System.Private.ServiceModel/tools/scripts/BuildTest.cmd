:: Build tests
::Windows Release Build
call ..\..\..\..\eng\common\cibuild.cmd -configuration Release -prepareMachine /p:Root_Certificate_Installed=true /p:Client_Certificate_Installed=true /p:SSL_Available=true /p:Test=false

::Linux Build - not used
::call eng\common\cibuild.sh -configuration Release -preparemachine /p:Root_Certificate_Installed=true /p:Client_Certificate_Installed=true /p:SSL_Available=true /p:Test=false

