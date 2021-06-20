using Fuel_Georgia_Parser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Services
{
    class ReadmeUpdater
    {
        const string start = "<!--PRICING-START-->", end = "<!--PRICING-END-->", readme = "README.md";
        public static void UpdatePricing(Company[] companies, string userName, string repo, string branch)
        {
            if (File.Exists(readme))
            {
                var content = File.ReadAllText(readme);

                var startIndex = content.IndexOf(start);
                var endIndex = content.IndexOf(end);

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    var pre = content.Substring(0, startIndex + start.Length);

                    var mid = getContent();

                    var app = content.Substring(endIndex);

                    File.WriteAllText(readme, string.Concat(pre, mid, app));
                }
            }


            string getContent()
            {
                StringBuilder b = new(Environment.NewLine);
                b.AppendLine($"ამჯერად ყოველდღიურად მოწმდება {companies.Length} კომპანიის მონაცემები");


                b.AppendLine("<div>");
                foreach (var company in companies)
                    b.AppendLine($"<img src=\"https://raw.githubusercontent.com/{userName}/{repo}/{branch}/blob/{company.Key}.png\" alt=\"{company.Key} logo\" width=\"50\" >");
                b.AppendLine("</div>");

                b.AppendLine("კომპანიების საწვავის მიმდინარე ფასები მოცემულია შემდეგ ცხრილებში");

                foreach (var company in companies)
                {
                    b.AppendLine("<table>");

                    b.AppendLine($"<tr><th colSpan=\"3\">{company.Name}</th></tr>");
                    b.AppendLine($"<tr><th>სახელი</th><th>ფასი</th><th>ცვლილება</th></th></tr>");

                    foreach (var fuel in company.Fuels)
                    {
                        b.AppendLine($"<tr><td>{fuel.Name}</td><td>{fuel.Price:N2}</td><td>{sign()}{Math.Abs(fuel.Change):N2}</td></tr>");

                        string sign() => fuel.Change switch
                        {
                            < 0 => "-",
                            > 0 => "+",
                            _ => ""
                        };
                    }

                    b.AppendLine("</table>");
                }

                b.AppendLine();
                return b.ToString();
            }

        }


    }
}
