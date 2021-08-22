
## Balance the Loan Books

How to build and run
- Ensure that you have dotnet core CLI tools available
   - https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build   
- The `dotnet build` command builds the project and its dependencies into a set of binaries.
- The `dotnet run --project BalanceLoans` command will run the console application developed in the BalanceLoans project
- After running the application, the output files will be located at the following paths from the repository root
   - `BalanceLoans/CsvData/large/results/assignments.csv`
   - `BalanceLoans/CsvData/large/results/yields.csv`  
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
    > I completed the algorithm in ~3 hours as the project description had outlined. There was some time before this in setting up the repository and doing some basic research for the CSV library that would be best. 
    The trickiest part of the algorithm was related to navigating the covenants. I had to revisit my approach once I re-read the rules regarding covenant restrictions. The following two rules were the trickiest part of the algorithm implementation (though easy to implement after a second reading):  
    >- “If present, denotes that this covenant applies to the facility with this ID; otherwise, this covenant applies to all of the bank’s facilities.”  
    >- “If a row contains both a max_default_likelihood and a banned_state , they should be treated as separate covenants.”   
    >
    >My initial Instinct was to implement an algorithm that would identify a valid set of facility IDs directly from the covenants list, however I ultimately realized that this covenant list could really only provide constraints that disqualified certain facilities or banks based on the rules specified. This led me to an approach where I ended up using **disqualified** facility ids, and **disqualified** bank ids as a basis for identifying appropriate facilities based on what's left after removing these choices.

2. How would you modify your data model or code to account for an eventual introduction
   of new, as-of-yet unknown types of covenants, beyond just maximum default likelihood
   and state restrictions?
  
    > The data model provided to specify covenants relied on particular column definitions which rigidly specified maximum default likelihood and banned states  for a particular covenant:
    ```
    facility_id,max_default_likelihood,bank_id,banned_state  
    2,0.09,1,MT   
    1,0.06,2,VT   
    1,,2,CA
    ```
    >This schema is limited in the sense that any additional rules would need to be implemented as additional columns, which is not flexible for future introductions of “new, as-of-yet unknown types of covenants”. A more flexible model would be one in which we can support a list of rules, and new rules could simply be added as new rows to this collection. The design of the collection itself would specify a list of possible expressions such as the following new representation of the original covenants:
    ```
    facility_id,bank_id,field,expression,value
    2,1,state,NotEqual,MT
    2,1,default_likelihood,LessThanOrEqual,0.09
    1,2,state,NotEqual,VT
    1,2,default_likelihood,LessThanOrEqual,0.06
    1,2,state,NotEqual,CA`
    ```
    > In this way, we could easily introduce  new rules based on a predefined set of permissible relationships such as “NotEqual”, “Equal”, “GreaterThan”, “LessThan” etc. Another benefit of this, is that it can also accommodate new fields as the model grows in the future. This approach allows for some more sophisticated relationships as well, such as providing rules that specify both greater than and less than relationships for the same field, if such a constraint was desirable  for a particular facility.  
   https://stackoverflow.com/questions/6488034/how-to-implement-a-rule-engine  
   This could be elaborated even further through the support of a domain specific language which could be specified dynamically in the data storage, and then interpreted at runtime against the models being operated on. Using this approach would allow for more complete Expressions to be developed such as
    ```
    facility_id,bank_id,dsl_expression
    2,1,’($default_likelihood * 5) < (.5 * $interest_rate)’`
    ```
   
3. How would you architect your solution as a production service wherein new facilities can
   be introduced at arbitrary points in time. Assume these facilities become available by the
   finance team emailing your team and describing the addition with a new set of CSVs.

    > The solution that I provided obviously was not designed for this consideration. One of the main drawbacks is the fact that our current implementation maintains all of the current covenants, banks and facilities in memory, while processing all of the loan requests. This obviously does not accommodate any concurrency in behavior, and it also does not take into consideration any updates that are taking place to the underlying data. There are a broad range of potential challenges here, however we can limit our consideration for the moment to just the idea expressed in the question, namely the idea that we would be introducing new facilities at  arbitrary points in time.  
    >
    > The architecture for the addition of facilities at arbitrary points in time would  need to be designed based on the assumptions regarding the time frame in which we expect that new loans should be able to be fulfilled by the new facility being added.   Assuming that our users would intend for loans to get assigned to the new facility right away, it is therefore necessary that the addition of a new facility provides an indication to the runtime to “clear the runtime cache” of information regarding the current set of facilities, and have this refreshed in-memory to reflect the new facility that was added. Note: The running updates of amounts available for each facility are very sensitive pieces of information, and they should also be maintained in persistent storage via transactional controls to ensure that we do not lose any updates to the amounts being drawn upon from each facility at runtime. This would likely be achieved using the transactional capabilities of a SQL database choice on the back end, along with a write through type of caching approach for runtime updates to ensure durability for this critical data.  
    >
    > If the proposal was to email new facilities using attached csvs, I would immediately push back and propose a more robust solution than this, since this approach really has no controls over the integrity and security of the data or the workflow being followed for the process of communicating these facilities updates. Some type of secure application driven process  which allows only authenticated users to participate would be highly preferable. Assuming that the emailing of csvs is a requirement that cannot be changed, I would then ask for some clarification regarding the contents of the CSVs being sent. Do these csvs specify only new facilities, or may they sometimes specify repetitive information or updates to existing facilities?  Assuming some integrity has been established here, and we can be confident that these files would contain only new facilities with valid data, then I would seek to introduce integrity and security elements in the design going forward.  
    >
    > Based on these assumptions that have been established,  the design could involve a website in which an authenticated and authorized user uploads the CSV and then has an opportunity to review the contents on the screen before proceeding to submit this update to our system.  This part of the design does not require transaction controls since we are not yet making any changes to the runtime in production, and could even use a No-Sql store on the back end for this staging data. The implementation for this stage could involve a basic Angular website, with an API to upload the CSV file to an object store. The API would return a response as soon as this upload is complete. The upload would also trigger an event so that the CSV contents would get parsed and persisted to the data store in a more structured format. Once the user has an opportunity to review this information on the screen, this authorized user can then choose to submit these new facilities to the production run time system. This would then trigger an event with a payload containing all of the data for the new facility in Json format.
    >
    > The handling of  this event now needs to much more carefully respect the runtime considerations that were described earlier. Assuming that this is guaranteed to be a set of brand new facilities, then there is less chance for these changes to interfere with the current ongoing runtime execution (which is running against existing facilities). Once the new facility is added to the data store, we can send an event to alert the runtime systems that new data has been added. This should trigger a refresh of the in-memory data representation for the loan balancing logic, which should then include the new facilities as part of assignment operations going forward, and maintaining transactional updates to the amounts available in these new facilities, as was necessary for previous facilities being operated on up until this point.
      
4. Your solution most likely simulates the streaming process by directly calling a method in
   your code to process the loans inside of a for loop. What would a REST API look like for
   this same service? Stakeholders using the API will need, at a minimum, to be able to
   request a loan be assigned to a facility, and read the funding status of a loan, as well as
   query the capacities remaining in facilities.
    > The key difference here is that the concurrency of multiple API requests happening at the same time introduces the need for transactional controls around the handling of each loan request. This is a similar situation to the design controls that are required for a hotel reservation system:
   > 
   > https://www.youtube.com/watch?v=YyOXt2MEkv4&t=1570s  
   > 
   > In systems like these there is the opportunity to provide high performance, high concurrency data for reading, and providing quite reliable guidance at runtime, but this doesn’t completely guarantee the final result. The system design for reading can leverage caching layers and give users immediate feedback regarding the likely outcome for a new loan request, however it is not until that loan has been able to secure a transactional lock on the shared data that we can truly establish a final result for this loan. The updates to facilities amounts must be atomic (i.e. all or none changes to data), consistent (i.e. conforms to integrity rules for the system, and same data available to all), isolated (i.e. not affected by other read/write operations) and durable (i.e. can persist through system failure). 
   > 
   > In terms of API endpoints, it's best to design this in an asynchronous manner. As described in the question, we can provide end points for the following API behaviors:
   > 
   >- Request a loan be assigned to a facility (POST to /loan-requests)  
   >   - This will successfully submit the loan request but not yet guarantee an outcome
   >   -  As described before this will persist the request and submit an event for processing
   >- Read the funding status of a loan (GET from /loan-requests)
   >   -  This will read the collection of loan requests, which will be updated to maintain status of the event handling for the loan request
   >- The capacities remaining in facilities (GET from /facilities)
   >   -  This will likely be a read against a replica of the facilities data to avoid direct access to the transactional store for this information. May occasionally be out of date, but good enough for the purposes of this API.

5. How might you improve your assignment algorithm if you were permitted to assign loans
   in batch rather than streaming? We are not looking for code here, but pseudo code or
   description of a revised algorithm appreciated.
    >   When assigning loans in batch, it is possible to explore a wider variety of permutations regarding the ordering and assignment of loans. Our algorithm so far assumed a straightforward sequential determination of assignments, which did not consider that we might be able to further optimize our assignments to maximize yields by adjusting the ordering of all the loans being assigned. Considering our yield equation once again:  
    ```
    expected_yield =
    (1 - default_likelihood) * loan_interest_rate * amount
    - default_likelihood * amount
    - facility_interest_rate * amount
    ```
    > We can see that yield can be optimised through   
    > - Low default likelihood
    > - High loan interest rate
    > - High loan amount
    > - Low facility interest rate  
    >
    > Ultimately we should be able to consider every permutation of loan assignment possibilities to maximize the yield through the optimizations described above. This would involve ensuring that we prioritize finding facilities for those loans that have a low default likelihood, a high interest rate and a high loan amount. In a similar way, we will also seek to fully leverage the availability of facilities with the lowest interest rates, and develop an algorithm that will ensure that these facilities do not have any unused funding capacity left over after assignments have taken place.  
    >
    >A very basic approach that could target the requirements above would be the following enhancements in the development of the algorithm.  
   > 
   > First sort the group of incoming loans based on some expression involving the three elements of interest, for example we could use the yield formula itself to create a heuristic to represent the desirability of a particular incoming loan:
    >
    ```
    ((1 - default_likelihood) * loan_interest_rate * amount) 
    - (default_likelihood * amount)
    ```
    > Once we have established a prioritization of the incoming loans based on the heuristic above, we then handle each loan in order of this prioritization, and our existing algorithm will identify facilities that would offer the best facility interest rate for each loan.  
    > 
    > The approach above does guarantee that we handle the most desirable loans first, obtaining the best facility interest rates for these, however there may be further optimizations to the overall yield that could be discovered using more comprehensive techniques such as linear programming
    > 
   >- https://math.libretexts.org/Bookshelves/Applied_Mathematics/Applied_Finite_Mathematics_(Sekhon_and_Bloom)/04%3A_Linear_Programming_The_Simplex_Method/4.01%3A_Introduction_to_Linear_Programming_Applications_in_Business_Finance_Medicine_and_Social_Science
   > 
    >- https://byjus.com/linear-programming-calculator/  
    > 
   > As described on the textbook above, `Linear programming can be used as part of the process to determine the characteristics of the loan offer. The linear program seeks to maximize the profitability of its portfolio of loans. The constraints limit the risk that the customer will default and will not repay the loan. The constraints also seek to minimize the risk of losing the loan customer if the conditions of the loan are not favorable enough; otherwise the customer may find another lender, such as a bank, which can offer a more favorable loan.`
   > 
    > and rules engines.  
    >- https://inrule.com/resources/blog/loan-origination-and-business-rule-management-systems/ 
    >- https://medium.com/@er.rameshkatiyar/what-is-rule-engine-86ea759ad97d
   > 
    > The first link above describes how rules engines can play a role in simplifying the complex task of managing a large set of rules for a system like this.
   ```
   Determining Eligibility
   Many lenders offer up numerous products. Determining which products a 
   borrower is eligible for can involve thousands, if not tens of thousands,
   of rules. Having rule constructs like reusable vocabulary templates,
   decision tables, and business language rules makes the effort much more
   manageable.
   
   Pricing the Loan
   A number of factors go into the rate you receive for a loan. For example, 
   a lender establishes a base rate for (the loan). The rate
   increases or decreases based on your credit score, type of housing,
   secondary loan, loan-to-value, etc. These rules are well-suited 
   for a BRMS.
   ``` 

6. Discuss your solution’s runtime complexity.
    > The primary Loop in my solution is the following Loop over the set of incoming loans:  
    `foreach (Loan loan in inputModels.loans)`  
    > 
    > On its own, the complexity of this algorithm would be O(n), where n represents the number of loans. Most of the other operations against the input data leveraged the LINQ  capabilities of the C# language. The complexity underlying the operations using this Library can vary based on use case  as described in the following:  
    >- https://stackoverflow.com/questions/2799427/what-guarantees-are-there-on-the-run-time-complexity-big-o-of-linq-methods
    >
   > Most of the other operations taking place leverage filtering  based operations using this library.  As described above, most of these filtering operations could be optimized using a HashSet  data type to represent the collection in memory.  
   > In the worst-case, the order of this algorithm would be based on the fact that each loop must also loop through the complete set of covenants and facilities.  if we define variables for loans, covenants and facilities as follows:  
   > - L = number of loans
   > - C = number of covenants  
   > - F = number of facilities
   > 
   > This would give us a worst-case performance of:  
   > 
   > `O(L * (C + F))`  
   > 
   > (Note: The following video shows the basic rules I followed to establish this: https://www.youtube.com/watch?v=v4cd1O4zkGw)  
   > 
   > Assuming that our use of the library and hash sets can optimize access to items in the lists of covenants and facilities, this would mean that the order of the algorithm to access covenants would be improved from O(C) to O(1), and likewise for accessing facilities. Assuming optimal performance for LINQ access to hash sets, this would then mean that our algorithm performs closer to the best case of:  
   > 
   > `O(L)`  
   > 
   > As described in this response   
   > 
   > - https://stackoverflow.com/questions/9864777/what-is-the-big-o-of-linq-where  
   > 
   > ... the order of the where operation itself is O(1), however operating on the resulting objects, by using a ToList() method etc., brings the complexity back to O(N),  at least for the items contained in the result set.  Given the fact that most of my filtering operations using the where method also typically leveraged the ToList method, I expect that my implementation more closely approximates the worst-case performance described above. 


