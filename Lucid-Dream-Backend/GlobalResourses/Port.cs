
namespace GlobalResourses
{
    public class Port
    {
        protected readonly string _name;
        protected readonly int _number;
        protected bool _status;

        public Port(string name, int number)
        {
            this._name = name;
            this._number = number;
            this._status = false;

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

        public string GetName()
        {
            return this._name;

        }//End GetName

    }//End Port

}
