# BrassLoon

Developing a reusable logging plattiform and exploring the realm of hosted software.  The goal
is to setup a web service to store and retrieve logging data.  Additionaly, anyone should be
able to login to a web application and setup logging for any application.

Notice, this is in early stages, proofing, unsecured, and untested.

BrassLoon is a collection of utilitarian components. The goals is to create web API's to 
provide some low level functionality needed to develop web API's. These API's provide authorization, 
logging, and configuration functionality. The repository also includes an account API. The account 
API organizes data into domains and controls access to said domains.

## Authorization

The authorization API focuses on creating signed Json web tokens. It defines users based on 
Google authentication tokens. Roles can be created and assigned to said users. A user with a valid 
Google authentication token can request an access token in the form of a signed JWT with assigned 
roles. That token can be used to authorize actions on other web services.

## Logging

The logging API provides a place to stored error and metric data.

I intend to add trace logging functionality. I also intend to implement Microsoft.Extensions.Logging

## Configuration

The configuration API is used to store unstructured data. The data is saved and retrieved using a code.

## Account

The account API controls access to the other API's. It also organizes data into domains.
