namespace BalanceLoans.Source.Models.Input
{
    // Represents a loan we would like to fund.
    // Loans are ordered chronologically based on when we received them.
    public class Loan
    {
        public Loan(int id, int amount, float interest_rate, float default_likelihood, string state)
        {
            this.id = id;
            this.amount = amount;
            this.interest_rate = interest_rate;
            this.default_likelihood = default_likelihood;
            this.state = state;
        }

        // The ID of the loan. Strictly increasing.
        public int id { get; }

        // The size of the loan in cents.
        public int amount { get; }

        // Between 0 and 1; the interest rate of the loan.
        // In this simplified model, the amount of money we earn from a loan
        // (if it doesn’t default) is amount * interest_rate .
        public float interest_rate { get; }

        // Between 0 and 1; the probability that this loan will default.
        // In this simplified model, when the loan defaults, we lose all the money
        // that we lent and do not earn any interest on the loan.
        public float default_likelihood { get; }

        // State where the loan originated.
        public string state { get; }
    }
}