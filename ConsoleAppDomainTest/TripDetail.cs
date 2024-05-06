using ConsoleAppDomainTest.Builder;
using ConsoleAppDomainTest.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDomainTest
{
    internal class TripDetail
    {
        public void CalcSample()
        {
            string sample = "Start=2024/4/29 10:00,Length=100,Dest1={M1;P(10-20);2024/4/29 10:10}";

            var trip = new Director().Construct(sample);
            int price = new Calculator().Calc(trip, new Rating { KMRate = 10 });
            Console.WriteLine(price);

            var map = new Director().Construct(trip);
            Console.WriteLine(map);

            var t2 = new Director().Construct(map);

            Console.WriteLine(trip.Equals(t2));
        }
    }

    namespace Domain
    {
        public class Trip
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public int Length { get; set; }
            public Dest[] Dests { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj is not Trip trip) return false;
                if (trip.Start != Start) return false;
                if (trip.Length != Length) return false;
                if (!trip.Dests[0].Equals(Dests[0])) return false;

                return true;
            }
        }

        public class Dest
        {
            public string Name { get; set; }
            public Point Loc { get; set; }
            public DateTime Time { get; set; }

            public override bool Equals(object? obj)
            {
                if (obj is not Dest dest) return false;
                if (!dest.Name.Equals(Name)) return false;
                if (dest.Loc.X != Loc.X || dest.Loc.Y != Loc.Y) return false;
                if (dest.Time != Time) return false;

                return true;
            }
        }
    }

    namespace Builder
    {
        public class Director
        {
            public Trip Construct(string tripDesc)
            {
                return new TextParser<Trip>().Parse(tripDesc, new ObjectBuilder());
            }

            internal string Construct(Trip trip)
            {
                return new ObjectParser<string>().Parse(trip, new StringBuilder());
            }
        }

        interface TripBuilder<T>
        {
            void BuildStart(DateTime start);
            void BuildLength(int length);
            void BuildDest(string name, Point p, DateTime time);

            T GetResult();
        }

        class ObjectBuilder : TripBuilder<Trip>
        {
            Trip trip = new();

            public void BuildStart(DateTime start)
            {
                trip.Start = start;
            }

            public void BuildLength(int length)
            {
                trip.Length = length;
            }

            public void BuildDest(string name, Point p, DateTime time)
            {
                trip.Dests = new Dest[1];
                trip.Dests[0] = new Dest();
                var d = trip.Dests[0];
                d.Name = name;
                d.Loc = p;
                d.Time = time;
            }

            public Trip GetResult()
            {
                return trip;
            }
        }

        class StringBuilder : TripBuilder<String>
        {
            string trip = "";
            //"Start=2024/4/29 10:00,Length=100,Dest1={M1;P(10-20);2024/4/29 10:10}"
            public void BuildStart(DateTime start)
            {
                trip += "Start=" + start;
            }

            public void BuildLength(int length)
            {
                trip += ",Length=" + length;
            }

            public void BuildDest(string name, Point p, DateTime time)
            {
                trip += ",Dest1={" + name + ";P(" + p.X + "-" + p.Y +");"+time+"}";
            }

            public string GetResult()
            {
                return trip;
            }
        }

        interface Parser<I, O>
        {
            O Parse(I input, TripBuilder<O> builder);
        }

        class ObjectParser<T> : Parser<Trip, T>
        {
            public T Parse(Trip trip, TripBuilder<T> builder)
            {
                builder.BuildStart(trip.Start);
                builder.BuildLength(trip.Length);
                builder.BuildDest(trip.Dests[0].Name, trip.Dests[0].Loc, trip.Dests[0].Time);

                return builder.GetResult();
            }
        }

        class TextParser<T> : Parser<String, T>
        {
            public T Parse(string tripDesc, TripBuilder<T> builder)
            {
                //"Start=2024/4/29 10:00,Length=100,Dest1={M1;P(10-20);2024/4/29 10:10}"
                var parts = tripDesc.Split([',']);
                foreach (var part in parts)
                {
                    var exp = part.Split(['=']);
                    switch (exp[0])
                    {
                        case "Start":
                            builder.BuildStart(Convert.ToDateTime(exp[1]));
                            break;
                        case "Length":
                            builder.BuildLength(Convert.ToInt32(exp[1]));
                            break;
                        case "Dest1":
                            var dp = exp[1].Trim('{', '}').Split(';');
                            var loc = dp[1].Trim('P', '(', ')').Split('-');
                            builder.BuildDest(dp[0], new Point(Convert.ToInt32(loc[0]), Convert.ToInt32(loc[1])), Convert.ToDateTime(dp[2]));
                            break;
                        default:
                            break;
                    }
                }

                return builder.GetResult();
            }
        }
    }


    public class Rating
    {
        public int KMRate { get; set; } = 10;
    }

    public class Calculator
    {
        public int Calc(Trip trip, Rating rating)
        {
            return trip.Length * rating.KMRate;
        }
    }

    
}
