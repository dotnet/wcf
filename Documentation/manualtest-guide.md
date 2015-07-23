Manual Tests Instruction
========================
Test Name: WindowsAuthentication_RoundTrips_Echo

Set Up Requirement

1. Must run on a domain machine.

Steps

1. Enable the test
In src\System.Private.ServiceModel\tests\Scenarios\Security\TransportSecurity\Https\ClientCredentialTypeTests.cs,
Remove  [ActiveIssue(53)] above test method WindowsAuthentication_RoundTrips_Echo()
2. Go to the root of WCF source, run all scenario tests.
```
	     build /p:WithCategories=OuterLoop
```
