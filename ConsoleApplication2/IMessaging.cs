namespace MSMQSpike2
{
    using System.ServiceModel;
    using System.ServiceModel.MsmqIntegration;

    /// <summary>
    /// Messaging Interface.
    /// </summary>
    [ServiceContract]
    [ServiceKnownType(typeof(MyData))]
    public interface IMessaging
    {
        /// <summary>
        /// Processes the incoming message.
        /// </summary>
        /// <param name="incomingMessage">The incoming message.</param>
        [OperationContract(IsOneWay = true, Action = "*")]
        void ProcessIncomingMessage(MsmqMessage<MyData> incomingMessage);
    }
}
