using System.Collections.Generic;
using BalanceLoans.Source.Models.Output;

namespace BalanceLoans.Source.Models
{
    public class OutputModels
    {
        public OutputModels(IEnumerable<Assignment> assignmentsForLoans, IEnumerable<Yield> yieldsForAssignments)
        {
            this.assignmentsForLoans = assignmentsForLoans;
            this.yieldsForAssignments = yieldsForAssignments;
        }

        public IEnumerable<Assignment> assignmentsForLoans{ get; }
        public IEnumerable<Yield> yieldsForAssignments{ get; }
    }
}