using CG.Business.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CG.Secrets.Models
{
    /// <summary>
    /// This class represents a secret value.
    /// </summary>
    public class Secret : ModelBase<string>
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties

        /// <summary>
        /// This property contains the name of the secret.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This property contains the value of the secret.
        /// </summary>
        public string Value { get; set; }

        #endregion
    }
}
