
namespace GlobalResourses
{
    public class Port
    {
        protected readonly ChannelNames _name;
        protected readonly int _number;
        protected bool _status;
        protected bool iswitch;

        public Port(ChannelNames name, int number)
        {
            this._name = name;
            this._number = number;
            this._status = false;
            this.iswitch = false;

        }//End Port Constructor

        public void SetStatus(bool status)
        {
            this._status = status;

        }//End SetStatus

        public bool GetStatus()
        {
            return this._status;

        }//End SetStatus

        public int GetPortNumber()
        {
            return this._number;

        }//End GetPortNumber

        public ChannelNames GetName()
        {
            return this._name;

        }//End GetName

        public void setSwitch(bool iBool)
        {
            this.iswitch = iBool;
        }

        public bool getSwitch()
        {
            return this.iswitch;
        }

    }//End Port

}
