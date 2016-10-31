namespace MSMQSpike2
{
    using System;
    using System.Messaging;
    using System.Transactions;

    /// <summary>
    /// Class Sender.
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// The queue path.
        /// </summary>
        private string qPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sender"/> class.
        /// </summary>
        /// <param name="qPath">The q path.</param>
        public Sender(string qPath)
        {
            this.qPath = qPath;
        }

        /// <summary>
        /// Sends the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Send(string text)
        {
            var queue = new MessageQueue(this.qPath);
            
            var simulateAnUnknownMessageType = text.ToUpper() == "VBAD";
            if (simulateAnUnknownMessageType)
            {
                var badMsg = new Message { Body = new OtherData { SomeData = 10 } };
                PostToQueue(queue, badMsg);
                Console.WriteLine("Sender says - Message Sent");
                return;
            }

            var msg = new Message { Body = new MyData { SecretMessage = text } };
            PostToQueue(queue, msg);
            Console.WriteLine("Sender says - Message Sent");
        }

        private static void PostToQueue(MessageQueue queue, Message msg)
        {
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                queue.Send(msg, "SomeLabel", MessageQueueTransactionType.Automatic);
                ts.Complete();
            }
        }
    }
}
