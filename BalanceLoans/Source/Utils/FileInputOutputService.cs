using System.Collections.Generic;
using BalanceLoans.Source.Models;
using BalanceLoans.Source.Models.Input;
using BalanceLoans.Source.Models.Output;

namespace BalanceLoans
{
    class FileInputOutputService
    {
        public static void WriteResultsToOutputFiles(Source.Utils.CsvHelper csvHelperTest,
            OutputModels outputModels)
        {
            csvHelperTest.WriteAssignmentsToCsv(outputModels.assignmentsForLoans);
            csvHelperTest.WriteYieldsToCsv(outputModels.yieldsForAssignments);
        }

        public static InputModels ReadDataFromFiles(Source.Utils.CsvHelper csvHelperTest)
        {
            IEnumerable<Bank> banks = csvHelperTest.ReadBanksFromCsv();
            IEnumerable<Covenant> covenants = csvHelperTest.ReadCovenantsFromCsv();
            IEnumerable<Facility> facilities = csvHelperTest.ReadFacilitiesFromCsv();
            IEnumerable<Loan> loans = csvHelperTest.ReadLoansFromCsv();
            
            return new InputModels(banks, facilities, covenants, loans);
        }
    }
}