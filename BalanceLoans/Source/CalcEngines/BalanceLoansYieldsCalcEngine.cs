using System;
using System.Collections.Generic;
using System.Linq;
using BalanceLoans.Source.Models;
using BalanceLoans.Source.Models.Input;
using BalanceLoans.Source.Models.Output;

namespace BalanceLoans.Source.CalcEngines
{
    static class BalanceLoansYieldsCalcEngine
    {
        public static IEnumerable<Yield> CalculateYieldsForAssignments(IEnumerable<Assignment> assignments,
            InputModels inputModels)
        {
            /*
                The expected yield of the facility, rounded to the nearest cent, is defined
                as the sum of the expected yields for all the loans in the facility.
             */
            Dictionary<int, int> yieldsDictionary = new Dictionary<int, int>();

            foreach (Assignment assignment in assignments)
            {
                if (assignment.facility_id != null)
                {
                    int facilityId = (int) assignment.facility_id;
                    int yieldForAssignment = CalculateYieldForSingleAssignment(assignment, inputModels);
                    
                    if (yieldsDictionary.ContainsKey(facilityId))
                    {
                        yieldsDictionary[facilityId] += yieldForAssignment;
                    }
                    else
                    {
                        yieldsDictionary.Add(facilityId, yieldForAssignment);
                    }
                }
            }

            return yieldsDictionary.Select(keyValuePair => new Yield()
            {
                facility_id = keyValuePair.Key,
                expected_yield = keyValuePair.Value
            }).ToList();
        }

        private static int CalculateYieldForSingleAssignment(Assignment assignment, InputModels inputModels)
        {
            if (assignment.facility_id == null)
            {
                // unassigned loan
                Console.WriteLine("Found unassigned loan. This may happen just once according to the instructions");
                return 0;
            }

            /*
                Calculating Loan Yields
                The expected yield of a loan funded by a facility is the amount of interest that we expect to earn
                from the loan (taking into account the chance of a default), minus the expected loss from a
                default, minus the interest that we pay to to the bank to use their facility:
                expected_yield =
                (1 - default_likelihood) * loan_interest_rate * amount
                - default_likelihood * amount
                - facility_interest_rate * amount             
             */

            IEnumerable<Loan> loansForThisAssignmentLoanId = inputModels.loans.Where(x => x.id == assignment.loan_id);
            IEnumerable<Facility> facilitiesForThisAssignmentFacilityId =
                inputModels.facilities.Where(x => x.id == assignment.facility_id);

            if (loansForThisAssignmentLoanId.Any() && loansForThisAssignmentLoanId.Count() == 1 &&
                facilitiesForThisAssignmentFacilityId.Any() && facilitiesForThisAssignmentFacilityId.Count() == 1)
            {
                Loan loanForThisAssignment = loansForThisAssignmentLoanId.First();
                float default_likelihood = loanForThisAssignment.default_likelihood;
                float loan_interest_rate = loanForThisAssignment.interest_rate;
                int amount = loanForThisAssignment.amount;

                Facility facilityForThisAssignment = facilitiesForThisAssignmentFacilityId.First();
                float facility_interest_rate = facilityForThisAssignment.interest_rate;

                int expected_yield = (int) ( (int)((1 - default_likelihood) * loan_interest_rate * amount)
                                            - (int) (default_likelihood * amount)
                                            - (int) (facility_interest_rate * amount));

                return expected_yield;
            }

            // bad data
            return 0;
        }
    }
}