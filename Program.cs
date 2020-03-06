using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse8
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbHandler = new DbHandler();

            while (true)
            {
                Console.WriteLine("Please enter member id: ");
                int memberId;
                string value = Console.ReadLine();

                if(int.TryParse(value, out memberId))
                {
                    try
                    {
                        MemberDto member = dbHandler.GetMember(memberId);
                      
                        foreach (var prop in member.GetType().GetProperties())
                        {
                            Console.WriteLine($"{prop.Name} = {prop.GetValue(member, null)}");
                        }
                        Console.WriteLine(new string('-', 40));
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                } else
                {
                    Console.WriteLine("Invalid id");
                }
            }
        }
    }
}
