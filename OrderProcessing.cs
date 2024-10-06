using System;

public class OrderProcessing
{
    public static event OrderProcessEvent OrderProcess;

    // Validate credit card
    public static bool CreditCardCheck(long creditCardNumber)
    {
        return creditCardNumber >= 5000 && creditCardNumber <= 7000;
    }

    // Calculate the total charge
    public static double CalculateCharge(double unitPrice, int quantity)
    {
        Random rnd = new Random();
        double tax = rnd.Next(8, 13) / 100.0; // Tax between 8% and 12%
        double locationCharge = rnd.Next(20, 81); // Location charge between $20 and $80
        return (unitPrice * quantity) + (unitPrice * quantity * tax) + locationCharge;
    }

    // Process the order
    public static void ProcessOrder(Order order)
    {
        if (CreditCardCheck(order.CardNo))
        {
            double totalAmount = CalculateCharge(order.UnitPrice, order.Quantity);
            Console.WriteLine($"{order.SenderId}'s order is confirmed. The amount to be charged is ${totalAmount}");
            OrderProcess?.Invoke(order, totalAmount);
        }
        else
        {
            Console.WriteLine($"{order.SenderId}'s order failed due to invalid credit card.");
        }
    }
}
