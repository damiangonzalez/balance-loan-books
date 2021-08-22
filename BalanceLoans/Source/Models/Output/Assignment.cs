namespace BalanceLoans.Source.Models.Output
{
    // describes a loan assignment.
    public class Assignment
    {
        // The ID of the loan.
        public int loan_id { get; set; }

        // If the loan is funded, the ID of its facility;
        // otherwise, empty.
        public int? facility_id { get; set; }

    }
}