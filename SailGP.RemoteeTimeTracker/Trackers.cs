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
        double last_remote = 0;
        double filtered_remote = 0;
        double new_weight = 0.3;
        bool first = true;
        List<string> remotes;
        List<string> locals;
        List<string> filtered_remotes;
        List<Factor> factors= new List<Factor>();

        public double GetRemote(double local)
        {
            // Gradually move our filtered estimate toward the last remote value we saw
            //filtered_remote = (1 - new_weight) * filtered_remote + new_weight * last_remote;



            var first_local = locals.FirstOrDefault();
            var last_local = locals.LastOrDefault();
            var local_factor =Convert.ToDouble( $"{local:F2}") - Convert.ToDouble(last_local);
           
            var factor = factors.LastOrDefault();



            if(factor.local_factor.Equals("-"))
            {
                int i = 2;
                string value;
                do
                {
                    value = factors[factors.Count - i].remote_factor;
                    i++;
                }
                while (value.Equals("-"));

                factor = factors[factors.Count - i+ 1];
            }


            if(Convert.ToDouble(factor.local_factor).Equals(0))
            {
                if(Convert.ToDouble(filtered_remotes.LastOrDefault()).Equals(0))

                filtered_remote = Convert.ToDouble(locals.LastOrDefault())+0.03;
                else
                {
                    filtered_remote= Convert.ToDouble(filtered_remotes.LastOrDefault()) + 0.03;
                }

                //Math.Max(Convert.ToDouble($"{local:F2}"), Convert.ToDouble(remotes.LastOrDefault())) + local_factor;
            }
            else if (Convert.ToDouble(factor.local_factor)>0)
            {
                if(local_factor<0.03)
                {
                    filtered_remote = Convert.ToDouble(filtered_remotes.LastOrDefault()) + local_factor + (Convert.ToDouble($"{local:F2}") ;


                }
                else
                {
                    filtered_remote = Convert.ToDouble(filtered_remotes.LastOrDefault()) + 0.03;
                }

              //  filtered_remote = Convert.ToDouble(remotes.LastOrDefault()) + Convert.ToDouble(factor.local_factor);
               
            }

            filtered_remotes.Add($"{filtered_remote:F2}");


            return filtered_remote;
        }

        public void Set(int id, double local, double remote)
        {

            if (!first )
            {
                var local_factor = Convert.ToDouble($"{local:F2}") - Convert.ToDouble(locals.LastOrDefault());
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
                // Start by completely trusting the first report we receive
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
            last_remote = remote;

            var remotes_str = String.Join(", ", remotes.ToArray());

            Console.WriteLine($"Remote Times: {remotes_str}");
            Console.WriteLine($"Local Times: {String.Join(", ", locals.ToArray())}");
            Console.WriteLine($"Local Factors: {String.Join(", ", factors.Select(x=>x.local_factor).ToArray())}");
            Console.WriteLine($"Remote Factors: {String.Join(", ", factors.Select(x=>x.remote_factor).ToArray())}");
            Console.WriteLine("-------------------------------------");
        }
    }
}
