using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BalanceLoans.Source.Models.Input;
using BalanceLoans.Source.Models.Output;
using CsvHelper;

namespace BalanceLoans.Source.Utils
{
    public class CsvHelper
    {
        protected virtual string CsvDirectory => "large";
        protected string csvDataRelativePath = AppDomain.CurrentDomain.BaseDirectory + "../../../../BalanceLoans/CsvData/";

        public IEnumerable<Bank> ReadBanksFromCsv()
        {
            IEnumerable<Bank> records = new List<Bank>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/banks.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Bank>().ToList();
            }

            return records;
        }

        public IEnumerable<Covenant> ReadCovenantsFromCsv()
        {
            IEnumerable<Covenant> records = new List<Covenant>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/covenants.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Covenant>().ToList();
            }

            return records;
        }

        public IEnumerable<Facility> ReadFacilitiesFromCsv()
        {
            IEnumerable<Facility> records;
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/facilities.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Facility>().ToList();
            }

            return records;
        }

        public IEnumerable<Loan> ReadLoansFromCsv()
        {
            IEnumerable<Loan> records = new List<Loan>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/loans.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // todo opportunity to improve by leveraging yield instead of ToList 
                // for optimizing performance / memory usage on large csv sizes
                records = csv.GetRecords<Loan>().ToList();
            }

            return records;
        }

        public IEnumerable<Assignment> ReadAssignmentsFromCsv()
        {
            IEnumerable<Assignment> records = new List<Assignment>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/results/assignments.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Assignment>().ToList();
            }

            return records;
        }

        public IEnumerable<Yield> ReadYieldsFromCsv()
        {
            IEnumerable<Yield> records = new List<Yield>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/results/yields.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Yield>().ToList();
            }

            return records;
        }

        public void WriteAssignmentsToCsv(IEnumerable<Assignment> assignments)
        {
            using (var writer = new StreamWriter(GetAssignmentsCsvPath()))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                assignments.OrderBy(x => x.loan_id);
                csv.WriteRecords(assignments);
            }
        }

        public string GetAssignmentsCsvPath()
        {
            string fullPathToCsvDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(csvDataRelativePath), CsvDirectory));
            return $"{fullPathToCsvDirectory}/results/assignments.csv";
        }

        public void WriteYieldsToCsv(IEnumerable<Yield> yields)
        {
            using (var writer = new StreamWriter(GetYieldsCsvPath()))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                yields.OrderBy(x => x.facility_id);
                csv.WriteRecords(yields);
            }
        }

        public string GetYieldsCsvPath()
        {
            string fullPathToCsvDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(csvDataRelativePath), CsvDirectory));
            return $"{fullPathToCsvDirectory}/results/yields.csv";
        }
    }
}