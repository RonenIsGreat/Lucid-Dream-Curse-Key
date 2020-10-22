using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TargetsStreamerMain.Models;
using TargetsStreamerMain;

namespace ImprovingSimulator
{
    public partial class MainForm : Form
    {
        private readonly List<CancellationTokenSource> streamsCancellationTokens;
        private CancellationTokenSource targetsCancellationTokenSource;
        private int NumberOfMessages;

        public MainForm()
        {
            InitializeComponent();
            NumberOfMessages = 0;
            streamsCancellationTokens = new List<CancellationTokenSource>();
        } //End MainForm Constructor

        private void StartSendingStreamMessages(string streamType, CancellationToken ct,
            long numberOfMessagesToSend = -1)
        {
            if (ConfigurationManager.GetSection("StreamSettings/" + streamType) is NameValueCollection config)
            {
                var path = ConfigurationManager.AppSettings["RecordingsPath"];

                IPAddress udpIp = IPAddress.Parse( ConfigurationManager.AppSettings["UdpRemoteIp"]);

                var fullPath = Path.Combine(path, config["Recording_Name"]);

                var stream = new StreamWrapper.Main.StreamWrapper(fullPath,
                    udpIp.ToString(),
                    int.Parse(config["Port"]),
                    double.Parse(config["Delimiter"]));

                // We don't care if we go back to original context so configure await is false
                Task.Run(() => stream.SendMessages(ct, numberOfMessagesToSend), ct);
            }
        }

        private void StopAllSending()
        {
            foreach (var ct in streamsCancellationTokens) ct.Cancel();

            streamsCancellationTokens.Clear();
        } //End If

        private void ExitButton_Click(object sender, EventArgs e)
        {
            DestroyHandle();
        } //End ExitButton_Click

        private void SequenceSendingBtn_Click(object sender, EventArgs e)
        {
            if (SequenceSendingBtn.Text == "Start Sending")
            {
                //Ensure cancellation tokens list is cleared
                streamsCancellationTokens.Clear();

                //groupBox1.Enabled = false;

                DisableCheckboxes();

                var success = SendMessagesToCheckedStreams();

                if (!success)
                {
                    MessageBox.Show("No Channels Were Selected!");
                    SequenceSendingBtn.Text = "Start Sending";
                    EnableCheckboxes();
                    //groupBox1.Enabled = true;
                } //End If

                else
                {
                    SequenceSendingBtn.Text = "Stop Sending";
                }
            } //End If

            else
            {
                SequenceSendingBtn.Text = "Start Sending";
                StopAllSending();
                EnableCheckboxes();
                //groupBox1.Enabled = true;
            } //End Else
        }

        private bool SendMessagesToCheckedStreams(long numberOfMessagesToSend = -1)
        {
            var flag = false;
            var checkedBoxes = Controls.OfType<CheckBox>().Where(box => box.Checked);
            foreach (var box in checkedBoxes)
            {
                var ctSource = new CancellationTokenSource();
                var streamType = box.Name.Replace("CheckBox", "");

                streamsCancellationTokens.Add(ctSource);

                StartSendingStreamMessages(streamType, ctSource.Token, numberOfMessagesToSend);
                flag = true;
            }

            return flag;
        }

        private void DisableCheckboxes()
        {
            BeamBusCasCheckBox.Enabled = false;
            BeamBusFasTasCheckBox.Enabled = false;
            StaveBusCasCheckBox.Enabled = false;
            StaveBusFasTasCheckBox.Enabled = false;
            IdrsCheckBox.Enabled = false;
            PrsStaveBusCheckBox.Enabled = false;
        } //End DisableCheckBoxes

        private void EnableCheckboxes()
        {
            BeamBusCasCheckBox.Enabled = true;
            BeamBusFasTasCheckBox.Enabled = true;
            StaveBusCasCheckBox.Enabled = true;
            StaveBusFasTasCheckBox.Enabled = true;
            IdrsCheckBox.Enabled = true;
            PrsStaveBusCheckBox.Enabled = true;
        } //End EnableCheckboxes

        private void QuantitySendingBtn_Click(object sender, EventArgs e)
        {
            if (QuantitySendingBtn.Text == "Send By Number")
            {
                NumberOfMessages = (int) numericUpDown1.Value;

                DisableCheckboxes();
                QuantitySendingBtn.Text = "Stop Sending";


                var success = SendMessagesToCheckedStreams(NumberOfMessages);

                if (!success)
                {
                    MessageBox.Show("No Channels Were Selected!");
                    QuantitySendingBtn.Text = "Send By Number";
                    EnableCheckboxes();
                } //End If

                //BeamBusCasNumberSenderThread.Join();
            } //End If

            else
            {
                QuantitySendingBtn.Text = "Send By Number";
                StopAllSending();
                EnableCheckboxes();
            } //End Else
        } //End QuantitySendingBtn

        private void SendTargetsBtn_Click(object sender, EventArgs e)
        {
            if (SendTargetsBtn.Text == "Start Sending Targets")
            {
                targetsCancellationTokenSource = new CancellationTokenSource();
                TargetsStreamer targetsStreamer = TargetsStreamer.Instance;
                targetsCancellationTokenSource = new CancellationTokenSource();
                SendTargetsBtn.Text = "Stop Sending Targets";

                // Do it in background
                Task.Run(() => targetsStreamer.StartSending(targetsCancellationTokenSource.Token));
            }
            else
            {
                targetsCancellationTokenSource.Cancel();
                SendTargetsBtn.Text = "Start Sending Targets";
            }
        }
    } //End MainForm
} //End Improving Simulator