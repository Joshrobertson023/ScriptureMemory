using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Requests;
public class PassageMemorizedRequest
{
    public UserPassage Passage { get; set; }
    public int PointsGained { get; set; }
}
