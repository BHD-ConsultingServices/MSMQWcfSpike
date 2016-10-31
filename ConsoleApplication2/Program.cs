namespace MSMQSpike2
{
    using System;

    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            var qPath = @"FormatName:Direct=OS:.\Private$\Spike";
            var c = new ReceiverHost(qPath);
            Console.WriteLine("Enter message to send or 'X' to exit.");
            var txt = Console.ReadLine();
            if(txt.ToUpper()=="X")
            {
                return;
            }

            var s = new Sender(qPath);
            while (txt.ToUpper() != "X")
            {
                s.Send(txt);
                txt = Console.ReadLine();
            }
        }
    }
}
