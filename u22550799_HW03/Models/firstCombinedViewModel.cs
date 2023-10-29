using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace u22550799_HW03.Models
{
    public class firstCombinedViewModel
    { 
        public IPagedList<students> students { get; set; }
        public IPagedList<books> books { get; set; }    
    }
}