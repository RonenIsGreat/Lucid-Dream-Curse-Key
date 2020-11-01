using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Storage_Status_Backend
{
    class Program
    {
        private const int BytesInGB = 1073741824;

        static void Main()
        {
            const double BytesInGB = Program.BytesInGB;
            double allDrivesAvailable = 0.0, allDrivesUsed = 0.0;
            double driveAvailable, driveUsed;
            string driveName;
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            string[] driveData = new string[allDrives.Length];

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "storageStatus", type: ExchangeType.Fanout);

                while (true)
                {
                    var i = 0;
                    foreach (DriveInfo d in allDrives)
                    {
                        Console.WriteLine("Drive {0}", d.Name);
                        driveName = d.Name;

                        Console.WriteLine("  Drive type: {0}", d.DriveType);
                        if (d.IsReady == true)
                        {
                            Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                            Console.WriteLine("  File system: {0}", d.DriveFormat);
                            Console.WriteLine(
                                "  Available space to current user:{0, 15} Gigabytes",
                                (d.AvailableFreeSpace) / BytesInGB);

                            Console.WriteLine(
                                "  Total available space:          {0, 15} Gigabytes",
                                (d.TotalFreeSpace) / BytesInGB);
                            allDrivesAvailable += (d.TotalFreeSpace) / BytesInGB;
                            driveAvailable = (d.TotalFreeSpace) / BytesInGB;

                            Console.WriteLine(
                                "  Total available space:          {0, 15} Gigabytes",
                                (d.TotalSize - d.TotalFreeSpace) / BytesInGB);
                            allDrivesUsed += (d.TotalSize - d.TotalFreeSpace) / BytesInGB;
                            driveUsed = (d.TotalSize - d.TotalFreeSpace) / BytesInGB;

                            Console.WriteLine(
                                "  Total size of drive:            {0, 15} Gigabytes",
                                (d.TotalSize) / BytesInGB);

                            driveData[i] = driveAvailable + " " + driveUsed + " " + driveName;
                        }

                        i++;
                    }

                    //driveData[1] = "234 200 D:/";

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(driveData));

                    channel.BasicPublish(exchange: "storageStatus",
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
                    Console.WriteLine(" [x] Sent {0}", driveData);

                    allDrivesAvailable = 0.0;
                    allDrivesUsed = 0.0;
                    Thread.Sleep(10000);
                }
            }
        }
    }
}
