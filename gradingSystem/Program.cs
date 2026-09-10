using System;
using System.Collections.Generic;
using System.IO;

namespace gradingSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = "studentsScores.txt";
            string outputFilePath = "studentsReports.txt";

        

            StudentResultProcessor processor = new StudentResultProcessor();

            try
            {
                Console.WriteLine("Reading student data from file...");
                List<Student> students = processor.ReadStudentsFromFile(inputFilePath);

                Console.WriteLine($"Successfully read {students.Count} students.");

                Console.WriteLine("\n\nWriting report to output file...");
                processor.WriteReportToFile(students, outputFilePath);

               
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"[FILE NOT FOUND ERROR] {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"[INVALID SCORE ERROR] {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"[MISSING FIELD ERROR] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UNEXPECTED ERROR] {ex.Message}");
            }
        }
    }

    // student class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        // method for assigning grades according to the score range
        public string GetGrade()
        {
            if (Score >= 80 && Score <= 100) return "A";
            else if (Score >= 70 && Score <= 79) return "B";
            else if (Score >= 60 && Score <= 69) return "C";
            else if (Score >= 50 && Score <= 59) return "D";
            return "F";
        }

        //  method for better representation of student data
        public override string ToString() =>
            $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }

    // custom exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // student result processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException($"The input file you specified was not found: {inputFilePath}");
            }

            List<Student> students = new();

            using (StreamReader reader = new(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');

                    // Line must contain Id, FullName, and Score
                    if (parts.Length < 3)
                    {
                        throw new MissingFieldException($"Check Line {lineNumber}. Expected 3 fields, found {parts.Length}");
                    }

                    string rawId = parts[0].Trim();
                    string fullName = parts[1].Trim();
                    string rawScore = parts[2].Trim();

                    if (string.IsNullOrEmpty(rawId) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(rawScore))
                    {
                        throw new MissingFieldException($"Line {lineNumber} has empty values: '{line}'");
                    }

                    if (!int.TryParse(rawId, out int id))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid student ID format '{rawId}'.");
                    }

                    if (!int.TryParse(rawScore, out int score))
                    {
                        throw new InvalidScoreFormatException($"Line {lineNumber}: Score '{rawScore}' cannot be converted to an integer.");
                    }

                    students.Add(new Student(id, fullName, score));
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine(student.ToString());
                }

                Console.WriteLine($"\nReport successfully written to {outputFilePath}");
            }
        }
    }
}