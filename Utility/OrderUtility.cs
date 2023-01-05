using Microsoft.Extensions.Options;
using LaCafelogy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace LaCafelogy.Utility
{
    public class OrderUtility
    {
        private readonly DBContext _dbContext;

        public OrderUtility(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

       
        

    }
}



