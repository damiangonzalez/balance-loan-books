##Balance the Loan Books

How to build and run
- Ensure that you have dotnet core CLI tools available
   - https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build   
- The `dotnet build` command builds the project and its dependencies into a set of binaries.
- The `dotnet run --project BalanceLoans` command will run the console application developed in the BalanceLoans project
- After running the application, the output files will be located at the following paths from the repository root
   - `BalanceLoans/CsvData/large/results/assignments.csv`
   - `BalanceLoans/CsvData/large/results/yields.csv`  
- Results obtained from earlier execution can be found here
    - `BalanceLoans/CsvData/large/resultsReference/assignments.csv`
    - `BalanceLoans/CsvData/large/resultsReference/yields.csv`
- Sample execution below  
    ```
    damiangonzalez@Damians-MacBook-Pro balance-loan-books % dotnet build                     
    Microsoft (R) Build Engine version 16.9.0+57a23d249 for .NET
    Copyright (C) Microsoft Corporation. All rights reserved.
    
    Determining projects to restore...
    All projects are up-to-date for restore.
    BalanceLoans -> /Users/damiangonzalez/GitHub/balance-loan-books/BalanceLoans/bin/Debug/net5.0/BalanceLoans.dll
    BalanceLoansTest -> /Users/damiangonzalez/GitHub/balance-loan-books/BalanceLoansTest/bin/Debug/net5.0/BalanceLoansTest.dll
    
    Build succeeded.
    0 Warning(s)
    0 Error(s)
    
    Time Elapsed 00:00:02.11
    damiangonzalez@Damians-MacBook-Pro balance-loan-books % dotnet run --project BalanceLoans
    Assignments CSV can be found here: /Users/damiangonzalez/GitHub/balance-loan-books/BalanceLoans/CsvData/large/results/assignments.csv
    Yields CSV can be found here: /Users/damiangonzalez/GitHub/balance-loan-books/BalanceLoans/CsvData/large/results/yields.csv
    damiangonzalez@Damians-MacBook-Pro balance-loan-books %
    ```
1. How long did you spend working on the problem? What did you find to be the most
   difficult part?
    ```
   I completed the algorithm in ~3 hours as expected
   The trickiest part of the algorithm was related to navigating the covenants.
   I had to revisit my approach once I re-read the rules regarding covenant restrictions.
   This led me to an approach where I ended up using *disqualified* facility ids, 
   and *disqualified* bank ids as a basis for identifying appropriate facilities.
    ```   

2. How would you modify your data model or code to account for an eventual introduction
   of new, as-of-yet unknown types of covenants, beyond just maximum default likelihood
   and state restrictions?
    ```
    This is an interesting question. The problem described here lends itself to a "Rules Engine" approach, for which
   there are many sophisticated solutions available.
   
   https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine
   
   Also DSL approach?    
    ```   
3. How would you architect your solution as a production service wherein new facilities can
   be introduced at arbitrary points in time. Assume these facilities become available by the
   finance team emailing your team and describing the addition with a new set of CSVs.
    ```
    
    ```   
4. Your solution most likely simulates the streaming process by directly calling a method in
   your code to process the loans inside of a for loop. What would a REST API look like for
   this same service? Stakeholders using the API will need, at a minimum, to be able to
   request a loan be assigned to a facility, and read the funding status of a loan, as well as
   query the capacities remaining in facilities.
    ```
    
    ```   
5. How might you improve your assignment algorithm if you were permitted to assign loans
   in batch rather than streaming? We are not looking for code here, but pseudo code or
   description of a revised algorithm appreciated.
    ```
    
    ```   
6. Discuss your solution’s runtime complexity.
    ```
https://stackoverflow.com/questions/2799427/what-guarantees-are-there-on-the-run-time-complexity-big-o-of-linq-methods    
    ```   
