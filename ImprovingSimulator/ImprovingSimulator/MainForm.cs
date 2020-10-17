using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using StreamWrapper = StreamWrapper.StreamWrapper;

namespace ImprovingSimulator
{
    public partial class MainForm : Form
    {
        private int NumberOfMessages;

        #region SequenceThreads

        //Creating Thread For Each Channel For The Sequence Sending

        Thread BeamBusFasTasSenderThread = new Thread(delegate ()
        {
            global::BeamBusFasTas.BeamBusFasTasSender.SendMessage();

        });

        Thread BeamBusCasSenderThread = new Thread(delegate ()
        {
            global::BeamBusCas.BeamBusCasSender.SendMessage();
        });

        Thread IdrsSenderThread = new Thread(delegate ()
        {
            global::IDRS.IdrsSender.SendMessage();
        });

        Thread PrsStaveBusSenderThread = new Thread(delegate ()
        {
            global::PrsStaveBus.PrsStaveBusSender.SendMessage();
        });

        Thread StaveBusCasSenderThread = new Thread(delegate ()
        {
            global::StaveBusCas.StaveBusCasSender.SendMessage();
        });

        Thread StaveBusFasTasSenderThread = new Thread(delegate ()
        {
            global::StaveBusFasTas.StaveBusFasTasSender.SendMessage();
        });

        #endregion

        #region QuantityThreads

        //Creating Thread For Each Channel For The Quantity Sending

        Thread BeamBusFasTasNumberSenderThread;

        Thread BeamBusCasNumberSenderThread;

        Thread IdrsNumberSenderThread;

        Thread PrsStaveBusNumberSenderThread;

        Thread StaveBusCasNumberSenderThread;

        Thread StaveBusFasTasNumberSenderThread;

        #endregion

        //Methods For Activating The Threads
        private async Task BeamBusFasTasSendMessage()
        {
            global::StreamWrapper.StreamWrapper a = new global::StreamWrapper.StreamWrapper("", IPAddress.Loopback.ToString(), 0);
            CancellationToken da = new CancellationToken();


            await Task.Run(() => a.SendMessages(da));

        }//End BeamBusFasTasSendMessage

        private void BeamBusCasSendMessage()
        {
            BeamBusCasSenderThread.SetApartmentState(ApartmentState.STA);
            BeamBusCasSenderThread.Start();

        }//End BeamBusCasSendMessage

        private void IdrsSendMessgae()
        {
            IdrsSenderThread.SetApartmentState(ApartmentState.STA);
            IdrsSenderThread.Start();

        }//End IdrsSendMessage

        private void PrsStaveBusSendMessage()
        {
            PrsStaveBusSenderThread.SetApartmentState(ApartmentState.STA);
            PrsStaveBusSenderThread.Start();

        }//End PrsStaveBusSendMessage

        private void StaveBusCasSendMessage()
        {
            StaveBusCasSenderThread.SetApartmentState(ApartmentState.STA);
            StaveBusCasSenderThread.Start();

        }//End StaveBusCasSendMessage

        private void StaveBusFasTasSendMessage()
        {
            StaveBusFasTasSenderThread.SetApartmentState(ApartmentState.STA);
            StaveBusFasTasSenderThread.Start();

        }//End SaveBusFasTasSendMessage

        private void RestartThreads()
        {
            //Restarting The Beam Bus Cas Thread `
            if (BeamBusCasCheckBox.Checked)
            {
                BeamBusCasSenderThread.Abort();
                BeamBusCasSenderThread = null;
                BeamBusCasSenderThread = new Thread(delegate ()
                {
                    global::BeamBusCas.BeamBusCasSender.SendMessage();

                });

            }//End If

            //Restarting The Beam Bus Fas/Tas Thread
            if (BeamBusFasTasCheckBox.Checked)
            {
                BeamBusFasTasSenderThread.Abort();
                BeamBusFasTasSenderThread = null;
                BeamBusFasTasSenderThread = new Thread(delegate ()
                {
                    global::BeamBusFasTas.BeamBusFasTasSender.SendMessage();

                });

            }//End If

            //Restarting The Prs Stave Bus
            if (PrsStaveBusCheckBox.Checked)
            {
                PrsStaveBusSenderThread.Abort();
                PrsStaveBusSenderThread = null;
                PrsStaveBusSenderThread = new Thread(delegate ()
                {
                    global::PrsStaveBus.PrsStaveBusSender.SendMessage();

                });

            }//End If

            //Restarting The Idrs 
            if (IdrsCheckBox.Checked)
            {
                IdrsSenderThread.Abort();
                IdrsSenderThread = null;
                IdrsSenderThread = new Thread(delegate ()
                {
                    global::IDRS.IdrsSender.SendMessage();

                });

            }//End If

            //Restarting The Stave Bus Cas
            if (StaveBusCasCheckBox.Checked)
            {
                StaveBusCasSenderThread.Abort();
                StaveBusCasSenderThread = null;
                StaveBusCasSenderThread = new Thread(delegate ()
                {
                    global::StaveBusCas.StaveBusCasSender.SendMessage();

                });

            }//End If

            //Restarting The Stave Bus Fas/Tas
            if (StaveBusFasTasCheckBox.Checked)
            {
                StaveBusFasTasSenderThread.Abort();
                StaveBusFasTasSenderThread = null;
                StaveBusFasTasSenderThread = new Thread(delegate ()
                {
                    global::StaveBusFasTas.StaveBusFasTasSender.SendMessage();

                });

            }//End If

        }//End RestartThreads

        private void RestartThreadsByNumber()
        {
            //Restarting The Beam Bus Cas Thread `
            if (BeamBusCasCheckBox.Checked)
            {
                BeamBusCasNumberSenderThread.Abort();
                BeamBusCasNumberSenderThread = null;

            }//End If

            //Restarting The Beam Bus Fas/Tas Thread
            if (BeamBusFasTasCheckBox.Checked)
            {
                BeamBusFasTasNumberSenderThread.Abort();
                BeamBusFasTasNumberSenderThread = null;

            }//End If

            //Restarting The Prs Stave Bus
            if (PrsStaveBusCheckBox.Checked)
            {
                PrsStaveBusNumberSenderThread.Abort();
                PrsStaveBusNumberSenderThread = null;

            }//End If

            //Restarting The Idrs 
            if (IdrsCheckBox.Checked)
            {
                IdrsNumberSenderThread.Abort();
                IdrsNumberSenderThread = null;

            }//End If

            //Restarting The Stave Bus Cas
            if (StaveBusCasCheckBox.Checked)
            {
                StaveBusCasNumberSenderThread.Abort();
                StaveBusCasNumberSenderThread = null;

            }//End If

            //Restarting The Stave Bus Fas/Tas
            if (StaveBusFasTasCheckBox.Checked)
            {
                StaveBusFasTasNumberSenderThread.Abort();
                StaveBusFasTasNumberSenderThread = null;

            }//End If

        }//End RestartThreadsByNumber

        public MainForm()
        {
            InitializeComponent();
            this.NumberOfMessages = 0;


        }//End MainForm Constructor

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.DestroyHandle();
            

        }//End ExitButton_Click

        private void SequenceSendingBtn_Click(object sender, EventArgs e)
        {
            if (SequenceSendingBtn.Text.ToString() == "Start Sending")
            {
                int Flag = 0;

                //groupBox1.Enabled = false;
                DisableCheckboxes();
                SequenceSendingBtn.Text = "Stop Sending";

                if (BeamBusFasTasCheckBox.Checked)
                {
                    BeamBusFasTasSendMessage();
                    Flag = 1;

                }//End If

                if (BeamBusCasCheckBox.Checked)
                {
                    BeamBusCasSendMessage();
                    Flag = 1;

                }//End If

                if (IdrsCheckBox.Checked)
                {
                    IdrsSendMessgae();
                    Flag = 1;

                }//End If

                if (PrsStaveBusCheckBox.Checked)
                {
                    PrsStaveBusSendMessage();
                    Flag = 1;

                }//End If

                if (StaveBusCasCheckBox.Checked)
                {
                    StaveBusCasSendMessage();
                    Flag = 1;

                }//End If

                if (StaveBusFasTasCheckBox.Checked)
                {
                    StaveBusFasTasSendMessage();
                    Flag = 1;

                }//End If

                if (Flag == 0)
                {
                    System.Windows.Forms.MessageBox.Show("No Channels Were Selected!");
                    SequenceSendingBtn.Text = "Start Sending";
                    EnableCheckboxes();
                    //groupBox1.Enabled = true;

                }//End If

            }//End If

            else
            {
                SequenceSendingBtn.Text = "Start Sending";
                RestartThreads();
                EnableCheckboxes();
                //groupBox1.Enabled = true;

            }//End Else

        }//End Else

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
                int Flag = 0;
                this.NumberOfMessages = (int)this.numericUpDown1.Value;

                DisableCheckboxes();
                QuantitySendingBtn.Text = "Stop Sending";

                if (this.BeamBusCasCheckBox.Checked)
                {
                    Flag = 1;

                    this.BeamBusCasNumberSenderThread = new Thread(delegate ()
                    {
                        global::BeamBusCas.BeamBusCasSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.BeamBusCasNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.BeamBusCasNumberSenderThread.Start();

                }//End If

                if (this.BeamBusFasTasCheckBox.Checked)
                {
                    Flag = 1;

                    this.BeamBusFasTasNumberSenderThread = new Thread(delegate ()
                    {
                        global::BeamBusFasTas.BeamBusFasTasSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.BeamBusFasTasNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.BeamBusFasTasNumberSenderThread.Start();

                }//End If


                if (this.PrsStaveBusCheckBox.Checked)
                {
                    Flag = 1;

                    this.PrsStaveBusNumberSenderThread = new Thread(delegate ()
                    {
                        global::PrsStaveBus.PrsStaveBusSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.PrsStaveBusNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.PrsStaveBusNumberSenderThread.Start();

                }//End If

                if (this.IdrsCheckBox.Checked)
                {
                    Flag = 1;

                    this.IdrsNumberSenderThread = new Thread(delegate ()
                    {
                        global::IDRS.IdrsSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.IdrsNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.IdrsNumberSenderThread.Start();

                }//End If

                if (this.StaveBusCasCheckBox.Checked)
                {
                    Flag = 1;

                    this.StaveBusCasNumberSenderThread = new Thread(delegate ()
                    {
                        global::StaveBusCas.StaveBusCasSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.StaveBusCasNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.StaveBusCasNumberSenderThread.Start();

                }//End If


                if (this.StaveBusFasTasCheckBox.Checked)
                {
                    Flag = 1;

                    this.StaveBusFasTasNumberSenderThread = new Thread(delegate ()
                    {
                        global::StaveBusFasTas.StaveBusFasTasSender.SendNumberOfMessages(this.NumberOfMessages);

                    });

                    this.StaveBusFasTasNumberSenderThread.SetApartmentState(ApartmentState.STA);
                    this.StaveBusFasTasNumberSenderThread.Start();

                }//End If

                if (Flag == 0)
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
                RestartThreadsByNumber();
                EnableCheckboxes();

            }//End Else



        }//End QuantitySendingBtn

    }//End MainForm

}//End Improving Simulator
