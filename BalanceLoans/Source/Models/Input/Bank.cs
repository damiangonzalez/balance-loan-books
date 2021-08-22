namespace BalanceLoans.Source.Models.Input
{
    // describes a banking partner.
    public class Bank
    {
        public Bank(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        // The ID of the bank.
        public int id { get; }

        // The name of the bank.
        public string name { get; }
    }
}