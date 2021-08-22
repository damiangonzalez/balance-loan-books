using System.Collections.Generic;
using BalanceLoans.Source.Models.Input;

namespace BalanceLoans.Source.Models
{
    public class InputModels
    {
        public InputModels(IEnumerable<Bank> banks,
            IEnumerable<Facility> facilities,
            IEnumerable<Covenant> covenants,
            IEnumerable<Loan> loans)
        {
            this.banks = banks;
            this.facilities = facilities;
            this.covenants = covenants;
            this.loans = loans;
        }

        public IEnumerable<Bank> banks { get; }
        public IEnumerable<Facility> facilities { get; }
        public IEnumerable<Covenant> covenants { get; }
        public IEnumerable<Loan> loans { get; }
    }
}