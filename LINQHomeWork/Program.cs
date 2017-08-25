using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace LINQHomeWork
{
    class Program
    {
        static void Main(string[] args)
        {

            //1.) Napraviti metodu koja prima n te vratiti proste brojeve od 1 do n.

            //Console.WriteLine("Unesite neki broj:");
            //var n = int.Parse(Console.ReadLine());
            //var numbers = Enumerable.Range(2, n)
            //            .Where(number =>
            //            Enumerable.Range(2, number - 2)
            //            .All(divisor => number % divisor != 0));
            //var numbers = Enumerable.Range(1, n)
            //    .Where(number =>
            //    {
            //        if (number == 2)
            //            return true;
            //        if (number < 2 || number % 2 == 0)
            //            return false;
            //        for (int i = 3; i * i <= number; i += 2)
            //            if (number % i == 0) return false;
            //        return true;
            //    })
            //    .ToList();
            //Console.WriteLine("Prosti brojevi su:");
            //foreach (var number in numbers)
            //    Console.Write("{0} ", number);

            //2.) Napraviti metodu koja prima n te racuna broj djelitelja svakog broja
            // od 1 do n. Na temelju toga racuna "slabost" broja gledajuci koliko brojeva
            // manjih od njega ima veci broj djelitelja od njega. Metoda vraca polje ciji
            // prvi clan govori kolika je slabost najslabijeg broja (odnosno koji ima
            // najvecu slabost), a drugi koliko brojeva ima tu slabost.

            //    Console.WriteLine("Unesite neki broj:");
            //    var n = int.Parse(Console.ReadLine());
            //    var numberOfDivisorsList = Enumerable.Range(1, n).Select(Divisors).ToList();
            //    var weaknessList = numberOfDivisorsList
            //        .Select((x, index) => 
            //            numberOfDivisorsList
            //            .Take(index)
            //            .Count(number => number > x)
            //        ).ToList();
            //    for (int i = 1; i <= n; i++)
            //    {
            //        Console.WriteLine("Number: {0} Divisors: {1} Weakness: {2}",
            //            i, numberOfDivisorsList[i - 1], weaknessList[i-1]);
            //    }
            //    var weakestNumber = weaknessList.Max();
            //    var numberOfWeakestNumbers = weaknessList.Count(w => w == weakestNumber);
            //    Console.WriteLine("The weakest value is {0}, the number of such numbers is {1}",
            //        weakestNumber,numberOfWeakestNumbers);
            //
            //static int Divisors(int x)
            //{
            //    int limit = x;
            //    int numberOfDivisors = 0;

            //    if (x == 1) return 1;

            //    for (int i = 1; i < limit; ++i)
            //    {
            //        if (x % i == 0)
            //        {
            //            limit = x / i;
            //            if (limit != i)
            //            {
            //                numberOfDivisors++;
            //            }
            //            numberOfDivisors++;
            //        }
            //    }
            //    return numberOfDivisors;
            //}


            //3.)
            //Stvarna baza:
            //(Restorat je prvo, proučiti entitete dobro, pokušat rekreirat priču koliko možetete onda ako zapnete pitat). 
            //a) Potrebno je pronaci koliko je projekata pravilno ispunjeno, ali nije predano. 
            //b) Potrebno je izracunati koliko je novaca sveukupno traženo od SZST-a, 
            //   te koliko je novaca svekupuno potrebo, te koliki postotak čini prvi dio od drugog.
            //c) Potrebno je pronaći sve projekte koji su ocjenjeni do kraja. (4 sudca su ih ocijenila).
            //   Natječaji se prijavljuju u pet kategorija (Radionice i predavanja, STudentski mediji, ...)
            //   Natječaj dodjeljuje 350.000,00 kuna, tako da se 200.000,00 kuna podijeli najbolje ocijenjenim projektima 
            //   sveukupno, te jos 30.000,00 kuna se podijeli po svakoj kategoriji.
            //d) Potrebno je izračunati te na kraju ispisati koliko će koji projekt dobiti novaca te to ispisati.

            var context =new SzstApplicationContext();

            var unsubmittedCorrectProjectsCount =
                context.Applications.Count(application => application.IsValid && !application.IsSubmitted);
            Console.WriteLine("{0} projekata je pravilno ispunjeno, ali nije predano.",unsubmittedCorrectProjectsCount);
            Console.WriteLine();

            var totalMoneyRequested = context.Answers.Where(index => index.QuestionIndex == "4.3")
                .ToList()
                .Aggregate(0.0, (amount, answer) =>
                {
                    if (answer.Value == null)
                        return amount;
                    var value=StringConverter(answer.Value);
                    return amount + double.Parse(value);
                });

            var totalMoneyNeeded = context.Answers.Where(index => index.QuestionIndex == "4.1")
                .ToList()
                .Aggregate(0.0, (amount, answer) =>
                {
                    if (answer.Value == null)
                        return amount;
                    var value=StringConverter(answer.Value);
                    return amount + double.Parse(value);
                });
           
            Console.WriteLine("Sveukupno je trazeno {0} kn, sto cini {1}% od sveukupno potrebnih {2} kn",
                totalMoneyRequested,totalMoneyRequested/totalMoneyNeeded*100,totalMoneyNeeded);
            Console.WriteLine();

            var completelyGradedProjects = context.Grades.GroupBy(project => project.ApplicationId)
                .Select(group => new {id = group.Key, count = group.Count()})
                .Where(project => project.count == 4);
            Console.WriteLine("Potpuno ocijenjeni projekti su:");
            foreach (var project in completelyGradedProjects)
                Console.Write(project.id + " ");
            Console.WriteLine();

            var bestGradedProjects = context.Grades.Include(grade => grade.Application)
                .GroupBy(project => project.ApplicationId).ToList()
                .Select(group => new
                {
                    id = group.Key,
                    moneyRequested = context.Answers
                                    .Where(answer => answer.ApplicationId==group.Key && answer.QuestionIndex == "4.3")
                                    .Select(answer=>answer.Value)
                                    .FirstOrDefault(),
                    category = context.Applications.Where(application => application.Id==group.Key).Select(application=> application.ApplicationType).FirstOrDefault(),
                    total = group.Select(application => application.Application.ObjectiveGrade).FirstOrDefault() + group.Aggregate(0, (total, current) => total
                                                                   + current.Sustainability
                                                                   + current.Workflow
                                                                   + current.SzstPromotion
                                                                   + current.SupportFromOtherOrganizations
                                                                   + current.Innovation
                                                                   + current.ImportanceForOtherStudents
                                                                   + current.SubjectiveOpinion)
                })
                .OrderByDescending(projectsGraded => projectsGraded.total);

            var grandPrize = 200000.0;
            var grandPrizeWinners = bestGradedProjects.TakeWhile(project =>
            {
                var value = StringConverter(project.moneyRequested);

                if (grandPrize > double.Parse(value))
                {
                    grandPrize -= double.Parse(value);
                    return true;
                }

                if (grandPrize > 0)
                {
                    grandPrize = -grandPrize;
                    return true;
                }

                return false;
            }).ToList();

            Console.WriteLine("Glavni iznos (200 000 kn) se dodjeljuje na način:");
            Console.WriteLine();
            foreach (var winner in grandPrizeWinners)
            {
                    if(winner.Equals(grandPrizeWinners.Last()))
                    Console.WriteLine("Projektu {0} se dodjeljuje {1} kn", winner.id, -grandPrize);
                    else
                    Console.WriteLine("Projektu {0} se dodjeljuje {1} kn", winner.id,
                        double.Parse(StringConverter(winner.moneyRequested)));
            }

            var smallPrize = 30000.0;
            Console.WriteLine();
            Console.WriteLine("Sredstva koja se dijele po kategorijama (30 000 po svakoj):");
            Console.WriteLine();
            foreach (var category in Categories)
            {
                Console.WriteLine("U kategoriji {0} na način:",category.Value);
                var smallPrizeWinners=bestGradedProjects.Skip(grandPrizeWinners.Count).Where(project =>
                {
                    if (project.category != int.Parse(category.Key))
                        return false;

                    var value = StringConverter(project.moneyRequested);

                    if (smallPrize > double.Parse(value))
                    {
                        smallPrize -= double.Parse(value);
                        return true;
                    }

                    if (smallPrize > 0)
                    {
                        smallPrize = -smallPrize;
                        return true;
                    }

                    return false;
                }).ToList();
                foreach (var winner in smallPrizeWinners)
                {
                    if (winner.Equals(smallPrizeWinners.Last()))
                        Console.WriteLine("Projektu {0} se dodjeljuje {1} kn", winner.id, -smallPrize);
                    else
                        Console.WriteLine("Projektu {0} se dodjeljuje {1} kn", winner.id,
                            double.Parse(StringConverter(winner.moneyRequested)));
                }
                smallPrize = 30000.0;
                Console.WriteLine();
            }
        }

        public static string StringConverter(string stringToConvert)
        {
            string value;
            if (stringToConvert.IndexOf(",") < stringToConvert.IndexOf("."))
            {
                value = stringToConvert.Replace(",", "");
                value = value.Replace(".", ",");
            }
            else
                value = stringToConvert.Replace(".", "");

            return value;
        }

        public static Dictionary<string, string> Categories { get; set; } = new Dictionary<string, string>()
        {
            {"1", "Radionice, predavanja i tribine"},
            {"2", "Studentski mediji i kulturna događanja"},
            {"3", "Znanstveno-istraživački rad"},
            {"4", "Inovativno-tehnološki projekti"},
            {"5", "Šport"}
        };

    }
}
