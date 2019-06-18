using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationContato.Models
{
    public class Results
    {
        public string count;
        public string next;
        public string previous;
        public List<ResultsPlanetas> results;        
    }

    public class ResultsPlanetas
    {        
        public string name;
        public string population;
        public string climate;
        public string terrain;
        public List<string> films;        
    }
}