using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 2.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/

namespace ConsoleApp1
{
    //delegate declaration for creating events
    public delegate void PriceCutEvent(double roomPrice, Thread agentThread);
    public delegate void OrderProcessEvent(Order order, double orderAmount);
    public delegate void OrderCreationEvent();

    public class MainClass
    {
        public static MultiCellBuffer buffer;
        public static Thread[] travelAgentThreads;
        public static bool hotelThreadRunning = true;

        public static void Main(string[] args)
        {
            Console.WriteLine("Inside Main");
            buffer = new MultiCellBuffer();

            Hotel hotel = new Hotel();
            TravelAgent travelAgent = new TravelAgent();

            Thread hotelThread = new Thread(new ThreadStart(hotel.hotelFun));
            hotelThread.Start();

            Hotel.PriceCut += new PriceCutEvent(travelAgent.agentOrder);
            Console.WriteLine("Price cut event has been subscribed");
            TravelAgent.orderCreation += new OrderCreationEvent(hotel.takeOrder);
            Console.WriteLine("Order creation event has been subscribed");
            OrderProcessing.OrderProcess += new OrderProcessEvent(travelAgent.orderProcessConfirm);
            Console.WriteLine("Order process event has been subscribed");

            travelAgentThreads = new Thread[5];
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Creating travel agent thread {0}", (i + 1));
                travelAgentThreads[i] = new Thread(travelAgent.agentFun);
                travelAgentThreads[i].Name = (i + 1).ToString();
                travelAgentThreads[i].Start();
            }
        }
    }

    public class MultiCellBuffer
    {
        // Each cell can contain an order object
        private const int bufferSize = 3; // buffer size
        int usedCells;
        private Order[] multiCells;
        public static Semaphore getSemaph;
        public static Semaphore setSemaph;

        public MultiCellBuffer() // constructor
        {
            multiCells = new Order[bufferSize];
            getSemaph = new Semaphore(0, bufferSize);
            setSemaph = new Semaphore(bufferSize, bufferSize);
        }

        public void SetOneCell(Order data)
        {
            setSemaph.WaitOne();
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] == null)
                    {
                        multiCells[i] = data;
                        break;
                    }
                }
            }
            getSemaph.Release();
        }

        public Order GetOneCell()
        {
            getSemaph.WaitOne();
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] != null)
                    {
                        Order order = multiCells[i];
                        multiCells[i] = null;
                        setSemaph.Release();
                        return order;
                    }
                }
            }
            return null;
        }
    }

    public class Order
    {
        private string senderId;
        private long cardNo;
        private double unitPrice;
        private int quantity;

        public Order(string senderId, long cardNo, double unitPrice, int quantity)
        {
            this.senderId = senderId;
            this.cardNo = cardNo;
            this.unitPrice = unitPrice;
            this.quantity = quantity;
        }

        public string getSenderId()
        {
            return senderId;
        }

        public long getCardNo()
        {
            return cardNo;
        }

        public double getUnitPrice()
        {
            return unitPrice;
        }

        public int getQuantity()
        {
            return quantity;
        }
    }

    public class OrderProcessing
    {
        public static event OrderProcessEvent OrderProcess;

        public static bool creditCardCheck(long creditCardNumber)
        {
            return creditCardNumber >= 5000 && creditCardNumber <= 7000;
        }

        public static double calculateCharge(double unitPrice, int quantity)
        {
            Random rnd = new Random();
            double tax = rnd.Next(8, 13) / 100.0;
            double locationCharge = rnd.Next(20, 81);
            return (unitPrice * quantity) + (unitPrice * quantity * tax) + locationCharge;
        }

        public static void ProcessOrder(Order order)
        {
            if (creditCardCheck(order.getCardNo()))
            {
                double totalAmount = calculateCharge(order.getUnitPrice(), order.getQuantity());
                OrderProcess?.Invoke(order, totalAmount);
                Console.WriteLine($"{order.getSenderId()}'s order is confirmed. The amount to be charged is ${totalAmount}");
            }
            else
            {
                Console.WriteLine($"{order.getSenderId()}'s order failed due to invalid credit card.");
            }
        }
    }

    public class TravelAgent
    {
        public static event OrderCreationEvent orderCreation;

        public void agentFun()
        {
            Random rnd = new Random();
            while (MainClass.hotelThreadRunning)
            {
                Thread.Sleep(rnd.Next(1000, 5000));
                createOrder(Thread.CurrentThread.Name);
            }
        }

        public void orderProcessConfirm(Order order, double orderAmount)
        {
            Console.WriteLine($"Agent {order.getSenderId()} received confirmation for order. Total amount: ${orderAmount}");
        }

        private void createOrder(string senderId)
        {
            Random rnd = new Random();
            long cardNo = rnd.Next(4000, 8000);
            double unitPrice = rnd.Next(80, 160);
            int quantity = rnd.Next(1, 5);
            Order order = new Order(senderId, cardNo, unitPrice, quantity);
            MultiCellBuffer buffer = MainClass.buffer;
            buffer.SetOneCell(order);
            orderCreation?.Invoke();
            Console.WriteLine($"Agent {senderId} created an order for {quantity} rooms at ${unitPrice} each.");
        }

        public void agentOrder(double roomPrice, Thread travelAgent)
        {
            Console.WriteLine($"Agent {travelAgent.Name} received a price cut notification. New price: ${roomPrice}");
        }
    }

    public class Hotel
    {
        static double currentRoomPrice = 100;
        static int eventCount = 0;
        public static event PriceCutEvent PriceCut;

        public void hotelFun()
        {
            Random rnd = new Random();
            while (eventCount < 10)
            {
                Thread.Sleep(rnd.Next(1000, 3000));
                double newPrice = pricingModel();

                if (newPrice < currentRoomPrice)
                {
                    eventCount++;
                    PriceCut?.Invoke(newPrice, Thread.CurrentThread);
                    Console.WriteLine($"Price cut! New price is ${newPrice}");
                }

                currentRoomPrice = newPrice;
                takeOrder();
            }
        }

        public double pricingModel()
        {
            Random rnd = new Random();
            return rnd.Next(70, 150);
        }

        public void takeOrder()
        {
            Order order = MainClass.buffer.GetOneCell();
            if (order != null)
            {
                Thread orderProcessingThread = new Thread(() => OrderProcessing.ProcessOrder(order));
                orderProcessingThread.Start();
            }
        }
    }
}
