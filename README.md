# Project Title

Inventory system

## Description

A project demonstrating WCF integration with WPF. The WPF project was implemented in an MVVM manner using Prism library. AdonisUI was used for theming and Polly was used for retries on the client end when connecting to the server.

## List of projects

### InventoryClient.Common

* A class library that contains all the models and events as well as service interfaces used in the client WPF application

### InventoryClient.Inventory

* A module that handles everything related to inventory.

### InventoryClient.Orders

* A module that handles everything related to orders. Multiple order windows can launched and uses the OrderService to manage orders

### InventoryClient.Wpf

* This is the WPF project that acts as the main entry point to the application. Uses Prism

### InventoryServiceHost

* A console project that serves as a host for the WCF services simulating a server

### InventoryServiceLibrary

* A WCF class library project that houses all the WCF definitions and configuration

### InventoryServiceTest

* Unit test and integration tests for the WCF services.

### TestApp

* A console project app to sample and play with the WCF services

### ListRandomNumbersAndSQLTest

* A console project app to demonstrate an algorithm based on Fisher–Yates method to randomly generate and shuffle N integers in a list. Also contains a modest Transact SQL script for comparing the contents of two tables

## How to Run

Debug the solution or Debug the following projects in any order. 
* InventoryServiceHost
* InventoryClient.Wpf





