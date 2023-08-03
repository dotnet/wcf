:: Build tools on Docker host machine. 
:: This saves time during container startup and running tests (simply mount a volume for the build artifacts) 

:: Build certifcate generator tool
call BuildCertUtil.cmd
:: Build Self Hosted service
call BuildWCFSelfHostedService.cmd
