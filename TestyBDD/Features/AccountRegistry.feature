Feature: Account registry
	Scenario: 01. User is able to create a new account
		Given Number of accounts in registry equals: 0
		When I create an account using name: kurt, last name: cobain, pesel: 89092909876
		Then Number of accounts in registry equals: 1
		And Account with pesel 89092909876 exists in registry

	Scenario: 02. User is able to create a second account
		Given Number of accounts in registry equals: 1
		When I create an account using name: Janusz, last name: Kowalski, pesel: 12345678903
		Then Number of accounts in registry equals: 2
		And Account with pesel 12345678903 exists in registry

	Scenario: 03. User is able to update name of already created account
		Given Account with pesel 89092909876 exists in registry
		When I update Imie of account with pesel: 89092909876 to russell
		Then Account with pesel 89092909876 has Imie equal to russell

	Scenario: 04. User is able to update surname of already created account
		Given Account with pesel 12345678903 exists in registry
		When I update Nazwisko of account with pesel: 12345678903 to Wajda
		Then Account with pesel 12345678903 has Nazwisko equal to Wajda

	Scenario: 05. User is able to delete already created account
		Given Account with pesel 89092909876 exists in registry
		When I delete account with pesel: 89092909876
		Then Account with pesel 89092909876 does not exist in registry
		And Number of accounts in registry equals: 1

	Scenario: 06. User is able to delete last account
		Given Account with pesel 12345678903 exists in registry
		When I delete account with pesel: 12345678903
		Then Account with pesel 12345678903 does not exist in registry
		And Number of accounts in registry equals: 0