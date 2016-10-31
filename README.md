# MSMQWcfSpike
THis is a simple demo that defines a contract (MyData.cs) to be sent by the Sender to the Receiver over a message queue. 
The sender places the secret message on the queue and the receiver implements MSMQ WCF integration binding to receive the queued 
message as a WCF call.
The benefit of this over a straight WCF call is guaranteed delivery. If the receiver is unavailable the message is persisted 
on the queue until the receiver is able to handle the message.
In this case when the app starts just type any message in the console window and press enter to send it via the queue.
If the queue does not exist it will automatically be created. A transactional queue is used to further guarantee delivery and
successful handling.

To run this project you need MSMQ installed on your PC. Go to windows features and install MSMQ.
