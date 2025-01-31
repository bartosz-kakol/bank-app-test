Feature: Transfers
	Scenario: 01. User is able to make incoming transfer
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		When User with PESEL: 12345678903 does a transfer of type incoming for 50 PLN
		Then The last transfer reponse has a status code of 200
		And User with PESEL: 12345678903 has a history: [50]
		And User with PESEL: 12345678903 has 50 PLN in their account

	Scenario: 02. User is able to make outgoing transfer
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		And User with PESEL: 12345678903 gets 50 PLN in their account
		When User with PESEL: 12345678903 does a transfer of type outgoing for 50 PLN
		Then The last transfer reponse has a status code of 200
		And User with PESEL: 12345678903 has a history: [50, -50]
		And User with PESEL: 12345678903 has 0 PLN in their account

	Scenario: 03. User makes invalid outgoing transfer
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		And User with PESEL: 12345678903 gets 30 PLN in their account
		When User with PESEL: 12345678903 does a transfer of type outgoing for 50 PLN
		Then The last transfer reponse has a status code of 422
		And User with PESEL: 12345678903 has a history: [30]
		And User with PESEL: 12345678903 has 30 PLN in their account

	Scenario: 04. User is able to make express transfer
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		And User with PESEL: 12345678903 gets 50 PLN in their account
		When User with PESEL: 12345678903 does a transfer of type express for 50 PLN
		Then The last transfer reponse has a status code of 200
		And User with PESEL: 12345678903 has a history: [50, -50, -1]
		And User with PESEL: 12345678903 has -1 PLN in their account

	Scenario: 05. User makes invalid express transfer
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		And User with PESEL: 12345678903 gets 30 PLN in their account
		When User with PESEL: 12345678903 does a transfer of type express for 50 PLN
		Then The last transfer reponse has a status code of 422
		And User with PESEL: 12345678903 has a history: [30]
		And User with PESEL: 12345678903 has 30 PLN in their account

	Scenario: 06. Invalid user makes incoming transfer
		When User with PESEL: 12345678904 does a transfer of type incoming for 50 PLN
		Then The last transfer reponse has a status code of 404

	Scenario: 07. Invalid user makes outgoing transfer
		When User with PESEL: 12345678904 does a transfer of type outgoing for 50 PLN
		Then The last transfer reponse has a status code of 404

	Scenario: 08. Invalid user makes express transfer
		When User with PESEL: 12345678904 does a transfer of type express for 50 PLN
		Then The last transfer reponse has a status code of 404

	Scenario: 09. User makes transfer of invalid type
		Given User with name: Dariusz, last name: Januszewski and PESEL: 12345678903 exists and has a clean state
		When User with PESEL: 12345678903 does a transfer of type dfsfad for 50 PLN
		Then The last transfer reponse has a status code of 400
		And User with PESEL: 12345678903 has a history: []
		And User with PESEL: 12345678903 has 0 PLN in their account
		
