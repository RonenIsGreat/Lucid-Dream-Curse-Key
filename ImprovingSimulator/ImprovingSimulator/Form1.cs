using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using BeamBusFasTas;

namespace ImprovingSimulator
{
    public partial class Form1 : Form
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

        Thread PrsStaveBusMessageSenderThread;

        Thread StaveBusCasNumberSenderThread;

        Thread StaveBusFasTasNumberSenderThread;

        #endregion

        public Form1()
        {
            InitializeComponent();
            this.NumberOfMessages = 0;
            this.SequenceSendingRadioBtn.Checked = true;

        }//End Form1

        //Methods For Activating The Threads
        private void BeamBusFasTasSendMessage()
        {
            BeamBusFasTasSenderThread.SetApartmentState(ApartmentState.STA);
            BeamBusFasTasSenderThread.Start();

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
                PrsStaveBusMessageSenderThread.Abort();
                PrsStaveBusMessageSenderThread = null;

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

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text.ToString() == "Start Sending")
            {
                int Flag = 0;

                groupBox1.Enabled = false;
                DisableCheckboxes();
                button1.Text = "Stop Sending";

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
                    button1.Text = "Start Sending";
                    EnableCheckboxes();
                    groupBox1.Enabled = true;

                }//End If

            }//End If

            else
            {
                button1.Text = "Start Sending";
                RestartThreads();
                EnableCheckboxes();
                groupBox1.Enabled = true;

            }//End Else

        }//End StartSending_Click

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

        private void SequenceSendingRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            label1.Hide();
            numericUpDown1.Hide();
            SendByNumber.Hide();
            button1.Show();
        }

        private void QuantitySendingRadioBtn_CheckedChanged(object sender, EventArgs e)
        {
            label1.Show();
            numericUpDown1.Show();
            SendByNumber.Show();
            button1.Hide();
        }

        private void SendByNumber_Click_1(object sender, EventArgs e)
        {
            if (SendByNumber.Text == "Send By Number")
            {
                int Flag = 0;
                this.NumberOfMessages = (int)this.numericUpDown1.Value;

                groupBox1.Enabled = false;
                DisableCheckboxes();
                SendByNumber.Text = "Stop Sending";

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

                if (Flag == 0)
                {
                    System.Windows.Forms.MessageBox.Show("No Channels Were Selected!");
                    SendByNumber.Text = "Send By Number";
                    EnableCheckboxes();
                    groupBox1.Enabled = true;

                }//End If

                //BeamBusCasNumberSenderThread.Join();

            }//End If

            else
            {
                SendByNumber.Text = "Send By Number";
                RestartThreadsByNumber();
                EnableCheckboxes();
                groupBox1.Enabled = true;

            }//End Else


        }//End SendByNumber-Click1

    }

}//End ImprovingSimulator
