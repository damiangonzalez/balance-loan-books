namespace BalanceLoans.Source.Models.Input
{
    // represents at least one covenant that we have with a bank.
    // If a row contains both a max_default_likelihood and a banned_state ,
    // they should be treated as separate covenants. 
    public class Covenant
    {
        public Covenant(int bank_id, int? facility_id, float? max_default_likelihood, string banned_state)
        {
            this.bank_id = bank_id;
            this.facility_id = facility_id;
            this.max_default_likelihood = max_default_likelihood;
            this.banned_state = banned_state;
        }

        // The ID of the bank requiring this covenant.
        public int bank_id { get; }

        // If present, denotes that this covenant applies to
        // the facility with this ID; otherwise, this covenant applies
        // to all of the bank’s facilities.
        public int? facility_id { get; } // nullable to identify case when not present

        // If present, specifies the maximum allowed default rate for loans
        // in the facility (or in the bank’s facilities).
        public float? max_default_likelihood { get; }

        //  If present, indicates that loans in the facility
        // (or in the bank’s facilities) may not originate from this state.
        public string banned_state { get; }
    }
}