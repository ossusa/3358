using System;
using System.Linq;
using MatrixGroup.Implementation.Helix.Model;

namespace SitefinityWebApp.Mvc.Models
{
    public class AccountModel: HelixIndividual
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
        
    }
}