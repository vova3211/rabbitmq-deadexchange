using MQ.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQ.Rabbit
{
    public class RabbitMQClient : IMQClient
    {
        public IMQConfig Config
        {
            get;
        }
        private ConnectionFactory _factory = new ConnectionFactory();
        private IConnection _connection;
        private IModel _channel;

        public event EventHandler<MQMessage> Message;

        protected bool Initialized { get; private set; } = false;
        protected bool DeclaredDeadExchangeQueue { get; private set; } = false;

        public RabbitMQClient(IMQConfig config)
        {
            Config = config;
        }

        public void Publish<T>(T message, string routingKey = "", bool reuseConnection = false) where T : new()
        {
            if (!Initialized)
            {
                Init();
            }

            try
            {
                var m = Newtonsoft.Json.JsonConvert.SerializeObject(message);

                if (string.IsNullOrEmpty(routingKey))
                {
                    routingKey = Config.QueueName;
                }

                var body = Encoding.UTF8.GetBytes(m);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                if (reuseConnection)
                {
                    _channel.BasicPublish(exchange: Config.Exchange,
                                         routingKey: routingKey.ToLower(),
                                         basicProperties: properties,
                                         body: body);
                }
                else
                {
                    using (_connection)
                    {
                        using (_channel)
                        {
                            _channel.BasicPublish(exchange: Config.Exchange,
                                                 routingKey: routingKey.ToLower(),
                                                 basicProperties: properties,
                                                 body: body);
                            Initialized = false;
                        }
                    }
                }

            }
            catch (Exception)
            {
                //todo: log error
                throw;
            }

        }

        private void Init()
        {
            _factory.UserName = Config.Username;
            _factory.Password = Config.Password;
            _factory.AutomaticRecoveryEnabled = true;
            _factory.Port = Config.Port;
            _factory.VirtualHost = Config.VirtualHost;

            var hosts = Config.HostName.Split(',');

            _connection = _factory.CreateConnection(hosts, Config.Name);
            _channel = _connection.CreateModel();

            _channel.ModelShutdown += Channel_ModelShutdown;
            _channel.CallbackException += Channel_CallbackException;
            _channel.BasicRecoverOk += Channel_BasicRecoverOk;
            _connection.ConnectionBlocked += Connection_ConnectionBlocked;
            _connection.ConnectionRecoveryError += Connection_ConnectionRecoveryError;
            _connection.ConnectionShutdown += Connection_ConnectionShutdown;
            _connection.ConnectionUnblocked += Connection_ConnectionUnblocked;
            _connection.RecoverySucceeded += Connection_RecoverySucceeded;

            if (!string.IsNullOrEmpty(Config.Exchange))
            {
                _channel.ExchangeDeclare(exchange: Config.Exchange, type: Config.ExchangeType, durable: true);
                if (!string.IsNullOrEmpty(Config.QueueName))
                {
                    _channel.QueueDeclare(queue: Config.QueueName,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
                }
            }
            else
            {
                _channel.QueueDeclare(queue: Config.QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            }

            if (Config.UseDeadExchangeToSaveMessages == true && Config.WaitingTimeInDeadExchange.HasValue
                && !string.IsNullOrEmpty(Config.Exchange))
            {
                var argDeadQueue = new Dictionary<string, object>();
                bool declareDeadExchangeQueue = true;

                var time = Config.WaitingTimeInDeadExchange.Value;
                // if time < 0 - messages just redirected to DeadExchange and DeadQueue
                // if time == 0 - messages won't be redirected
                // if time > 0 - messages will be redirected and saved this time
                if (time > 0)
                {
                    argDeadQueue.Add("x-dead-letter-exchange", Config.Exchange);
                    argDeadQueue.Add("x-message-ttl", time);
                }
                else if (time == 0)
                {
                    declareDeadExchangeQueue = false;
                }

                if (declareDeadExchangeQueue)
                {
                    _channel.ExchangeDeclare(exchange: Config.Exchange + "-dead-exchange", type: Config.ExchangeType, durable: true);
                    _channel.QueueDeclare(queue: Config.Exchange + "-dead-queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: argDeadQueue);
                    _channel.QueueBind(queue: Config.Exchange + "-dead-queue",
                                  exchange: Config.Exchange + "-dead-exchange",
                                  routingKey: "");
                    DeclaredDeadExchangeQueue = true;
                }
            }

            Initialized = true;
        }

        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
        }

        private void Connection_ConnectionUnblocked(object sender, EventArgs e)
        {
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
        }

        private void Connection_ConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
        }

        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
        }

        private void Channel_BasicRecoverOk(object sender, EventArgs e)
        {
        }

        private void Channel_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
        }

        private void Channel_ModelShutdown(object sender, ShutdownEventArgs e)
        {
        }

        public void Consume()
        {
            if (!Initialized)
            {
                Init();
            }

            if (!string.IsNullOrEmpty(Config.Exchange))
            {
                var queueName = string.IsNullOrEmpty(Config.QueueName) ? _channel.QueueDeclare().QueueName : Config.QueueName;

                _channel.QueueBind(queue: queueName,
                                  exchange: Config.Exchange,
                                  routingKey: "");

                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_Received;
                _channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
            else
            {
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += Consumer_Received;
                _channel.BasicConsume(queue: Config.QueueName,
                                         autoAck: false,
                                         consumer: consumer);
            }
        }
        private void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var body = e.Body;
                var message = Encoding.UTF8.GetString(body);

                Message?.Invoke(this, new MQMessage
                {
                    RoutingKey = e.RoutingKey,
                    Message = message
                });
                _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //todo: log error

                if (DeclaredDeadExchangeQueue)
                {
                    var properties = _channel.CreateBasicProperties();
                    properties.Persistent = true;

                    _channel.BasicPublish(exchange: Config.Exchange + "-dead-exchange",
                                         routingKey: e.RoutingKey,
                                         basicProperties: properties,
                                         body: e.Body);
                    _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
                }
                else
                {
                    _channel.BasicNack(deliveryTag: e.DeliveryTag, multiple: false, requeue: true);
                }
            }
        }
    }
}
