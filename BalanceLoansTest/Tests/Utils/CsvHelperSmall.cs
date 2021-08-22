using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BalanceLoans.Source.Models.Output;
using CsvHelper;

namespace BalanceLoans.Tests.Utils
{
    public class CsvHelperSmall : Source.Utils.CsvHelper
    {
        protected override string CsvDirectory => "small";
        
        public IEnumerable<Assignment> ReadAssignmentsReferenceFromCsv()
        {
            IEnumerable<Assignment> records = new List<Assignment>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/results/assignmentsReference.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Assignment>().ToList();
            }

            return records;
        }

        public IEnumerable<Yield> ReadYieldsReferenceFromCsv()
        {
            IEnumerable<Yield> records = new List<Yield>();
            using (var reader = new StreamReader($"{csvDataRelativePath}{CsvDirectory}/results/yieldsReference.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Yield>().ToList();
            }

            return records;
        }

        
    }
}