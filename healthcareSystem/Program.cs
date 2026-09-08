using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareSystemApp
{

    internal class Program
    {
        static void Main(string[] args)
        {
            HealthSystemApp appInstance = new();

            appInstance.SeedData();

            appInstance.BuildPrescriptionMap();

            appInstance.PrintAllPatients();

            Console.WriteLine("\n");

            appInstance.PrintPrescriptionsForPatient(30001);

            Console.WriteLine("\n");

            appInstance.PrintPrescriptionsForPatient(30002);

            
        }
    }

    // the generic class 
    public class Repository<T>
    {
        // field
        private readonly List<T> items = new();

        // methods
        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return new List<T>(items);
        }

        // return the first match by condition or null
        public T? GetById(Func<T, bool> predicate)
        {
            return items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var itemToRemove = items.FirstOrDefault(predicate);
            if (itemToRemove != null)
            {
                return items.Remove(itemToRemove);
            }

            return false;
        }
    }

    // patient class
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        // used to convert the patient object to a string representation for display
        public override string ToString() =>
            $"[Patient ID: {Id}] {Name,-18} | Age: {Age,-3} | Gender: {Gender}";
    }

    // prescription class
    public class Prescription
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string MedicationName { get; set; }
        public DateTime DateIssued { get; set; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
        // used to convert the prescription object to a string representation for display
        public override string ToString() =>
            $"  - {MedicationName,-22} (Issued: {DateIssued:yyyy-MM-dd})";
    }

    // integrated system
    public class HealthSystemApp
    {
        // fields 
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // methods
        public void SeedData()
        {
            // See 3 patient objects
            _patientRepo.Add(new Patient(30001, "Naki Meah", 35, "Male"));
            _patientRepo.Add(new Patient(30002, "Saali Zang", 28, "Female"));
            _patientRepo.Add(new Patient(30003, "Amin Karim", 43, "Male"));

            // 5 prescription objects referencing valid patient IDs
            _prescriptionRepo.Add(new Prescription(1, 30001, "Amoxicillin 500mg", DateTime.Today));
            _prescriptionRepo.Add(new Prescription(2, 30003, "Paracetamol 1g", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(3, 30002, "Metformin 850mg", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(4, 30002, "Nuvit 10mg", DateTime.Today.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(5, 30003, "Artesunate 60mg", DateTime.Today.AddDays(-2)));
        }

        // group prescriptions by PatientId
        public void BuildPrescriptionMap()
        {
            _prescriptionMap = new Dictionary<int, List<Prescription>>();

            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }

                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        // get prescriptions by PatientId from the grouped map
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
            {
                return prescriptions;
            }
            return new List<Prescription>();
        }

        // Print all patients from the patient repository
        public void PrintAllPatients()
        {
            Console.WriteLine("PATIENT DIRECTORY\n-------------------------");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"  - {patient} \n");
            }
           
        }

        // print prescription set for a specific patient
        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            string patientHeader = patient != null ? $"{patient.Name} (Patient ID: {patientId})" : $"Patient #{patientId}";

            Console.WriteLine($"Prescriptions for: {patientHeader}\n-----------------------------------------------------");

            var prescriptions = GetPrescriptionsByPatientId(patientId);

            if (prescriptions.Count == 0)
            {
                Console.WriteLine("  No prescriptions recorded for this patient.");
            }
            
            else
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }

           
        }
    }

   
}