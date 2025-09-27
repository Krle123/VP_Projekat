using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [DataContract]
    public class MotorSample
    {
        private double i_q;
        private double i_d;
        private double coolant;
        private int profile_id;
        private double ambient;
        private double torque;

        [DataMember]
        public double I_q { get { return i_q; } set { i_q = value; } }
        [DataMember]
        public double I_d { get { return i_d; } set { i_d = value; } }
        [DataMember]
        public double Coolant { get { return coolant; } set { coolant = value; } }
        [DataMember]
        public double Torque { get { return torque; } set { torque = value; } }
        [DataMember]
        public int ProfileId { get { return profile_id; } set { profile_id = value; } }
        [DataMember]
        public double Ambient { get { return ambient; } set { ambient = value; } }
    }
}
