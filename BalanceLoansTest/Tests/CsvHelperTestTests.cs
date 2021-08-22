using System;
using System.Collections.Generic;
using System.Linq;
using BalanceLoans.Source.Models.Input;
using BalanceLoans.Source.Models.Output;
using BalanceLoans.Tests.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace BalanceLoans.Tests
{
    public class CsvHelperTestTests
    {
        [Test]
        public void Given_CsvHelperTest_When_ReadBanksFromCsv_Then_ReturnAllBanksInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Bank> banks = csvHelperTest.ReadBanksFromCsv();

            banks.Count().Should().Be(2);
            banks.Should().ContainSingle(x => x.id == 1 && x.name == "Chase");
            banks.Should().ContainSingle(x => x.id == 2 && x.name == "Bank of America");
        }

        [Test]
        public void Given_CsvHelper_When_ReadBanksFromCsv_Then_ReturnAllBanksInLargeCsv()
        {
            Source.Utils.CsvHelper csvHelperTest = new Source.Utils.CsvHelper();
            IEnumerable<Bank> banks = csvHelperTest.ReadBanksFromCsv();

            banks.Count().Should().Be(5);
            banks.Should().ContainSingle(x => x.id == 1 && x.name == "American Trust");
            banks.Should().ContainSingle(x => x.id == 2 && x.name == "Citizens Mark");
            banks.Should().ContainSingle(x => x.id == 3 && x.name == "Morriseys Crown");
            banks.Should().ContainSingle(x => x.id == 4 && x.name == "Johnsons First");
            banks.Should().ContainSingle(x => x.id == 5 && x.name == "National Union");
        }

        [Test]
        public void Given_CsvHelperTest_When_ReadCovenantsFromCsv_Then_ReturnAllCovenantsInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Covenant> covenants = csvHelperTest.ReadCovenantsFromCsv();

            covenants.Count().Should().Be(3);
            covenants.Should().ContainSingle(x => x.facility_id.Value == 2
                                                  && ((x.max_default_likelihood.HasValue)
                                                      && (Math.Abs(x.max_default_likelihood.Value - 0.09) < .0001))
                                                  && x.bank_id == 1
                                                  && x.banned_state == "MT");
            covenants.Should().ContainSingle(x => x.facility_id.Value == 1
                                                  && ((x.max_default_likelihood.HasValue)
                                                      && (Math.Abs(x.max_default_likelihood.Value - 0.06) < .0001))
                                                  && x.bank_id == 2
                                                  && x.banned_state == "VT");
            covenants.Should().ContainSingle(x => x.facility_id.Value == 1
                                                  && x.max_default_likelihood.HasValue == false
                                                  && x.bank_id == 2
                                                  && x.banned_state == "CA");
        }

        [Test]
        public void Given_CsvHelperTest_When_ReadFacilitiesFromCsv_Then_ReturnAllFacilitiesInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Facility> facilities = csvHelperTest.ReadFacilitiesFromCsv();

            facilities.Count().Should().Be(2);
            facilities.Should().ContainSingle(x => x.amount == 61104
                                                   && Math.Abs(x.interest_rate - 0.07) < .0001
                                                   && x.id == 2
                                                   && x.bank_id == 1
            );
            facilities.Should().ContainSingle(x => x.amount == 126122
                                                   && Math.Abs(x.interest_rate - 0.06) < .0001
                                                   && x.id == 1
                                                   && x.bank_id == 2
            );
        }

        [Test]
        public void Given_CsvHelperTest_When_ReadLoansFromCsv_Then_ReturnAllLoansInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Loan> loans = csvHelperTest.ReadLoansFromCsv();

            loans.Count().Should().Be(3);
            loans.Should().ContainSingle(x => Math.Abs(x.interest_rate - 0.15) < .0001
                                              && x.amount == 10552
                                              && x.id == 1
                                              && Math.Abs(x.default_likelihood - 0.02) < .0001
                                              && x.state == "MO");
            loans.Should().ContainSingle(x => Math.Abs(x.interest_rate - 0.15) < .0001
                                              && x.amount == 51157
                                              && x.id == 2
                                              && Math.Abs(x.default_likelihood - 0.01) < .0001
                                              && x.state == "VT");
            loans.Should().ContainSingle(x => Math.Abs(x.interest_rate - 0.35) < .0001
                                              && x.amount == 74965
                                              && x.id == 3
                                              && Math.Abs(x.default_likelihood - 0.06) < .0001
                                              && x.state == "AL");
        }

        [Test]
        public void Given_CsvHelperTest_When_ReadAssignmentsFromCsv_Then_ReturnAllAssignmentsInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Assignment> assignments = csvHelperTest.ReadAssignmentsFromCsv();

            assignments.Count().Should().Be(3);
            assignments.Should().ContainSingle(x => x.loan_id == 1 && x.facility_id == 1);
            assignments.Should().ContainSingle(x => x.loan_id == 2 && x.facility_id == 2);
            assignments.Should().ContainSingle(x => x.loan_id == 3 && x.facility_id == 1);
        }

        [Test]
        public void Given_CsvHelperTest_When_ReadYieldsFromCsv_Then_ReturnAllYieldsInSmallCsv()
        {
            CsvHelperTest csvHelperTest = new CsvHelperTest();
            IEnumerable<Yield> yields = csvHelperTest.ReadYieldsFromCsv();

            yields.Count().Should().Be(2);
            yields.Should().ContainSingle(x => x.facility_id == 1 && x.expected_yield == 16375);
            yields.Should().ContainSingle(x => x.facility_id == 2 && x.expected_yield == 3504);
        }

        [Test]
        public void Given_CsvHelper_When_WriteYieldsToCsv_Then_WriteAllYieldsToCsv()
        {
            Source.Utils.CsvHelper csvHelperTest = new Source.Utils.CsvHelper();

            IEnumerable<Yield> yieldsToWrite = new List<Yield>()
            {
                new() {facility_id = 333, expected_yield = 654},
                new() {facility_id = 432, expected_yield = 765},
                new() {facility_id = 543, expected_yield = 876},
            };
            
            csvHelperTest.WriteYieldsToCsv(yieldsToWrite);

            IEnumerable<Yield> yieldsFromRead = csvHelperTest.ReadYieldsFromCsv();
            yieldsFromRead.Count().Should().Be(3);
            yieldsFromRead.Should().ContainSingle(x => x.facility_id == 333 && x.expected_yield == 654);
            yieldsFromRead.Should().ContainSingle(x => x.facility_id == 432 && x.expected_yield == 765);
            yieldsFromRead.Should().ContainSingle(x => x.facility_id == 543 && x.expected_yield == 876);
        }
        
        [Test]
        public void Given_CsvHelper_When_WriteAssignmentsToCsv_Then_WriteAllAssignmentsToCsv()
        {
            Source.Utils.CsvHelper csvHelperTest = new Source.Utils.CsvHelper();

            IEnumerable<Assignment> assignmentsToWrite = new List<Assignment>()
            {
                new() {facility_id = 333, loan_id = 8877},
                new() {facility_id = 2222, loan_id = 6644},
                new() {facility_id = 111, loan_id = 8976},
                new() {facility_id = 00, loan_id = 345},
            };
            
            csvHelperTest.WriteAssignmentsToCsv(assignmentsToWrite);

            IEnumerable<Assignment> assignmentsFromRead = csvHelperTest.ReadAssignmentsFromCsv();
            assignmentsFromRead.Count().Should().Be(4);
            assignmentsFromRead.Should().ContainSingle(x => x.facility_id == 333 && x.loan_id == 8877);
            assignmentsFromRead.Should().ContainSingle(x => x.facility_id == 2222 && x.loan_id == 6644);
            assignmentsFromRead.Should().ContainSingle(x => x.facility_id == 111 && x.loan_id == 8976);
            assignmentsFromRead.Should().ContainSingle(x => x.facility_id == 00 && x.loan_id == 345);
        }

    }
}