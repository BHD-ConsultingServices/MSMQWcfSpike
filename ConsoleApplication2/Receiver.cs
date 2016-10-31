namespace MSMQSpike2
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.MsmqIntegration;

    /// <summary>
    /// Class Receiver.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, InstanceContextMode = InstanceContextMode.Single, ReleaseServiceInstanceOnTransactionComplete = false)]
    [PoisonErrorBehaviorAttribute(typeof(PoisonErrorHandler))]
    public class Receiver : IMessaging
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver"/> class.
        /// </summary>
        public Receiver()
        {
        }
        
        /// <summary>
        /// Processes the incoming message.
        /// </summary>
        /// <param name="incomingMessage">The incoming message.</param>
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void ProcessIncomingMessage(MsmqMessage<MyData> incomingMessage)
        {
            if (incomingMessage.Body.SecretMessage.ToUpper() == "BAD" || string.IsNullOrWhiteSpace(incomingMessage.Body.SecretMessage))
            {
                Console.WriteLine("ERROR: Message was 'bad' or empty.");
                throw new ApplicationException("Cannot process - moving to poizon message queue");
            }

            Console.WriteLine("Receiver says - received - {0}", incomingMessage.Body.SecretMessage);
        }
    }

    class PoisonErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // No-op -We are not interested in this. This is only useful if you want to send back a fault on the wire�not applicable for queues [one-way].
        }

        public bool HandleError(Exception error)
        {
            if (error != null && error.GetType() == typeof(MsmqPoisonMessageException))
            {
                Console.WriteLine(" Poisoned message -message look up id = {0}", ((MsmqPoisonMessageException)error).MessageLookupId);
                return true;
            }

            return false;
        }
    }

    public class PoisonErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        Type errorHandlerType;

        public PoisonErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }

        void IServiceBehavior.Validate(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
            {
                errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
            }
            catch (MissingMethodException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the PoisonErrorBehaviorAttribute constructor must have a public empty constructor", e);
            }
            catch (InvalidCastException e)
            {
                throw new ArgumentException("The errorHandlerType specified in the PoisonErrorBehaviorAttribute constructor must implement System.ServiceModel.Dispatcher.IErrorHandler", e);
            }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }
    }
}
