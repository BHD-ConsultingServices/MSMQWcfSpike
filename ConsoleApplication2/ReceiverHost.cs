namespace MSMQSpike2
{
    using System;
    using System.Messaging;
    using System.ServiceModel;

    /// <summary>
    /// Class ReceiverHost.
    /// </summary>
    public class ReceiverHost : IDisposable
    {
        /// <summary>
        /// The host.
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiverHost"/> class.
        /// </summary>
        /// <param name="qPath">The queue path.</param>
        public ReceiverHost(string qPath)
        {
            var qSubPath = qPath.Substring(qPath.IndexOf("."));
            if (!MessageQueue.Exists(qSubPath))
            {
                Console.WriteLine("Queue missing....creating {0}", qPath);
                MessageQueue.Create(qSubPath, true);
                Console.WriteLine("Created.");
            }

            host = new ServiceHost(typeof(Receiver));
            host.Faulted += host_Faulted;
            host.Open();
            Console.WriteLine("The receiver is ready");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (host != null)
            {
                if (host.State == CommunicationState.Faulted)
                {
                    host.Abort();
                }
                host.Close();
            }
        }

        /// <summary>
        /// Handles the Faulted event of the host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void host_Faulted(object sender, EventArgs e)
        {
            Console.WriteLine("Faulted!");
        }
    }
}
