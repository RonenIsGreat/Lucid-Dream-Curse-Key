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
    amqp.connect('amqp://admin:admin@localhost', function (error0, connection) {
        if (error0) {
            throw error0;
        }
        connection.createChannel(function (error1, channel) {
            if (error1) {
                throw error1;
            }

            var channelStatusExchange = 'channelStatus';
            var distributionDataExchange = 'distributionData';
            var storageStatusExchange = "storageStatus";
            var targetDataExchange = 'SystemTracksJSON';

            channel.assertExchange(channelStatusExchange, 'fanout', {
                durable: false
            });
            channel.assertExchange(distributionDataExchange, 'fanout', {
                durable: false
            });
            channel.assertExchange(storageStatusExchange, 'fanout', {
                durable: false
            });
            channel.assertExchange(targetDataExchange, 'direct', {
                durable: true
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

            channel.assertQueue('', {
                exclusive: true
            }, function (error2, q) {
                if (error2) {
                    throw error2;
                }
                console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", q.queue);
                channel.bindQueue(q.queue, storageStatusExchange, '');

                channel.consume(q.queue, function (msg) {
                    // ---------- If received message from rabbitMQ: ---------- //
                    if (msg.content) {
                        const storageStatusJSON = msg.content.toString();
                        const storageStatusArray = JSON.parse(storageStatusJSON);
                        let storageStatusObjects = [];
                        storageStatusArray.forEach(element => {
                            const driveSplitArray = element.split(' ');
                            storageStatusObjects.push({
                                available: Number(driveSplitArray[0]),
                                used: Number(driveSplitArray[1]),
                                name: driveSplitArray[2]
                            });
                        });
                        console.log(storageStatusObjects);
                        socket.emit("StorageStatus", JSON.stringify(storageStatusObjects));
                    }
                }, {
                    noAck: true
                });
            });

            channel.assertQueue('', {
                exclusive: true
            }, function (error2, q) {
                if (error2) {
                    throw error2;
                }
                console.log(" [*] Waiting for messages in %s. To exit press CTRL+C", q.queue);
                channel.bindQueue(q.queue, targetDataExchange, 'SystemTracks');

                channel.consume(q.queue, function (msg) {
                    // ---------- If received message from rabbitMQ: ---------- //
                    console.log(msg)
                    if (msg.content) {
                        const targetsString = msg.content.toString();
                        let targets = JSON.parse(targetsString);
                        console.log(targets);
                        socket.emit("TargetStatus", targetsString);
                    }
                }, {
                    noAck: true
                });
            });
        });
    });
});