using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTCP
{
    [Serializable]
    public class Person
    {
        string _firstName = String.Empty;
        string _lastName = String.Empty;
        string _fullName = String.Empty;

        private Person()
        {
        }

        public Person(string lastName, string firstName)
        {
            _lastName = lastName;
            _firstName = firstName;
        }

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public override string ToString()
        {
            _fullName = _firstName + " " + _lastName;
            return _fullName;
        }
    }
}
