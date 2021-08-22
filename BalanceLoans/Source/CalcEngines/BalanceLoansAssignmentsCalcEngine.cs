using System;
using System.Collections.Generic;
using System.Linq;
using BalanceLoans.Source.Models;
using BalanceLoans.Source.Models.Input;
using BalanceLoans.Source.Models.Output;

namespace BalanceLoans.Source.CalcEngines
{
    static class BalanceLoansAssignmentsCalcEngine
    {
        public static IEnumerable<Assignment> IdentifyAssignmentsForLoans(InputModels inputModels)
        {
            List<Assignment> assignments = new List<Assignment>();
            inputModels.loans.OrderBy(x => x.id);

            foreach (Loan loan in inputModels.loans)
            {
                List<int> disqualifyingFacilityIds = IdentifyDisqualifyingFacilityIds(inputModels, loan);
                List<int> disqualifyingBankIds = IdentifyDisqualifyingBankIds(inputModels, loan); 
                
                var qualifyingFacilityIds = IdentifyQualifyingFacilityIds(inputModels, disqualifyingFacilityIds, disqualifyingBankIds);

                if (qualifyingFacilityIds.Any())
                {
                    var optimalFacility = IdentifyOptimalFacility(inputModels, qualifyingFacilityIds, loan);

                    if (optimalFacility != null)
                    {
                        AssignLoanToFacility(assignments, loan, optimalFacility);
                    }
                    else
                    {
                        LoanNotAssigned(assignments, loan);
                    }
                }
                else
                {
                    LoanNotAssigned(assignments, loan);
                }
            }

            return assignments;
        }

        private static List<int> IdentifyQualifyingFacilityIds(InputModels inputModels, 
            List<int> disqualifyingFacilityIds,
            List<int> disqualifyingBankIds)
        {
            // identify qualifying facility id's based on what's left after
            // removing disqualifying facilities and disqualifying banks 
            var qualifyingFacilityIds = new List<int>();
            foreach (Facility inputModelsFacility in inputModels.facilities)
            {
                if (disqualifyingFacilityIds.Contains(inputModelsFacility.id) ||
                    disqualifyingBankIds.Contains(inputModelsFacility.bank_id))
                {
                    // this facility doesn't qualify
                }
                else
                {
                    qualifyingFacilityIds.Add(inputModelsFacility.id);
                }
            }

            return qualifyingFacilityIds;
        }

        private static List<int> IdentifyDisqualifyingFacilityIds(InputModels inputModels, Loan loan)
        {
            var disqualifyingCovenants = IdentifyDisqualifyingCovenants(inputModels, loan);

            /*
             * If present, denotes that this
                covenant applies to the
                facility with this ID; otherwise,
                this covenant applies to all of
                the bank’s facilities.
             */
            List<int> disqualifyingFacilityIds = new List<int>();
            foreach (Covenant disqualifyingCovenant in disqualifyingCovenants)
            {
                if (disqualifyingCovenant.facility_id != null)
                {
                    disqualifyingFacilityIds.Add((int) disqualifyingCovenant.facility_id);
                }
            }

            return disqualifyingFacilityIds;
        }

        private static List<int> IdentifyDisqualifyingBankIds(InputModels inputModels, Loan loan)
        {
            var disqualifyingCovenants = IdentifyDisqualifyingCovenants(inputModels, loan);

            /*
             * If present, denotes that this
                covenant applies to the
                facility with this ID; otherwise,
                this covenant applies to all of
                the bank’s facilities.
             */
            List<int> disqualifyingBankIds = new List<int>();
            foreach (Covenant disqualifyingCovenant in disqualifyingCovenants)
            {
                if (disqualifyingCovenant.facility_id == null)
                {
                    disqualifyingBankIds.Add(disqualifyingCovenant.bank_id);
                }
            }

            return disqualifyingBankIds;
        }

        private static IEnumerable<Covenant> IdentifyDisqualifyingCovenants(InputModels inputModels, Loan loan)
        {
            // todo I'm calling this twice, this can be optimized, though it's in memory so not a huge deal
            // "If a row contains both a max_default_likelihood and a banned_state ,
            // they should be treated as separate covenants."
            IEnumerable<Covenant> disqualifyingCovenants = inputModels.covenants.Where(x =>
                x.banned_state == loan.state ||
                (x.max_default_likelihood != null &&
                 x.max_default_likelihood < loan.default_likelihood));
            return disqualifyingCovenants;
        }

        private static Facility IdentifyOptimalFacility(InputModels inputModels,
            IEnumerable<int> facilityIdsForMatchingCovenants, Loan loan)
        {
            // then identify matching facilities that still have enough amounts to cover the loan
            IEnumerable<Facility> matchingFacilitiesWithEnoughFunds = inputModels.facilities.Where(
                x => facilityIdsForMatchingCovenants.Contains(x.id) &&
                     x.amount >= loan.amount);

            if (matchingFacilitiesWithEnoughFunds.Any())
            {
                // then identify optimal facility based on interest rate
                var matchingFacilityWithLowestInterestRate =
                    matchingFacilitiesWithEnoughFunds.OrderBy(x => x.interest_rate).First();
                return matchingFacilityWithLowestInterestRate;
            }

            // no optimal facility was found
            return null;
        }

        private static void LoanNotAssigned(List<Assignment> assignments, Loan loan)
        {
            assignments.Add(new Assignment()
            {
                loan_id = loan.id,
                facility_id = null
            });
        }

        private static void AssignLoanToFacility(List<Assignment> assignments, Loan loan,
            Facility matchingFacilityWithLowestInterestRate)
        {
            assignments.Add(new Assignment()
            {
                loan_id = loan.id,
                facility_id = matchingFacilityWithLowestInterestRate.id
            });

            matchingFacilityWithLowestInterestRate.WithdrawAmount(loan.amount);

            // Console.WriteLine($"Assigned loan {loan.id} to facility {matchingFacilityWithLowestInterestRate.id}, loan amount {loan.amount} outstanding facility balance {matchingFacilityWithLowestInterestRate.amount}");
        }
    }
}