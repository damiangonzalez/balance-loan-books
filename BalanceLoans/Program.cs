using System;

namespace BalanceLoans
{
    static class Program
    {
        static void Main()
        {
            Source.Utils.CsvHelper csvHelper = new Source.Utils.CsvHelper();
            BalanceLoansOrchestrator.ExecuteLoanBalancingSequence(csvHelper);
            
            Console.WriteLine($"Assignments CSV can be found here: " + csvHelper.GetAssignmentsCsvPath());
            Console.WriteLine($"Yields CSV can be found here: " + csvHelper.GetYieldsCsvPath());
        }
    }
}