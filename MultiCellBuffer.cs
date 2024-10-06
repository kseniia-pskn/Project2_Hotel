using System;
using System.Threading;

namespace Project2_Hotel
{public class MultiCellBuffer
{
    private const int bufferSize = 3;
    private Order?[] multiCells = new Order?[bufferSize];
    private SemaphoreSlim setSemaph = new SemaphoreSlim(bufferSize, bufferSize);
    private SemaphoreSlim getSemaph = new SemaphoreSlim(0, bufferSize);

    // Method to set an order into one of the buffer cells
    public void SetOneCell(Order order)
    {
        setSemaph.Wait(); // Wait for an available cell
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
        getSemaph.Release(); // Release after setting
    }

    // Method to get an order from one of the buffer cells
    public Order? GetOneCell()
    {
        if (getSemaph.Wait(3000)) // Wait up to 3 seconds for an available cell
        {
            lock (multiCells)
            {
                for (int i = 0; i < bufferSize; i++)
                {
                    if (multiCells[i] != null)
                    {
                        Order order = multiCells[i]!;
                        multiCells[i] = null; // Clear the cell after getting the order
                        setSemaph.Release(); // Release for further setting
                        return order;
                    }
                }
            }
        }
        Console.WriteLine("Timeout occurred while trying to get an order from the buffer.");
        return null; // Return null if no order is found
    }
}

}
