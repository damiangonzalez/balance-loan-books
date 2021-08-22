using System.Collections.Generic;
using BalanceLoans.Source.CalcEngines;
using BalanceLoans.Source.Models;
using BalanceLoans.Source.Models.Output;

namespace BalanceLoans
{
    public static class BalanceLoansOrchestrator
    {
        public static void ExecuteLoanBalancingSequence(Source.Utils.CsvHelper csvHelper)
        {
            InputModels inputModels = ReadInputs(csvHelper);
            OutputModels outputModels = PerformCalculations(inputModels);
            WriteOutputs(csvHelper, outputModels);
        }

        private static void WriteOutputs(Source.Utils.CsvHelper csvHelper, OutputModels outputModels)
        {
            FileInputOutputService.WriteResultsToOutputFiles(csvHelper,
                new OutputModels(outputModels.assignmentsForLoans, outputModels.yieldsForAssignments));
        }

        private static OutputModels PerformCalculations(InputModels inputModels)
        {
            IEnumerable<Assignment> assignmentsForLoans =
                BalanceLoansAssignmentsCalcEngine.IdentifyAssignmentsForLoans(inputModels);
            IEnumerable<Yield> yieldsForAssignments =
                BalanceLoansYieldsCalcEngine.CalculateYieldsForAssignments(assignmentsForLoans, inputModels);
            return new OutputModels(assignmentsForLoans, yieldsForAssignments);
        }

        private static InputModels ReadInputs(Source.Utils.CsvHelper csvHelper)
        {
            return FileInputOutputService.ReadDataFromFiles(csvHelper);
        }
    }
}