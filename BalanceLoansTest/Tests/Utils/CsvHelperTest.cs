using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BalanceLoans.Source.Models.Output;
using CsvHelper;

namespace BalanceLoans.Tests.Utils
{
    public class CsvHelperTest : CsvHelperSmall
    {
        protected override string CsvDirectory => "test";
    }
}