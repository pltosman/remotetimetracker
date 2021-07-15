/*
 * TimeTracker test application
 * Copyright (c) 2020, SailGP, All Rights Reserved
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        double lastRemote = 0;
        double filtered_remote = 0;
        double new_weight = 0.3;
        bool first = true;
        List<string> remotes;
        List<string> locals;
        List<string> filtered_remotes;
        List<Factor> factors = new List<Factor>();
        const double local_incr = 0.03;
 
       
        public double GetRemote(double local)
        {
          
            var lastLocal = locals.LastOrDefault(); 
            var lastFactor = factors.LastOrDefault();
            var lastFilteredRemotes = filtered_remotes.LastOrDefault();
            var local_factor = Convert.ToDouble($"{local:F2}") - Convert.ToDouble(lastLocal);



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
            if(ConvertToDouble(lastFactor.local_factor).Equals(0))
            {
                if(ConvertToDouble(lastFilteredRemotes).Equals(0))

                filtered_remote = ConvertToDouble(locals.LastOrDefault())+ local_incr;
                else
                {
                    filtered_remote= ConvertToDouble(lastFilteredRemotes) + local_incr;
                }
            }
            else if (ConvertToDouble(lastFactor.local_factor)>0)
            {
                if(local_factor< local_incr)
                {
                    filtered_remote = ConvertToDouble(lastFilteredRemotes) + local_factor + (ConvertToDouble($"{local:F2}") );
                }
                else
                {
                    filtered_remote = ConvertToDouble(lastFilteredRemotes) + local_incr;
                }
            }

            filtered_remotes.Add($"{filtered_remote:F2}");


            return filtered_remote;
        }
        public double ConvertToDouble(object value)
        {
            return Convert.ToDouble($"{value:F2}");
        }


        public void Set(int id, double local, double remote)
        {

            if (!first )
            {
                var local_factor = ConvertToDouble(local) - Convert.ToDouble(locals.LastOrDefault());
                var remote_factor = Convert.ToDouble($"{remote:F2}") - Convert.ToDouble(remotes.LastOrDefault());
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
            lastRemote = remote;

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
