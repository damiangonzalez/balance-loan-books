using System;
using System.Collections.Generic;
using System.Linq;
using BalanceLoans.Source.Models.Output;
using BalanceLoans.Tests.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace BalanceLoans.Tests
{
    public class BalanceLoansOrchestratorTests
    {
        [Test]
        public void
            Given_BalanceLoansOrchestrator_When_ExecuteLoanBalancingSequence_Then_AllResultsShouldMatchSmallCsvSampleResults()
        {
            // ARRANGE
            CsvHelperSmall csvHelperSmall = new CsvHelperSmall();

            // ACT
            BalanceLoansOrchestrator.ExecuteLoanBalancingSequence(csvHelperSmall);

            // ASSERT
            // obtain calculated results
            List<Assignment> assignments = csvHelperSmall.ReadAssignmentsFromCsv().ToList();
            List<Yield> yields = csvHelperSmall.ReadYieldsFromCsv().ToList();

            // obtain expected results
            List<Assignment> assignmentsExpected = csvHelperSmall.ReadAssignmentsReferenceFromCsv().ToList();
            List<Yield> yieldsExpected = csvHelperSmall.ReadYieldsReferenceFromCsv().ToList();

            // confirm that calculated results are equal to expected results
            assignments.Count().Should().Equals(assignmentsExpected.Count());
            yields.Count().Should().Equals(yieldsExpected.Count());

            assignments.OrderBy(x => x.loan_id);
            assignmentsExpected.OrderBy(x => x.loan_id);
            for (int i = 0; i < assignments.Count; i++)
            {
                assignments[i].facility_id.Should().Be(assignmentsExpected[i].facility_id);
                assignments[i].loan_id.Should().Be(assignmentsExpected[i].loan_id);
            }

            yields.OrderBy(x => x.facility_id);
            yieldsExpected.OrderBy(x => x.facility_id);
            for (int i = 0; i < yields.Count(); i++)
            {
                yields[i].facility_id.Should().Be(yieldsExpected[i].facility_id);
                Math.Abs(yields[i].expected_yield - yieldsExpected[i].expected_yield).Should().BeLessOrEqualTo(1); // within one cent
            }
        }
    }
}