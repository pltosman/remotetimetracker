/*
 * TimeTracker test application
 * Copyright (c) 2020, SailGP, All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeTrackerTest
{

    /// <summary>
    /// A Tracker receives reports of remote time reports, along
    /// with the local time at which the report was received.
    /// The calling app can query the Tracker at any time - generally
    /// much more frequently than tie reports come in - to
    /// ask for the predicted remote time given a local time.
    /// </summary>
    public interface ITracker
    {
        void Set(int id, double local, double remote);
        double GetRemote(double local);
    }


    public class Factor
    {

        public string local_factor { get; set; }
        public string remote_factor { get; set; }
    }

    /// <summary>
    /// Here's a very simple implementation of a Tracker. It remembers
    /// the last remote time it saw, and continually moves an estimated
    /// remote time value toward that remembered value with every call to
    /// GetRemote().  Note this doesn't try to estimate the rate of remote
    /// time change, and will always lag the actual remote time.
    /// </summary>
    public class SimpleTracker : ITracker
    {
        int remoteMessageCount = 0;
        double filtered_remote = 0;
        bool first = true;
        List<string> remotes;
        List<string> locals;
        List<string> filtered_remotes;
        List<Factor> factors = new List<Factor>();
        const double local_incr = 0.03;
 
       
        public double GetRemote(double local)
        {
          
            var lastLocal = locals.LastOrDefault();
            var lastRemote = ConvertToDouble( remotes.LastOrDefault());
            var lastFactor = factors.LastOrDefault();
            var lastFilteredRemote = ConvertToDouble(filtered_remotes.LastOrDefault());
            var local_factor = Convert.ToDouble($"{local:F2}") - Convert.ToDouble(lastLocal);

            bool IsLastFilteredRemotesZero = lastFilteredRemote.Equals(0);
            bool IsLastLocalFactorZero = ConvertToDouble(lastFactor.local_factor).Equals(0);
            bool IsLastLocalFactorBiggerThenZero = ConvertToDouble(lastFactor.local_factor) > 0;
            double tenpercentOfLastRemote = lastRemote * 0.1;
            bool IsInTenPercent = lastFilteredRemote < lastRemote - tenpercentOfLastRemote || lastFilteredRemote > lastRemote + tenpercentOfLastRemote ? false : true;


         

            if(remoteMessageCount!=remotes.Count)
            {
                remoteMessageCount = remotes.Count;

                if (!IsInTenPercent)
                {
                    lastFilteredRemote = lastRemote;
                }
            }


            //Firstly I check every local time value when they are come.
            //If next one is some the latest one,I added '-' on factor.

            if (lastFactor.local_factor.Equals("-"))
            {
                int i = 2;
                string value;
                do
                {
                    value = factors[factors.Count - i].remote_factor;
                    i++;
                }
                while (value.Equals("-"));

                lastFactor = factors[factors.Count - i+ 1];
            }

            // When set method call, add a new item in Factor. If this is the first time, Factor properties have a zero value. 
            if(IsLastLocalFactorZero)
            {
                if(IsLastFilteredRemotesZero)

                filtered_remote = ConvertToDouble(lastLocal)+ local_incr;
                else
                {
                    filtered_remote= lastFilteredRemote + local_incr;
                }
            }
            else if (IsLastLocalFactorBiggerThenZero)
            {
                if(local_factor< local_incr)
                {
                    filtered_remote = ConvertToDouble(lastFilteredRemote) + local_factor; // + ConvertToDouble(local);
                }
                else
                {
                    filtered_remote = lastFilteredRemote + local_incr;
                }
            }

            filtered_remotes.Add($"{filtered_remote:F2}");


            return filtered_remote;
        }
        public double ConvertToDouble(object value)
        {
           double returnVal;
           var result=  double.TryParse($"{value:F2}", out returnVal);
            if (!result)
                return 0;

            return returnVal;

        }


        public void Set(int id, double local, double remote)
        {

            if (!first )
            {
                var local_factor = ConvertToDouble(local) - Convert.ToDouble(locals.LastOrDefault());
                var remote_factor = ConvertToDouble(remote) - Convert.ToDouble(remotes.LastOrDefault());
                if(local_factor>0 )
                factors.Add( new Factor {local_factor=$"{local_factor:F2}", remote_factor= $"{remote_factor:F2}" });
                else
                {
                    factors.Add(new Factor { local_factor = "-", remote_factor = "-" });
                }

            }

            if (first)
            {
                first = false;
                filtered_remote = remote;
                remotes = new List<string>();
                locals = new List<string>();
                filtered_remotes = new List<string>();
                filtered_remotes.Add($"{filtered_remote:F2}");

                factors.Add(new Factor { local_factor = "0", remote_factor = "0" });


            }
            remotes.Add($"{remote:F2}");
            locals.Add($"{local:F2}");
           

            var remotes_str = String.Join(", ", remotes.ToArray());


            /// I had added this to check all the answers.
            Console.WriteLine($"Remote Times: {remotes_str}");
            Console.WriteLine($"Local Times: {String.Join(", ", locals.ToArray())}");
            Console.WriteLine($"Local Factors: {String.Join(", ", factors.Select(x=>x.local_factor).ToArray())}");
            Console.WriteLine($"Remote Factors: {String.Join(", ", factors.Select(x=>x.remote_factor).ToArray())}");
            Console.WriteLine("-------------------------------------");
        }
    }
}
