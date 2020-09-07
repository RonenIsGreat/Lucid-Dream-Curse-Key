namespace GlobalResourses
{
    public class ChannelDetails
    {
        protected readonly ChannelNames _name;
        protected readonly int _number;
        protected bool _status;

        public ChannelDetails(ChannelNames name, int number)
        {
            _name = name;
            _number = number;
            _status = false;
        } //End Port Constructor

        public void SetStatus(bool status)
        {
            _status = status;
        } //End SetStatus

        public bool GetStatus()
        {
            return _status;
        } //End SetStatus

        public int GetPortNumber()
        {
            return _number;
        } //End GetPortNumber

        public ChannelNames GetName()
        {
            return _name;
        } //End GetName
    } //End Port
}