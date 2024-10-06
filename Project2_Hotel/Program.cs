using System;
using System.Threading;

namespace Project2_Hotel
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiCellBuffer buffer = new MultiCellBuffer();
            Hotel hotel = new Hotel(buffer);
            TravelAgent agent1 = new TravelAgent("Agent1", buffer);
            TravelAgent agent2 = new TravelAgent("Agent2", buffer);

            Thread hotelThread = new Thread(hotel.Start);
            Thread agentThread1 = new Thread(agent1.Start);
            Thread agentThread2 = new Thread(agent2.Start);

            hotelThread.Start();
            agentThread1.Start();
            agentThread2.Start();

            hotelThread.Join();
            agentThread1.Join();
            agentThread2.Join();
        }
    }

    public class MultiCellBuffer
    {
        private const int bufferSize = 3;
        private Order[] multiCells = new Order[bufferSize];
        private SemaphoreSlim setSemaph = new SemaphoreSlim(bufferSize, bufferSize);
        private SemaphoreSlim getSemaph = new SemaphoreSlim(0, bufferSize);

        public void SetOneCell(Order order)
        {
            setSemaph.Wait();
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] == null)
                    {
                        multiCells[i] = order;
                        break;
                    }
                }
            }
            getSemaph.Release();
        }

        public Order GetOneCell()
        {
            getSemaph.Wait();
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
        public string SenderId { get; private set; }
        public long CardNo { get; private set; }
        public double UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public Order(string senderId, long cardNo, double unitPrice, int quantity)
        {
            SenderId = senderId;
            CardNo = cardNo;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }

    public class Hotel
    {
        private MultiCellBuffer buffer;
        private Random rnd = new Random();

        public Hotel(MultiCellBuffer buffer)
        {
            this.buffer = buffer;
        }

        public void Start()
        {
            while (true)
            {
                Thread.Sleep(rnd.Next(1000, 3000)); // Random wait to simulate price change
                double newPrice = rnd.Next(70, 150);
                Console.WriteLine($"Hotel updated the room price to ${newPrice}");
            }
        }
    }

    public class TravelAgent
    {
        private string agentId;
        private MultiCellBuffer buffer;

        public TravelAgent(string agentId, MultiCellBuffer buffer)
        {
            this.agentId = agentId;
            this.buffer = buffer;
        }

        public void Start()
        {
            Random rnd = new Random();
            while (true)
            {
                Thread.Sleep(rnd.Next(1000, 5000)); // Random wait to simulate activity
                long cardNo = rnd.Next(5000, 8000);
                double unitPrice = rnd.Next(80, 160);
                int quantity = rnd.Next(1, 5);

                Order order = new Order(agentId, cardNo, unitPrice, quantity);
                buffer.SetOneCell(order);
                Console.WriteLine($"{agentId} created an order for {quantity} rooms at ${unitPrice} each.");
            }
        }
    }
}
