namespace BalanceLoans.Source.Models.Input
{
    // We borrow money from our banking partners through debt facilities .
    // In turn, we use these facilities to extend loans to customers.
    public class Facility
    {
        public Facility(int id, int bank_id, float interest_rate, decimal amount)
        {
            this.id = id;
            this.bank_id = bank_id;
            this.interest_rate = interest_rate;
            this.amount = amount;
        }

        // The ID of the facility.
        public int id { get; }

        // The ID of the bank providing this facility.
        public int bank_id { get; }

        // Between 0 and 1; the interest rate of this facility.
        // In this simplified model, when we use x dollars from this facility to fund a loan,
        // we are charged x * interest_rate dollars in interest.
        public float interest_rate { get; }

        // The total capacity of the facility in cents.
        public decimal amount { get; private set; }

        public bool WithdrawAmount(decimal amountToWithdraw)
        {
            if (amount >= amountToWithdraw)
            {
                amount -= amountToWithdraw;
                return true;
            }

            return false;
        }
    }
}