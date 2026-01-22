namespace Nhom21.FinancialTerminal.Server
{
    public class Stock
    {
        public string Symbol { get; set; }
        public double Price { get; set; }
        public double PrevPrice { get; set; }
        public double ChangePercent => PrevPrice == 0 ? 0 : ((Price - PrevPrice) / PrevPrice) * 100;

        public Stock(string symbol, double initialPrice)
        {
            Symbol = symbol;
            Price = initialPrice;
            PrevPrice = initialPrice;
        }

        public void UpdatePrice(Random rng)
        {
            PrevPrice = Price;
            // Randomly change price by -2% to +2%
            double change = 1 + (rng.NextDouble() * 0.04 - 0.02);
            Price = Math.Round(Price * change, 2);
        }

        public override string ToString()
        {
            return $"{Symbol}|{Price:F2}|{ChangePercent:F2}%";
        }
    }
}
