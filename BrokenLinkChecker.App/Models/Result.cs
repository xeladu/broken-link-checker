using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokenLinkChecker.App.Models;
internal class Result
{
    public int BrokenLinkCount { get; set; }
    public int TotalLinkCount { get; set; }
    public IEnumerable<Link> BrokenLinks { get; set; }
}
