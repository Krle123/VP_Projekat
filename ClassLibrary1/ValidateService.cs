using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [DataContract]
    public class ValidationFault
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Message { get; set; }
    }

    [DataContract]
    public class DataFormatFault
    {
        [DataMember] public string Message { get; set; }
    }

    [ServiceContract]
    public interface IValidateService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        [FaultContract(typeof(DataFormatFault))]
        void Validate(MotorSample sample);
    }
        public class ValidateService : IValidateService
        {
            public void Validate(MotorSample sample)
            {
                if (sample == null)
                    throw new FaultException<DataFormatFault>(
                        new DataFormatFault { Message = "Objekat je null" },
                        new FaultReason("Invalid data format"));

                if (sample.ProfileId <= 0)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "ProfileId", Message = "'ProfileId' mora biti veci od 0" },
                        new FaultReason("Validation error"));

                if (sample.Coolant <= 0)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "Coolant", Message = "'Coolant' temperatura mora biti veca od 0" },
                        new FaultReason("Validation error"));

                if (sample.Ambient < -50 || sample.Ambient > 100)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "Ambient", Message = "Ambient mora biti izmedju -50 i 100 °C" },
                        new FaultReason("Validation error"));

                if (sample.I_q < -500 || sample.I_q > 500)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "I_q", Message = "I_q izvan dozvoljenog dometa [-500, 500]" },
                        new FaultReason("Validation error"));

                if (sample.I_d < -500 || sample.I_d > 500)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "I_d", Message = "I_d izvan dozvoljenog dometa [-500, 500]" },
                        new FaultReason("Validation error"));

                if (sample.Torque < 0)
                    throw new FaultException<ValidationFault>(
                        new ValidationFault { Field = "Torque", Message = "Torque mora biti ne-negativan" },
                        new FaultReason("Validation error"));

                Console.WriteLine($"Sample valid: ProfileId={sample.ProfileId}, Coolant={sample.Coolant}");
            }
        }
    }
