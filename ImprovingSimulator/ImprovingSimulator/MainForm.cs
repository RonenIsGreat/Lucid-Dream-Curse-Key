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
using System.Runtime.InteropServices;

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

                IPAddress udpIp = IPAddress.Parse(ConfigurationManager.AppSettings["UdpRemoteIp"]);

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
                DisableCheckboxes();
                NumberSendingBtn.Enabled = false;

                var success = SendMessagesToCheckedStreams();

                if (TargetsCheckbox.Checked == true)
                {
                    targetsCancellationTokenSource = new CancellationTokenSource();
                    TargetsStreamer targetsStreamer = TargetsStreamer.Instance;
                    targetsCancellationTokenSource = new CancellationTokenSource();

                    // Do it in background
                    Task.Run(() => targetsStreamer.StartSending(targetsCancellationTokenSource.Token));
                    panel2.Visible = true;

                }//End If

                if (!success)
                {
                    MessageBox.Show("No Channels Were Selected!");
                    SequenceSendingBtn.Text = "Start Sending";
                    EnableCheckboxes();
                    NumberSendingBtn.Enabled = true;
                    panel2.Visible = false;

                } //End If
                else
                {
                    panel2.Visible = true;
                    SequenceSendingBtn.Text = "Stop Sending";
                }
            } //End If

            else
            {
                if (TargetsCheckbox.IsDisposed == true)
                    targetsCancellationTokenSource.Cancel();

                SequenceSendingBtn.Text = "Start Sending";
                StopAllSending();
                EnableCheckboxes();
                NumberSendingBtn.Enabled = true;
                this.panel2.Visible = false;

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
            TargetsCheckbox.Enabled = false;

        } //End DisableCheckBoxes

        private void EnableCheckboxes()
        {
            BeamBusCasCheckBox.Enabled = true;
            BeamBusFasTasCheckBox.Enabled = true;
            StaveBusCasCheckBox.Enabled = true;
            StaveBusFasTasCheckBox.Enabled = true;
            IdrsCheckBox.Enabled = true;
            PrsStaveBusCheckBox.Enabled = true;
            TargetsCheckbox.Enabled = true;

        } //End EnableCheckboxes

        private void QuantitySendingBtn_Click(object sender, EventArgs e)
        {
            if (QuantitySendingBtn.Text == "Send By Number")
            {

                NumberOfMessages = (int)numericUpDown1.Value;

                DisableCheckboxes();
                QuantitySendingBtn.Text = "Stop Sending";
                TimeSendingBtn.Enabled = false;

                var success = SendMessagesToCheckedStreams(NumberOfMessages);

                if (!success)
                {
                    MessageBox.Show("No Channels Were Selected!");
                    QuantitySendingBtn.Text = "Send By Number";
                    EnableCheckboxes();
                    TimeSendingBtn.Enabled = true;

                } //End If

                //BeamBusCasNumberSenderThread.Join();
            } //End If

            else
            {
                QuantitySendingBtn.Text = "Send By Number";
                StopAllSending();
                EnableCheckboxes();
                TimeSendingBtn.Enabled = true;

            } //End Else
        } //End QuantitySendingBtn

        //private void SendTargetsBtn_Click(object sender, EventArgs e)
        //{
        //    if (SendTargetsBtn.Text == "Start Sending Targets")
        //    {
        //        NumberSendingBtn.Enabled = false;
        //        TimeSendingBtn.Enabled = false;
        //        targetsCancellationTokenSource = new CancellationTokenSource();
        //        TargetsStreamer targetsStreamer = TargetsStreamer.Instance;
        //        targetsCancellationTokenSource = new CancellationTokenSource();
        //        SendTargetsBtn.Text = "Stop Sending Targets";

        //        // Do it in background
        //        Task.Run(() => targetsStreamer.StartSending(targetsCancellationTokenSource.Token));
        //    }
        //    else
        //    {
        //        targetsCancellationTokenSource.Cancel();
        //        SendTargetsBtn.Text = "Start Sending Targets";
        //        NumberSendingBtn.Enabled = true;
        //        TimeSendingBtn.Enabled = true;
        //    }
        //}

        private void DisplayingNumberSendingComponents()
        {
            HeadlineLabel.Visible = false;
            SubHeadlineLabel.Visible = false;
            SendByNumberPanel.Visible = true;
            QuantitySendingBtn.Visible = true;
            label3.Visible = true;
            BeamBusCas.Visible = true;
            BeamBusFasTasLabel.Visible = true;
            StaveBusCasLabel.Visible = true;
            StaveBusFasTasLabel.Visible = true;
            PrsStaveBusLabel.Visible = true;
            IdrsLabel.Visible = true;
            TargetsLabel.Visible = false;
            BeamBusCasCheckBox.Visible = true;
            BeamBusFasTasCheckBox.Visible = true;
            StaveBusCasCheckBox.Visible = true;
            StaveBusFasTasCheckBox.Visible = true;
            PrsStaveBusCheckBox.Visible = true;
            IdrsCheckBox.Visible = true;
            TargetsCheckbox.Visible = false;
            SequenceSendingBtn.Visible = false;


        }//End DisableNumberSendingComponents

        private void DisplayingTimeSendingComponent()
        {
            HeadlineLabel.Visible = false;
            SubHeadlineLabel.Visible = false;
            SendByNumberPanel.Visible = false;
            QuantitySendingBtn.Visible = false;
            label3.Visible = true;
            BeamBusCas.Visible = true;
            BeamBusFasTasLabel.Visible = true;
            StaveBusCasLabel.Visible = true;
            StaveBusFasTasLabel.Visible = true;
            PrsStaveBusLabel.Visible = true;
            IdrsLabel.Visible = true;
            TargetsLabel.Visible = true;
            BeamBusCasCheckBox.Visible = true;
            BeamBusFasTasCheckBox.Visible = true;
            StaveBusCasCheckBox.Visible = true;
            StaveBusFasTasCheckBox.Visible = true;
            PrsStaveBusCheckBox.Visible = true;
            IdrsCheckBox.Visible = true;
            TargetsCheckbox.Visible = true;
            SequenceSendingBtn.Visible = true;

        }//End DisableTimeSendingComponent

        private void NumberSendingBtn_Click(object sender, EventArgs e)
        {
            DisplayingNumberSendingComponents();

        }//End NumberSendingBtn_Click

        private void TimeSendingBtn_Click(object sender, EventArgs e)
        {
            DisplayingTimeSendingComponent();

        }//End TimeSendingBtn_Click

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;

        }//End MinimizeButton_Click

        private void BeamBusFasTasLabel_Click(object sender, EventArgs e)
        {
            if (BeamBusFasTasCheckBox.Checked == true)
                BeamBusFasTasCheckBox.Checked = false;
            else
                BeamBusFasTasCheckBox.Checked = true;

        }//End BeamBusFasTasLabel_Click

        private void BeamBusCas_Click(object sender, EventArgs e)
        {
            if (BeamBusCasCheckBox.Checked == true)
                BeamBusCasCheckBox.Checked = false;
            else
                BeamBusCasCheckBox.Checked = true;

        }//End BeamBusCas_Click

        private void IdrsLabel_Click(object sender, EventArgs e)
        {
            if (IdrsCheckBox.Checked == true)
                IdrsCheckBox.Checked = false;
            else
                IdrsCheckBox.Checked = true;

        }//End IdrsLabel_Click

        private void PrsStaveBusLabel_Click(object sender, EventArgs e)
        {
            if (PrsStaveBusCheckBox.Checked == true)
                PrsStaveBusCheckBox.Checked = false;
            else
                PrsStaveBusCheckBox.Checked = true;

        }//End PrsStaveBusLabel_Click

        private void StaveBusCasLabel_Click(object sender, EventArgs e)
        {
            if (StaveBusCasCheckBox.Checked == true)
                StaveBusCasCheckBox.Checked = false;
            else
                StaveBusCasCheckBox.Checked = true;

        }//End StaveBusCasLabel

        private void StaveBusFasTasLabel_Click(object sender, EventArgs e)
        {
            if (StaveBusFasTasCheckBox.Checked == true)
                StaveBusFasTasCheckBox.Checked = false;
            else
                StaveBusFasTasCheckBox.Checked = true;

        }//End StaveBusFasTasLabel_click

        private void TargetsLabel_Click(object sender, EventArgs e)
        {
            if (TargetsCheckbox.Checked == true)
                TargetsCheckbox.Checked = false;
            else
                TargetsCheckbox.Checked = true;

        }//End TargetLabel_Click

    }

} //End Improving Simulator