﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Utilities.BunifuButton.Transitions;
using Utilities.BunifuRadioButton.Transitions;

namespace ImprovingSimulator
{
    public partial class MainForm : Form
    {
        private int NumberOfMessages;
        private List<CancellationTokenSource> cancellationTokens;

        private async void StartSendingMessages(string streamType, CancellationToken ct, long numberOfMessagesToSend = -1)
        {
            if (ConfigurationManager.GetSection("StreamSettings/" + streamType) is NameValueCollection config)
            {
                var path = Environment.GetEnvironmentVariable("RecordingsPath");

                var fullPath = Path.Combine(path, config["Recording_Name"]);

                var stream = new global::StreamWrapper.StreamWrapper(fullPath, IPAddress.Loopback.ToString(), int.Parse(config["Port"]));

                // We don't care if we go back to original context so configure await is false
                await Task.Run(() => stream.SendMessages(ct, numberOfMessagesToSend)).ConfigureAwait(false);
            }
        }

        private void StopAllSending()
        {
            foreach (var ct in cancellationTokens)
            {
                ct.Cancel();
            }

            cancellationTokens.Clear();

        }//End If

        public MainForm()
        {
            InitializeComponent();
            this.NumberOfMessages = 0;
            cancellationTokens = new List<CancellationTokenSource>();


        }//End MainForm Constructor

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.DestroyHandle();
            

        }//End ExitButton_Click

        private void SequenceSendingBtn_Click(object sender, EventArgs e)
        {
            if (SequenceSendingBtn.Text == "Start Sending")
            {
                //Ensure cancellation tokens list is cleared
                cancellationTokens.Clear();

                //groupBox1.Enabled = false;

                DisableCheckboxes();

                bool success = SendMessagesToCheckedStreams();

                if (!success)
                {
                    MessageBox.Show("No Channels Were Selected!");
                    SequenceSendingBtn.Text = "Start Sending";
                    EnableCheckboxes();
                    //groupBox1.Enabled = true;
                }//End If

                else
                    SequenceSendingBtn.Text = "Stop Sending";


            }//End If

            else
            {
                SequenceSendingBtn.Text = "Start Sending";
                StopAllSending();
                EnableCheckboxes();
                //groupBox1.Enabled = true;

            }//End Else

        }

        private bool SendMessagesToCheckedStreams(long numberOfMessagesToSend = -1)
        {
            bool flag = false;
            var checkedBoxes = Controls.OfType<CheckBox>().Where(box => box.Checked);
            foreach (var box in checkedBoxes)
            {
                CancellationTokenSource ctSource = new CancellationTokenSource();
                var streamType = box.Name.Replace("CheckBox", "");

                cancellationTokens.Add(ctSource);

                StartSendingMessages(streamType, ctSource.Token, numberOfMessagesToSend);
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

        }//End DisableCheckBoxes

        private void EnableCheckboxes()
        {
            BeamBusCasCheckBox.Enabled = true;
            BeamBusFasTasCheckBox.Enabled = true;
            StaveBusCasCheckBox.Enabled = true;
            StaveBusFasTasCheckBox.Enabled = true;
            IdrsCheckBox.Enabled = true;
            PrsStaveBusCheckBox.Enabled = true;

        }//End EnableCheckboxes

        private void QuantitySendingBtn_Click(object sender, EventArgs e)
        {
            if (QuantitySendingBtn.Text == "Send By Number")
            {
                var checkedBoxes = Controls.OfType<CheckBox>().Where(box => box.Checked);

                bool success = false;
                this.NumberOfMessages = (int)this.numericUpDown1.Value;

                DisableCheckboxes();
                QuantitySendingBtn.Text = "Stop Sending";


                success = SendMessagesToCheckedStreams(this.NumberOfMessages);

                if (!success)
                {
                    System.Windows.Forms.MessageBox.Show("No Channels Were Selected!");
                    QuantitySendingBtn.Text = "Send By Number";
                    EnableCheckboxes();

                }//End If

                //BeamBusCasNumberSenderThread.Join();

            }//End If

            else
            {
                QuantitySendingBtn.Text = "Send By Number";
                StopAllSending();
                EnableCheckboxes();

            }//End Else



        }//End QuantitySendingBtn

    }//End MainForm

}//End Improving Simulator
