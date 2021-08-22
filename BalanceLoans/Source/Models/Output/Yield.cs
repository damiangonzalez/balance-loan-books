namespace BalanceLoans.Source.Models.Output
{
    // describes the expected yield of a facility.
    public class Yield
    {
        // The ID of the facility.
        public int facility_id { get; set; }

        // The expected yield of the facility,
        // rounded to the nearest cent.
        // This is defined as the sum of the expected yields
        // for all the loans in the facility.
        public int expected_yield { get; set; }
    }
}