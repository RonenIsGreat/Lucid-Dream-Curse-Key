import { createServer } from 'http';
import amqp from 'amqplib/callback_api';
import socketIo from 'socket.io';

const hostname = '127.0.0.1';
const port = 4000;

// ------------------- CreateServer ------------------- //
const server = createServer((req, res) => {
    res.statusCode = 200;
    res.setHeader('Content-Type', 'text/html');
    res.end('<h1>Hello World</h1>');
});


// ------------------- Server Listener ------------------- //
server.listen(port, hostname, () => {
    console.log(`Server running at http://${hostname}:${port}/`);

    let sonarTimeoutChannel;
    amqp.connect('amqp://localhost', function (error0, connection) {
        if (error0) {
            throw error0;
        }
        connection.createChannel(function (error1, channel) {
            if (error1) {
                throw error1;
            }
            
            var channelStatusExchange = 'channelStatus';
            var distributionDataExchange = 'distributionData';

            channel.assertExchange(channelStatusExchange, 'fanout', {
                durable: false
            });
            channel.assertExchange(distributionDataExchange, 'fanout', {
                durable: false
            });

            // ------------------- SocketIO ------------------- //
            const io = socketIo(server);
            const socketCallback = (socket) => {
                console.log("New client connected");
                socket.on("disconnect", () => {
                    console.log("Client disconnected");
                });
                socket.on("DistributionSocketIO", data => {
                    console.log(data);
                    channel.publish(distributionDataExchange, '', Buffer.from(JSON.stringify(data)));
                })
            }
            const socket = io.on("connection", socketCallback);

            channel.assertQueue('', {
                exclusive: true
            }, function (error2, q) {
                if (error2) {
                    throw error2;
                }
                console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", q.queue);
                channel.bindQueue(q.queue, channelStatusExchange, '');

                channel.consume(q.queue, function (msg) {
                    // ---------- If received message from rabbitMQ: ---------- //
                    if (msg.content) {
                        sonarTimeoutChannel = msg.content.toString();
                        console.log(` [x] ${msg.content.toString()}`);
                        socket.emit("StatusSocketIO", sonarTimeoutChannel);
                    }
                }, {
                    noAck: true
                });
            });
        });
    });
});