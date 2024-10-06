using System;

namespace Project2_Hotel
{
    public delegate void OrderProcessEvent(Order order, double orderAmount);

    public class OrderProcessing
    {
        public static event OrderProcessEvent OrderProcess = delegate { };

        // Method to check for valid credit card number input
        public static bool CreditCardCheck(long creditCardNumber)
        {
            // Dummy check: valid card numbers are between 5000 and 7000
            return creditCardNumber >= 5000 && creditCardNumber <= 7000;
        }

        // Method to calculate the final charge after adding taxes, location charges, etc.
        public static double CalculateCharge(double unitPrice, int quantity)
        {
            Random rnd = new Random();
            double tax = rnd.Next(8, 13) / 100.0; // Tax between 8% and 12%
            double locationCharge = rnd.Next(20, 81); // Location charge between $20 and $80
            return (unitPrice * quantity) + (unitPrice * quantity * tax) + locationCharge;
        }

        // Method to process the order
        public static void ProcessOrder(Order order)
        {
            if (CreditCardCheck(order.CardNo))
            {
                double totalAmount = CalculateCharge(order.UnitPrice, order.Quantity);
                OrderProcess?.Invoke(order, totalAmount);
                Console.WriteLine($"{order.SenderId}'s order is confirmed. The amount to be charged is ${totalAmount}");
            }
            else
            {
                Console.WriteLine($"{order.SenderId}'s order failed due to invalid credit card.");
            }
        }
    }
}
